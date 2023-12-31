﻿using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace ASqlite
{
    public class Sqlite
    {
        public static string ConnectString = "Data Source=base.db";
        public SqlExecute Execute => new SqlExecute(ConnectString);
        public SqliteConnection? Conn => Execute.Connection;
        public Sqlite()
        {
        }

        public void CreateTable(Type type)
        {
            new SqlExecute(new SqlCommand().ToSqlCreateTable(type)).Execute();
        }

        public void DropTable(Type type)
        {
            new SqlExecute(new SqlCommand().ToSqlDropTable(type)).Execute();
        }

        public object? Create<T>(T obj)
            where T : new()
        {
            return new SqlExecute(new SqlCommand().ToSqlCreate(typeof(T), obj)).ExecuteReturnScalar();
        }

        public object? Read<T>(string sql)
        {
            return new SqlExecute(sql).ExecuteReturnScalar();
        }

        public List<T>? Read<T>(int pageIndex, int pageSize)
            where T : new()
        {

            return new SqlExecute(new SqlCommand().ToSqlRead(typeof(T), pageIndex, pageSize)).ExecuteReturn<T>();
        }

    }

    public static class SqliteOutput
    {
        public static bool Show = true;
        public static void Write(string msg)
        {
            if(Show) Console.Write(msg);
        }
        public static void WriteLine(string msg)
        {
            if(Show) Console.WriteLine(msg);
        }
    }

    public class SqlExecute : IDisposable
    {
        protected string Command;
        public SqliteConnection? Connection;
        public SqlExecute(string sql)
        {
            Connection = new SqliteConnection(Sqlite.ConnectString);
            Connection.Open();
            Command = sql;
        }

        public virtual bool Execute()
        {
            if (Connection == null) return false;

            var command = Connection.CreateCommand();
            command.CommandText = Command;

            SqliteOutput.Write(Command);

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                    }
                }
            }
            catch(Exception err)
            {
                SqliteOutput.WriteLine($" - Failure: {err.Message}");
                return false;
            }

            SqliteOutput.WriteLine(" - Ok");

            return true;
        }

        public virtual List<T>? ExecuteReturn<T>()
            where T : new()
        {
            if (Connection == null) return null;

            var command = Connection.CreateCommand();
            command.CommandText = Command;

            SqliteOutput.Write(Command);

            try
            {
                var objs = new List<T>();
                using (var reader = command.ExecuteReader())
                {
                    // 取得 Schema
                    ReadOnlyCollection<DbColumn>? schema = null;
                    if (reader.CanGetColumnSchema())
                    {
                        schema = reader.GetColumnSchema();
                    }

                    while (reader.Read())
                    {
                        var obj = new T();
                        foreach(var prop in typeof(T).GetProperties())
                        {
                            var SC = SqlAttribute.GetSC(prop);
                            var s = schema?.FirstOrDefault(p => p.ColumnName == prop.Name);
                            if (SC == null || s == null || s.DataType == null) continue;

                            var nullType = Nullable.GetUnderlyingType(prop.PropertyType);
                            if (prop.PropertyType == typeof(DateTime) || nullType == typeof(DateTime))
                            {
                                if(DateTime.TryParse(reader.GetValue(SC.Name).ToString(), out var dt))
                                {
                                    prop.SetValue(obj, dt);
                                }
                                else
                                {
                                    prop.SetValue(obj, default);
                                }
                            }
                            else
                            {
                                prop.SetValue(obj, reader.GetValue(SC.Name));
                            }
                        }
                        objs.Add(obj);
                    }
                }

                SqliteOutput.WriteLine(" - Ok");
                return objs;
            }
            catch(Exception err)
            {
                SqliteOutput.WriteLine($" - Failure: {err.Message}");
                return null;
            }
        }

        public virtual object? ExecuteReturnScalar()
        {
            if (Connection == null) return null;

            var command = Connection.CreateCommand();
            command.CommandText = Command;

            SqliteOutput.Write(Command);

            try
            {
                var obj = command.ExecuteScalar();
                SqliteOutput.WriteLine(" - Ok");
                return obj;
            }
            catch(Exception err)
            {
                SqliteOutput.WriteLine($" - Failure: {err.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            Connection?.Close();
            Connection = null;
        }
    }

    public class SqlCommand
    {
        public string ToSqlCreateTable(Type type)
        {
            // 蒐集 Sqlite Column Attribute 減少重新尋找
            var SCs = new List<SCAttribute>();
            var ST = SqlAttribute.GetST(type);
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE {ST?.Name} (");
            var props = type.GetProperties();
            for(int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                var attr = SqlAttribute.GetSC(prop);
                if (attr == null) break;
                SCs.Add(attr);

                sb.Append(attr.ToStringImpl(1));
                if(i != props.Length - 1)
                {
                    sb.Append(",");
                }
                sb.AppendLine();
            }
            sb.AppendLine(");");

            if (SCs.Any(p => p.IsIndex))
            {
                sb.Append("CREATE INDEX IDX_" + ST.Name + " ON " + ST.Name);
                sb.AppendLine("(" + String.Join(", ", SCs.Where(e => e.IsIndex).Select(e => e.Name)) + ");");
            }

            return sb.ToString();
        }

        public string ToSqlDropTable(Type type)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"DROP TABLE {SqlAttribute.GetST(type)?.Name}");
            return sb.ToString();
        }

        public string ToSqlCreate(Type type, object obj)
        {
            // 蒐集 Sqlite Column Attribute 減少重新尋找
            var vals = new List<string>();
            var cols = new List<string>();
            var ST = SqlAttribute.GetST(type);
            var sb = new StringBuilder();
            sb.AppendLine($"INSERT INTO {ST?.Name} (");
            var props = type.GetProperties();
            for(int i = 0; i < props.Length; i++)
            {
                var prop = props[i];

                var SC = SqlAttribute.GetSC(prop);

                // 排除沒有 Sqlite Column 設定
                if (SC == null) continue;
                // 排除自增鍵
                if (SC.AutoIncrement == true) continue;
                var val = prop?.GetValue(obj)?.ToString();
                // 排除預設但給空值
                if (!String.IsNullOrEmpty(SC.Default) && String.IsNullOrEmpty(val)) continue;
                cols.Add(new string(' ', 4) + prop.Name);
                if(val == null) continue;
                switch (SC.Type)
                {
                    case SCAttribute.CType.CHAR:
                    case SCAttribute.CType.VARCHAR:
                    case SCAttribute.CType.NVARCHAR:
                    case SCAttribute.CType.DATETIME:
                        val = "'" + val + "'";
                        break;
                }

                vals.Add(val);
            }
            sb.AppendLine(String.Join("," + Environment.NewLine, cols));
            sb.AppendLine(") VALUES");
            // 這裡未來可以改為 Composite
            sb.AppendLine("(" + String.Join(", ", vals) + ")");

            return sb.ToString();
        }

        public string ToSqlRead(Type type, int pageIndex, int pageSize)
        {
            // 蒐集 Sqlite Column Attribute 減少重新尋找
            var ST = SqlAttribute.GetST(type);
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT * FROM {ST?.Name} LIMIT {pageIndex * pageSize}, {pageSize}");

            return sb.ToString();
        }

        public string ToSqlCount(Type type)
        {
            var ST = SqlAttribute.GetST(type);
            var sb = new StringBuilder();
            sb.AppendLine($"SELECT COUNT(*) FROM {ST?.Name}");

            return sb.ToString();
        }
    }

    public class SqlAttribute
    {
        /// <summary>
        /// 取得 Sqlite Table Attribute
        /// </summary>
        public static STAttribute? GetST(Type type)
        {
            var res = type.GetCustomAttributes(typeof(STAttribute), true)
                .Cast<STAttribute>()
                .FirstOrDefault();

            return res;
        }

        /// <summary>
        /// 取得 Sqlite Column Attribute
        /// </summary>
        public static SCAttribute? GetSC(PropertyInfo type)
        {
            var res = type.GetCustomAttributes(typeof(SCAttribute), true)
                .Cast<SCAttribute>()
                .FirstOrDefault();

            return res;
        }
    }

    /// <summary>
    /// Sqlite Column 描述
    /// </summary>
    public class SCAttribute : Attribute
    {
        /// <summary>
        /// Column Type
        /// </summary>
        public enum CType
        {
            VARCHAR,
            NVARCHAR,
            CHAR,
            INT,
            DECIMAL,
            DATETIME,
        }

        /// <summary>
        /// Sqlite Column 類型
        /// </summary>
        public CType Type { get; set; } = CType.INT;
        /// <summary>
        /// 長度 (僅支援類型有效)
        /// </summary>
        public int Len { get; set; } = 10;
        /// <summary>
        /// 小數位 (僅支援類型有效)
        /// </summary>
        public int Point { get; set; } = 2;
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// PK 主鍵
        /// </summary>
        public bool IsPrimaryKey {  get; set; }
        /// <summary>
        /// Index 索引
        /// </summary>
        public bool IsIndex { get; set; }
        /// <summary>
        /// 自增鍵 (僅支援類型有效)
        /// </summary>
        public bool AutoIncrement { get; set; }
        /// <summary>
        /// 預設值
        /// </summary>
        public string Default { get; set; } = "";

        public string ToStringImpl(int indent)
        {
            var sb = new StringBuilder();
            sb.Append(new string(' ', 4 * indent));
            sb.Append(Name + " ");
            switch (Type)
            {
                case CType.VARCHAR:
                    sb.Append(CType.VARCHAR.ToString());
                    sb.Append("(" + Len + ")");
                    break;
                case CType.NVARCHAR:
                    sb.Append(CType.VARCHAR.ToString());
                    sb.Append("(" + Len + ")");
                    break;
                case CType.CHAR:
                    sb.Append(CType.CHAR.ToString());
                    sb.Append("(" + Len + ")");
                    break;
                case CType.INT:
                    sb.Append("INTEGER");
                    break;
                case CType.DATETIME:
                    sb.Append(CType.DATETIME.ToString());
                    break;
                case CType.DECIMAL:
                    sb.Append(CType.DECIMAL.ToString());
                    sb.Append("(" + Len + "," + Point + ")");
                    break;
            }

            if (!String.IsNullOrEmpty(Default))
            {
                sb.Append($" DEFAULT {Default}");
            }

            if (IsPrimaryKey)
            {
                sb.Append(" PRIMARY KEY");
            }

            if (Type == CType.INT && AutoIncrement)
            {
                sb.Append(" AUTOINCREMENT");
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Sqlite Table 描述
    /// </summary>
    public class STAttribute : Attribute
    {
        public string Name { get; set; }
    }

    [ST(Name = "EnglishNoteBook")]
    public class EnglishNoteBook
    {
        [SC(Name = "EngID", Type = SCAttribute.CType.INT, IsPrimaryKey = true, AutoIncrement = true)]
        public long EngID { get; set; }
        [SC(Name = "English", Type = SCAttribute.CType.VARCHAR, Len = 500)]
        public string English { get; set; }
    }

    [ST(Name = "EnglishTranslate")]
    public class EnglishTranslate
    {
        [SC(Name = "ID", Type = SCAttribute.CType.INT, IsPrimaryKey = true, AutoIncrement = true)]
        public long ID { get; set; }
        [SC(Name = "EngID", Type = SCAttribute.CType.INT, IsIndex = true)]
        public long EngID { get; set; }
        [SC(Name = "Translate", Type = SCAttribute.CType.NVARCHAR, Len = 500)]
        public string Translate { get; set; }
    }

    [ST(Name = "EnglishPronounce")]
    public class EnglishPronounce
    {
        [SC(Name = "ID", Type = SCAttribute.CType.INT, IsPrimaryKey = true, AutoIncrement = true)]
        public long ID { get; set; }
        [SC(Name = "EngID", Type = SCAttribute.CType.INT, IsIndex = true)]
        public long EngID { get; set; }
        [SC(Name = "Pronounce", Type = SCAttribute.CType.NVARCHAR, Len = 500)]
        public string Pronounce { get; set; }
    }

    [ST(Name = "EnglishFocusTime")]
    public class EnglishFocusTime
    {
        [SC(Name = "ID", Type = SCAttribute.CType.INT, IsPrimaryKey = true, AutoIncrement = true)]
        public long ID { get; set; }
        [SC(Name = "EngID", Type = SCAttribute.CType.INT, IsIndex = true)]
        public long EngID { get; set; }
        [SC(Name = "FocusTime", Type = SCAttribute.CType.INT, Len = 500)]
        public int FocusTime { get; set; }
    }

}