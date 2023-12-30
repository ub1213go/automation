using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
    public interface IObservable<T>
    {
        void Subscription(IObserver<T> observer);
    }

    public interface IObserver<T>
    {
        void Notify(T t);
    }
}
