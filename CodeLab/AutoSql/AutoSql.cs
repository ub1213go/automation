using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLab.AutoSql
{
    public interface ISqlTable
    {

        public string ToStringImpl(int indent = 0)
        {
            var sb = new StringBuilder();

            foreach (var prop in GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    sb.AppendLine($"{new string(' ', indent * 4)}{prop.Name}: {prop.GetValue(this) ?? ""}");
                }
                else if (prop.PropertyType == typeof(int))
                {
                    sb.AppendLine($"{new string(' ', indent * 4)}{prop.Name}: {Convert.ToString(prop.GetValue(this)) ?? ""}");
                }
                else if (
                    prop.PropertyType.IsGenericType &&
                    prop.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                )
                {
                    sb.AppendLine(prop.Name);
                    foreach (var val in (IEnumerable<ISqlTable>)prop.GetValue(this))
                    {
                        sb.AppendLine(val.ToStringImpl(indent + 1));
                    }
                }
                else if (prop.PropertyType.GetInterfaces().Contains(typeof(ISqlTable)))
                {
                    sb.AppendLine(prop.Name);
                    sb.AppendLine(((ISqlTable)(prop.GetValue(this))).ToStringImpl(indent + 1));
                }
            }
                
            return sb.ToString();
        }
    }
    public class EmployeeHead : ISqlTable
    {
        public string? ID { get; set; } = "";
        public string? Name { get; set; } = "";
        public int Age { get; set; } = 0;
    }
    public class EmployeeBody : ISqlTable
    {
        public string ID { get; set; } = "";
        public string Address { get; set; } = "";
    }
    public class Employee : ISqlTable
    {
        public EmployeeHead? Head { get; set; } = new EmployeeHead();
        public IEnumerable<EmployeeBody>? Body { get; set; } = new List<EmployeeBody>();
    }
    public class EmployeeBuilder
    {
        protected Employee Employee { get; set; } = new Employee();
        public EmployeeHeadBuilder EmployeeHeadBuilder => new EmployeeHeadBuilder(Employee);
        public EmployeeBodyBuilder EmployeeBodyBuilder => new EmployeeBodyBuilder(Employee);
        public static implicit operator Employee(EmployeeBuilder eb)
        {
            Debug.Assert(!String.IsNullOrEmpty(eb.Employee.Head.ID));

            return eb.Employee;
        }
    }
    public class EmployeeHeadBuilder : EmployeeBuilder
    {
        public EmployeeHeadBuilder(Employee emp)
        {
            Employee = emp;
        }

        public EmployeeHeadBuilder SetID(string ID)
        {
            Employee.Head.ID = ID;
            return this;
        }
        
        public EmployeeHeadBuilder SetName(string Name)
        {
            Employee.Head.Name = Name;
            return this;
        }

        public EmployeeHeadBuilder SetAge(int age)
        {
            Employee.Head.Age = age;
            return this;
        }
    }
    public class EmployeeBodyBuilder : EmployeeBuilder
    {
        public EmployeeBodyBuilder(Employee emp)
        {
            Employee = emp;
        }

        public EmployeeBodyBuilder SetAddress(string address)
        {
            Employee.Body = Employee.Body.Append(new EmployeeBody() { ID = Employee.Head.ID, Address = address });
            return this;
        }
    }
    public class AutoSql<T>
        where T : ISqlTable, new()
    {
        public ISqlTable Table { get; set; }
        public AutoSql(T tb)
        {
            Table = tb;
        }
        public override string ToString()
        {
            return Table.ToStringImpl();
        }
    }
    //public static class ExtensionMethods
    //{
    //    public static T GetT<T>(this AutoSql<T> autosql)
    //        where T : ISqlTable, new()
    //    {
    //        return (T)autosql.Table;
    //    }
    //}
}
