using AConsole.Model.ConsoleUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
    public class KeyEvent<T> : IObserver<T>
    {
        private Action<T> action { get; set; }
        public KeyEvent(Action<T> action)
        {
            this.action = action;
        }
        public void Update(T obj)
        {
            action(obj);
        }
    }
}
