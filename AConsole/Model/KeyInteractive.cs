using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
    public class KeyIntractive : IObservable
    {
        private int loopLimit = 1000;
        private HashSet<IObserver> Subscribers
            = new HashSet<IObserver>();
        private Dictionary<ConsoleKey, IObserver> SpecifyNotify
            = new Dictionary<ConsoleKey, IObserver>();
        public bool Done { get; set; }
        public void Subscription(IObserver observer, ConsoleKey key)
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
        public void Notify(ConsoleKey key)
        {
            if (SpecifyNotify.TryGetValue(key, out var val))
            {
                val.Update();
            }
        }
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
