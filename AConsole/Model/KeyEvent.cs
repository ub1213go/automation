using AConsole.Model.ConsoleUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
    public class KeyEvent : IObserver
    {
        private Action<object> action { get; set; }
        public KeyEvent(Action<object> action)
        {
            this.action = action;
        }
        public void Update(object obj)
        {
            action(obj);
        }
    }
}
