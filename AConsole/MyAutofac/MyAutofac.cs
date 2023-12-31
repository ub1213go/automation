using Dynamitey;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.MyAutofac
{
    public enum IState
    {
        Single,
        Multiple
    } 

    public class Autofac
    {
        internal protected Dictionary<Type, Tuple<Type, IState>> Dict
            = new Dictionary<Type, Tuple<Type, IState>>();
        internal protected Dictionary<Type, List<object>> ObjectDict
            = new Dictionary<Type, List<object>>();
        internal protected Dictionary<string, object[]> ObjectArgumentsDict
            = new Dictionary<string, object[]>();
        public AutofacEnter Enter => new AutofacEnter(this);
        public AutofacProvider Provider => new AutofacProvider(this);
        public AutofacEnter Register<T>(Type type) => Enter.Register<T>(type);
        public AutofacEnter Register(Type type) => Enter.Register(type);
        public AutofacEnter Single() => Enter.Single();
        public T Get<T>() where T : class => Provider.Get<T>();
        public T Get<T>(string id) where T : class => Provider.Get<T>(id);
    }

    public class AutofacEnter
    {
        private Autofac auto;
        private IState state;
        public AutofacEnter(Autofac auto)
        {
            this.auto = auto;

            state = IState.Multiple;
        }

        public AutofacEnter Single()
        {
            state = IState.Single;

            return this;
        }

        public AutofacEnter Register<T>(Type type)
        {
            if (auto.Dict.ContainsKey(typeof(T)))
            {
                auto.Dict[typeof(T)] = Tuple.Create(type, state);
            }
            else
            {
                auto.Dict.Add(typeof(T), Tuple.Create(type, state));
            }

            return this;
        }

        public AutofacEnter Register(Type type)
        {
            if (auto.Dict.ContainsKey(type))
            {
                auto.Dict[type] = Tuple.Create(type, state);
            }
            else
            {
                auto.Dict.Add(type, Tuple.Create(type, state));
            }

            return this;
        }

        public AutofacEnter WithParameter(string id, params object[] args)
        {
            auto.ObjectArgumentsDict.Add(id, args);

            return this; 
        }
    }

    public class AutofacProvider
    {
        private Autofac auto;
        public AutofacProvider(Autofac auto)
        {
            this.auto = auto;
        }

        public virtual T Get<T>() where T : class
        {
            auto.Dict.TryGetValue(typeof(T), out var definition);

            if (definition == null) throw new Exception($"{typeof(T).Name} 尚未註冊");

            object? obj = null;
            switch (definition.Item2)
            {
                case IState.Single:
                    if (auto.ObjectDict.TryGetValue(typeof(T), out var val))
                        obj = val.FirstOrDefault();
                    break;
                case IState.Multiple:
                    break;
            }

            if(obj == null)
            {
                // 取第一個建構子
                var cont = definition.Item1.GetConstructors()[0];
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
                        .GetMethods().First(p=>p.Name == "Get")
                        .MakeGenericMethod(new Type[]{ para.ParameterType })
                        .Invoke(this, null);

                }
                obj = cont.Invoke(props);

                CheckIn(obj);
            } 

            return (T)obj;
        }

        public virtual T Get<T>(string id) where T : class
        {
            auto.Dict.TryGetValue(typeof(T), out var definition);

            if (definition == null) throw new Exception($"{typeof(T).Name}-ID: {id} 尚未註冊");

            // 是否存在 id 
            object? obj = null;
            if (auto.ObjectArgumentsDict.TryGetValue(id, out var args))
            {
                // 是否有相同參數數量的建構子
                var conts = definition.Item1.GetConstructors()
                    .Where(
                        p => p.GetParameters().Length >= args.Length &&
                        args.Length >= p.GetParameters().Where(e=>!e.HasDefaultValue).Count()
                    ).ToList();
                if (conts.Count == 0) throw new Exception($"{typeof(T).Name}-ID: {id} 無相符的建構子");

                foreach(var cont in conts)
                {
                    // 是否參數的型態一致
                    var same = true;
                    var props = cont.GetParameters();
                    for(int i = 0; i < props.Length; i++)
                    {
                        var a_p = props[i];
                        var b_p = args[i];
                        var from = b_p.GetType().IsAssignableFrom(a_p.ParameterType);
                        var to = b_p.GetType().IsAssignableTo(a_p.ParameterType);

                        if(!(from || to))
                        {
                            same = false;
                        }
                    }

                    if (same == true)
                    {
                        obj = cont.Invoke(args);

                        CheckIn(obj);

                        return (T)obj;
                    };
                }

            }

            throw new Exception($"{typeof(T).Name}-ID: {id} 不存在相符的參數設定");
        }

        private void CheckIn(object obj)
        {
            if (auto.ObjectDict.ContainsKey(typeof(T)))
            {
                auto.ObjectDict[typeof(T)].Add(obj);
            }
            else
            {
                auto.ObjectDict[typeof(T)] = new List<object>() { obj };
            }
        }
    }
}
