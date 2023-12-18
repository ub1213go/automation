using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeLab.Condition
{
    public interface ICopyable<T>
        where T : new()
    {
        void CopyTo(T obj);
        public T DeepCopy()
        {
            T t = new T();
            CopyTo(t);
            return t;
        }
    }
    public static class CopyableExtension
    {
        public static T DeepCopy<T>(this ICopyable<T> self)
            where T : new()
        {
            return self.DeepCopy();
        }

    }
    public class Condition<T>
        where T : new()
    {
        public Expression Exp { get; set; }
    }
    public class ConditionObject<T> : Condition<T>, ICopyable<ConditionObject<T>>
        where T : new()
    {
        protected internal Expression Left;
        protected internal Expression Right;
        protected internal ParameterExpression Obj { get; set; }
        public ConditionObject()
        {
            Obj = Expression.Parameter(typeof(T), "leftObj");
        }
        public ConditionObject(ParameterExpression obj)
        {
            Obj = obj;
        }
        public ConditionBuilder<T> Builder => new ConditionBuilder<T>(this);
        public Func<T, bool> Lambda
        {
            get
            {
                var lambda = Expression.Lambda<Func<T, bool>>(
                    Exp,
                    Obj
                );

                return lambda.Compile();
            }
        }
        public void CopyTo(ConditionObject<T> obj)
        {
            obj.Obj = Obj;
            obj.Exp = Exp;
            obj.Left = Left;
            obj.Right = Right;
        }
    }
    public class ConditionBuilder<T>
        where T : new()
    {
        protected internal ConditionObject<T> Condition;
        public ConditionBuilder()
        {
            Condition = new ConditionObject<T>();
        }
        public ConditionBuilder(ConditionObject<T> condition)
        {
            Condition = condition.DeepCopy();
        }
        public OpLogicConditionBuilder<T> Logic => new OpLogicConditionBuilder<T>(Condition);
        public OpConditionBuilder<T> Op => new OpConditionBuilder<T>(Condition);
        public static implicit operator ConditionObject<T>(ConditionBuilder<T> condition)
        {
            condition.Condition.Exp = condition.Condition.Left;

            return condition.Condition;
        }
    }
    public class OpConditionBuilder<T> : ConditionBuilder<T>
        where T : new()
    {
        public OpConditionBuilder() : base() { }
        public OpConditionBuilder(ConditionObject<T> condition) : base(condition) {}
        public OpConditionBuilder<T> Equal(string name, object value)
        {
            Expression exp;

            exp = Expression.Equal(
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name)
                ),
                Expression.Constant(value)
            );

            if(Condition.Left == null)
            {
                Condition.Left = exp;
            }
            else
            {
                Condition.Right = exp;
            }
            return this;
        }
        public OpConditionBuilder<T> LessThanOrEqual(string name, object value)
        {
            Expression exp;

            exp = Expression.LessThanOrEqual(
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name)
                ),
                Expression.Constant(value)
            );

            if(Condition.Left == null)
            {
                Condition.Left = exp;
            }
            else
            {
                Condition.Right = exp;
            }
            return this;
        }

        public OpConditionBuilder<T> LessThan(string name, object value)
        {
            Expression exp;

            exp = Expression.LessThan(
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name)
                ),
                Expression.Constant(value)
            );

            if(Condition.Left == null)
            {
                Condition.Left = exp;
            }
            else
            {
                Condition.Right = exp;
            }
            return this;
        }
        public OpConditionBuilder<T> GreaterThanOrEqual(string name, object value)
        {
            Expression exp;

            exp = Expression.GreaterThanOrEqual(
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name)
                ),
                Expression.Constant(value)
            );

            if(Condition.Left == null)
            {
                Condition.Left = exp;
            }
            else
            {
                Condition.Right = exp;
            }
            return this;
        }
        public OpConditionBuilder<T> GreaterThan(string name, object value)
        {
            Expression exp;

            exp = Expression.GreaterThan(
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name)
                ),
                Expression.Constant(value)
            );

            if(Condition.Left == null)
            {
                Condition.Left = exp;
            }
            else
            {
                Condition.Right = exp;
            }
            return this;
        }
        public OpConditionBuilder<T> EqualColumn(string name1, string name2)
        {
            Expression exp;

            exp = Expression.Equal(
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name1)
                ),
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name2)
                )
            );

            if(Condition.Left == null)
            {
                Condition.Left = exp;
            }
            else
            {
                Condition.Right = exp;
            }
            return this;
        }
        public OpConditionBuilder<T> GreaterThanOrEqualColumn(string name1, string name2)
        {
            Expression exp;

            exp = Expression.GreaterThanOrEqual(
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name1)
                ),
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name2)
                )
            );

            if(Condition.Left == null)
            {
                Condition.Left = exp;
            }
            else
            {
                Condition.Right = exp;
            }
            return this;
        }
        public OpConditionBuilder<T> GreaterThanColumn(string name1, string name2)
        {
            Expression exp;

            exp = Expression.GreaterThan(
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name1)
                ),
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name2)
                )
            );

            if(Condition.Left == null)
            {
                Condition.Left = exp;
            }
            else
            {
                Condition.Right = exp;
            }
            return this;
        }
        public OpConditionBuilder<T> LessThanColumn(string name1, string name2)
        {
            Expression exp;

            exp = Expression.LessThan(
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name1)
                ),
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name2)
                )
            );

            if(Condition.Left == null)
            {
                Condition.Left = exp;
            }
            else
            {
                Condition.Right = exp;
            }
            return this;
        }
        public OpConditionBuilder<T> LessThanOrEqualColumn(string name1, string name2)
        {
            Expression exp;

            exp = Expression.LessThanOrEqual(
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name1)
                ),
                Expression.MakeMemberAccess(
                    Condition.Obj,
                    typeof(T).GetProperty(name2)
                )
            );

            if(Condition.Left == null)
            {
                Condition.Left = exp;
            }
            else
            {
                Condition.Right = exp;
            }
            return this;
        }
        public static implicit operator Expression(OpConditionBuilder<T> condition)
        {
            //condition.Condition.Exp = condition.Condition.Left;

            return condition.Condition.Exp;
        }
    }
    public class OpLogicConditionBuilder<T> : OpConditionBuilder<T>
        where T : new()
    {
        public OpLogicConditionBuilder() : base() { }
        public OpLogicConditionBuilder(ConditionObject<T> condition) : base(condition) {}
        public OpLogicConditionBuilder<T> And(ConditionObject<T> left, ConditionObject<T> right, bool pass = true)
        {
            if (!pass) return this;

            if (left.Exp == null || right.Exp == null)
            {
                if (left.Exp != null)
                    Condition.Left = left.Exp;
                if (right.Exp != null)
                    Condition.Left = right.Exp;
                return this;
            }
            Condition.Left = Expression.AndAlso(
                Condition.Left = left.Exp,
                Condition.Right = right.Exp
            );

            Condition.Right = null;

            return this;
        }
        public OpLogicConditionBuilder<T> Or(ConditionObject<T> left, ConditionObject<T> right, bool pass = true)
        {
            if (!pass) return this;

            if (left.Exp == null || right.Exp == null)
            {
                if (left.Exp != null)
                    Condition.Left = left.Exp;
                if (right.Exp != null)
                    Condition.Left = right.Exp;
                return this;
            }
            Condition.Left = Expression.OrElse(
                Condition.Left = left.Exp,
                Condition.Right = right.Exp
            );

            Condition.Right = null;

            return this;
        }
        public OpLogicConditionBuilder<T> AndEqual(string name, object value, bool pass = true)
        {
            if (!pass) return this;

            // 第一個條件
            if(Condition.Left == null)
            {
                Equal(name, value);
            }
            else
            {
                Equal(name, value);

                Condition.Left = Expression.AndAlso(
                    Condition.Left,
                    Condition.Right
                );
            }

            Condition.Right = null;

            return this;
        }
        public OpLogicConditionBuilder<T> AndGreaterThan(string name, object value, bool pass = true)
        {
            if (!pass) return this;

            // 第一個條件
            if(Condition.Left == null)
            {
                GreaterThan(name, value);
            }
            else
            {
                GreaterThan(name, value);

                Condition.Left = Expression.AndAlso(
                    Condition.Left,
                    Condition.Right
                );
            }

            Condition.Right = null;

            return this;
        }
        public OpLogicConditionBuilder<T> AndGreaterThanOrEqual(string name, object value, bool pass = true)
        {
            if (!pass) return this;

            // 第一個條件
            if(Condition.Left == null)
            {
                GreaterThanOrEqual(name, value);
            }
            else
            {
                GreaterThanOrEqual(name, value);

                Condition.Left = Expression.AndAlso(
                    Condition.Left,
                    Condition.Right
                );
            }

            Condition.Right = null;

            return this;
        }
        public OpLogicConditionBuilder<T> AndLessThan(string name, object value, bool pass = true)
        {
            if (!pass) return this;

            // 第一個條件
            if(Condition.Left == null)
            {
                LessThan(name, value);
            }
            else
            {
                LessThan(name, value);

                Condition.Left = Expression.AndAlso(
                    Condition.Left,
                    Condition.Right
                );
            }

            Condition.Right = null;

            return this;
        }
        public OpLogicConditionBuilder<T> AndLessThanOrEqual(string name, object value, bool pass = true)
        {
            if (!pass) return this;

            // 第一個條件
            if(Condition.Left == null)
            {
                LessThanOrEqual(name, value);
            }
            else
            {
                LessThanOrEqual(name, value);

                Condition.Left = Expression.AndAlso(
                    Condition.Left,
                    Condition.Right
                );
            }

            Condition.Right = null;

            return this;
        }
        public OpLogicConditionBuilder<T> And(string name, object value, bool pass = true)
        {
            if (!pass) return this;

            // 第一個條件
            if(Condition.Left == null)
            {
                Equal(name, value);
            }
            else
            {
                Equal(name, value);

                Condition.Left = Expression.AndAlso(
                    Condition.Left,
                    Condition.Right
                );
            }

            Condition.Right = null;

            return this;
        }
        public OpLogicConditionBuilder<T> AndEqualColumn(string name1, string name2, bool pass = true)
        {
            if (!pass) return this;

            // 第一個條件
            if(Condition.Left == null)
            {
                EqualColumn(name1, name2);
            }
            else
            {
                EqualColumn(name1, name2);

                Condition.Left = Expression.AndAlso(
                    Condition.Left,
                    Condition.Right
                );
            }

            Condition.Right = null;

            return this;
        }
        public OpLogicConditionBuilder<T> AndLessThanColumn(string name1, string name2, bool pass = true)
        {
            if (!pass) return this;

            // 第一個條件
            if(Condition.Left == null)
            {
                LessThan(name1, name2);
            }
            else
            {
                LessThan(name1, name2);

                Condition.Left = Expression.AndAlso(
                    Condition.Left,
                    Condition.Right
                );
            }

            Condition.Right = null;

            return this;
        }
        public OpLogicConditionBuilder<T> AndLessThanOrEqualColumn(string name1, string name2, bool pass = true)
        {
            if (!pass) return this;

            // 第一個條件
            if(Condition.Left == null)
            {
                LessThanOrEqual(name1, name2);
            }
            else
            {
                LessThanOrEqual(name1, name2);

                Condition.Left = Expression.AndAlso(
                    Condition.Left,
                    Condition.Right
                );
            }

            Condition.Right = null;

            return this;
        }
        public OpLogicConditionBuilder<T> AndGreaterThanColumn(string name1, string name2, bool pass = true)
        {
            if (!pass) return this;

            // 第一個條件
            if(Condition.Left == null)
            {
                GreaterThan(name1, name2);
            }
            else
            {
                GreaterThan(name1, name2);

                Condition.Left = Expression.AndAlso(
                    Condition.Left,
                    Condition.Right
                );
            }

            Condition.Right = null;

            return this;
        }
        public OpLogicConditionBuilder<T> AndGreaterThanOrEualColumn(string name1, string name2, bool pass = true)
        {
            if (!pass) return this;

            // 第一個條件
            if(Condition.Left == null)
            {
                GreaterThanOrEqual(name1, name2);
            }
            else
            {
                GreaterThanOrEqual(name1, name2);

                Condition.Left = Expression.AndAlso(
                    Condition.Left,
                    Condition.Right
                );
            }

            Condition.Right = null;

            return this;
        }
        public OpConditionBuilder<T> OrEqual(string name, object value, bool pass = true)
        {
            if (!pass) return this;

            // 第一個條件
            if(Condition.Left == null)
            {
                Equal(name, value);
            }
            else
            {
                Equal(name, value);

                Condition.Left = Expression.AndAlso(
                    Condition.Left,
                    Condition.Right
                );
            }

            Condition.Right = null;

            return this;
        }
        public static implicit operator ConditionObject<T>(OpLogicConditionBuilder<T> condition)
        {
            condition.Condition.Exp = condition.Condition.Left;

            return condition.Condition;
        }
    }


}
