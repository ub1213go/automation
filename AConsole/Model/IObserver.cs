using AConsole.Model.ConsoleUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
    public interface IObservable
    {
        void Subscription(IObserver observer, ConsoleKey key);
        void Notify(ConsoleKey key);
    }
    public interface IObserver
    {
        void Update(object obj);
    }
}
