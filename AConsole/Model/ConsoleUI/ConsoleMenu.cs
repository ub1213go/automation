using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model.ConsoleUI
{
    public class ConsoleMenu : KeyIntractive, IEnumerable<string>
    {
        private List<string> Menus { get; set; }
            = new List<string>();
        private int _Position;

        public int Length => Menus.Count;
        public string Current => Menus[Position];
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
            }
        }

        public string this[int index]
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
        public void SetMenu(string menu)
        {
            Menus.Add(menu);
        }
        public void Clear()
        {
            Position = 0;
            Menus.Clear();
        }
        public IEnumerator<string> GetEnumerator()
        {
            return Menus.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
