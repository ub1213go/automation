using Dynamitey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.MyAutofac
{
    public class Autofac
    {
        internal protected Dictionary<Type, Type> Dict
            = new Dictionary<Type, Type>();

        internal protected Dictionary<Type, object> List
            = new Dictionary<Type, object>();

        internal protected IAutoState State;

        public virtual T Get<T>() where T : class
        {
            return State.Get<T>(this);
        }

        public Autofac Register<T>(Type type)
        {
            Dict.Add(typeof(T), type);

            return this;
        }

        public Autofac Register(Type type)
        {
            Dict.Add(type, type);

            return this;
        }
    }

    public interface IAutoState
    {
        T Get<T>(Autofac auto);
    }

    public abstract class AutofacTemplate : IAutoState
    {
        protected T Base<T>(Autofac auto)
        {
            auto.Dict.TryGetValue(typeof(T), out var type);

            if (type == null) throw new Exception($"{typeof(T).Name} 尚未註冊");

            // 取第一個建構子
            var cont = type.GetConstructors()[0];
            var cont_params = cont.GetParameters();
            var props = new object?[cont_params.Length];
            for(int i = 0; i < cont_params.Length; i++)
            {
                var para = cont_params[i];
                if (para.HasDefaultValue)
                {
                    props[i] = para.DefaultValue;
                    continue;
                }

                auto.Dict.TryGetValue(para.ParameterType, out var para_type);

                if (para_type == null) throw new Exception($"{para.ParameterType.Name} 尚未註冊");

                props[i] = GetType()
                    .GetMethod("Get")?
                    .MakeGenericMethod(new Type[]{ para.ParameterType })
                    .Invoke(this, null);

            }
            return (T)cont.Invoke(props);
        } 

        protected virtual T Final<T>(T t)
        {
            return t;
        }

        public T Get<T>(Autofac auto)
        {
        }
    }

    public class AutofacMultiple : AutofacTemplate
    {
        public T Get<T>(Autofac auto)
        {
            auto.Dict.TryGetValue(typeof(T), out var type);

            if (type == null) throw new Exception($"{typeof(T).Name} 尚未註冊");

            // 取第一個建構子
            var cont = type.GetConstructors()[0];
            var cont_params = cont.GetParameters();
            var props = new object?[cont_params.Length];
            for(int i = 0; i < cont_params.Length; i++)
            {
                var para = cont_params[i];
                if (para.HasDefaultValue)
                {
                    props[i] = para.DefaultValue;
                    continue;
                }

                auto.Dict.TryGetValue(para.ParameterType, out var para_type);

                if (para_type == null) throw new Exception($"{para.ParameterType.Name} 尚未註冊");

                props[i] = GetType()
                    .GetMethod("Get")?
                    .MakeGenericMethod(new Type[]{ para.ParameterType })
                    .Invoke(this, null);

            }
            return (T)cont.Invoke(props);
        }
    }

    public class AutofacSingle : IAutoState
    {
        public T Get<T>(Autofac auto)
        {
            auto.Dict.TryGetValue(typeof(T), out var type);

            if (type == null) throw new Exception($"{typeof(T).Name} 尚未註冊");

            // 取第一個建構子
            var cont = type.GetConstructors()[0];
            var cont_params = cont.GetParameters();
            var props = new object?[cont_params.Length];
            for(int i = 0; i < cont_params.Length; i++)
            {
                var para = cont_params[i];
                if (para.HasDefaultValue)
                {
                    props[i] = para.DefaultValue;
                    continue;
                }

                auto.Dict.TryGetValue(para.ParameterType, out var para_type);

                if (para_type == null) throw new Exception($"{para.ParameterType.Name} 尚未註冊");

                props[i] = GetType()
                    .GetMethod("Get")?
                    .MakeGenericMethod(new Type[]{ para.ParameterType })
                    .Invoke(this, null);

            }

            return (T)cont.Invoke(props);
        }
    }
}
