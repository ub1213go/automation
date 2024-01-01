using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
    public abstract class KeyInteractive<T> : IObservable<T>
    {
        protected int loopLimit = 1000;
        protected HashSet<IObserver<T>> Subscribers
            = new HashSet<IObserver<T>>();
        protected Dictionary<ConsoleKey, IObserver<T>> SpecifyNotify
            = new Dictionary<ConsoleKey, IObserver<T>>();
        public bool Done { get; set; }
        public static StringBuilder Buffer {  get; set; } 
            = new StringBuilder();
        public void Subscription(IObserver<T> observer, ConsoleKey key)
        {
            Subscribers.Add(observer);

            if(!SpecifyNotify.ContainsKey(key))
            {
                SpecifyNotify.Add(key, observer);
            }
            else
            {
                SpecifyNotify[key] = observer;
            }
        }

        public abstract void Notify(ConsoleKey key);

        public IEnumerable<int> Run()
        {
            for (int i = 0; i < loopLimit && !Done; i++)
            {
                yield return i;
                Notify(Console.ReadKey(true).Key);
            }
        }
    }
}
