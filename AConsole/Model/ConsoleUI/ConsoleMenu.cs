using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model.ConsoleUI
{
    public class ConsoleMenu : KeyInteractive<ConsoleMenu>, IEnumerable<Tuple<string, string>>
    {
        protected List<Tuple<string, string>> Menus { get; set; }
            = new List<Tuple<string, string>>();
        protected int _Position;

        public int Length => Menus.Count;
        public Tuple<string, string> Current => Menus[Position];
        public int Position
        {
            get
            {
                return _Position;
            }
            set
            {
                if (0 <= value && value < Length)
                {
                    _Position = value;
                }
                else if(Length <= value)
                {
                    _Position = Length - 1;
                }
                else if(value < 0)
                {
                    _Position = 0;
                }
            }
        }

        public Tuple<string, string> this[int index]
        {
            get
            {
                return Menus[index];
            }
            set
            {
                Menus[index] = value;
            }
        }

        public void SetMenu(Tuple<string, string> menu)
        {
            Menus.Add(menu);
        }
        
        public void Clear()
        {
            Position = 0;
            Menus.Clear();
        }

        public IEnumerator<Tuple<string, string>> GetEnumerator()
        {
            return Menus.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override void Notify(ConsoleKey key)
        {
            if (SpecifyNotify.TryGetValue(key, out var val))
            {
                val.Update(this);
            }
        }
    }

}
