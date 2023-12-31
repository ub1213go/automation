using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
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
