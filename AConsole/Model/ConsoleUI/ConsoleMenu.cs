using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model.ConsoleUI
{
    public class ConsoleMenu : IEnumerable<string>
    {
        public List<string> Menus { get; set; }
            = new List<string>();

        public int Length => Menus.Count;
        public string Current => Menus[Position];

        private int _Position;

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

        public ConsoleMenu()
        {

        }

        public ConsoleMenu SetMenu(string menu)
        {
            Menus.Add(menu);

            return this;
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
