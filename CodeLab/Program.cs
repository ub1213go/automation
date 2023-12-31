
namespace Program
{
    public class Program
    {
        static void Main(string[] args)
        {
            var k = new Menu();

            var e1 = new KeyEvent(() =>
            {
                Console.WriteLine("hi");
            });
            var e2 = new KeyEvent(() =>
            {
                Console.WriteLine("hello");
            });
            var e3 = new KeyEvent(() =>
            {
                Console.WriteLine("apple");
            });
            var e4 = new KeyEvent(() =>
            {
                Console.WriteLine("banana");
            });

            k.Subscription(e1, ConsoleKey.A);
            k.Subscription(e2, ConsoleKey.B);
            k.Subscription(e3, ConsoleKey.C);
            k.Subscription(e4, ConsoleKey.D);

            foreach(var count in k.Run())
            {
                Console.WriteLine(count);
            }
        }
    }

    public interface IObservable
    {
        void Subscription(IObserver observer, ConsoleKey key);
        void Notify(ConsoleKey key);
    }
    public interface IObserver
    {
        void Update();
    }

    public class Menu : IObservable
    {
        private int loopLimit = 100;
        private HashSet<IObserver> Subscribers
            = new HashSet<IObserver>();
        private Dictionary<ConsoleKey, IObserver> SpecifyNotify
            = new Dictionary<ConsoleKey, IObserver>();
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
            for (int i = 0; i < loopLimit; i++)
            {
                Notify(Console.ReadKey(true).Key);
                yield return i;
            }
        }
    }
    public class KeyEvent : IObserver
    {
        private Action action { get; set; }
        public KeyEvent(Action action)
        {
            this.action = action;
        }
        public void Update()
        {
            action();
        }
    }
}






