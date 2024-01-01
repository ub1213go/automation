using AConsole.Model.ConsoleUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
    public interface IObservable<T>
    {
        void Subscription(IObserver<T> observer, ConsoleKey key);
        void Notify(ConsoleKey key);
    }
    public interface IObserver<T>
    {
        void Update(T obj);
    }
}
