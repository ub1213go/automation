using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.MyAutofac
{
    public class Autofac
    {
        Dictionary<Type, Type> Dict
            = new Dictionary<Type, Type>();

        public T Get<T>() where T : class
        {
            Dict.TryGetValue(typeof(T), out var type);

            if (type == null) throw new Exception($"{typeof(T).Name} 尚未註冊");

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

                Dict.TryGetValue(para.ParameterType, out var para_type);

                if (para_type == null) throw new Exception($"{para.ParameterType.Name} 尚未註冊");

                props[i] = GetType()
                    .GetMethod("Get")?
                    .MakeGenericMethod(new Type[]{ para.ParameterType })
                    .Invoke(this, null);

            }
            return (T)cont.Invoke(props);
        }

        public void Register<T>(Type type)
        {
            Dict.Add(typeof(T), type);
        }

        public void Register(Type type)
        {
            Dict.Add(type, type);
        }
    }
}
