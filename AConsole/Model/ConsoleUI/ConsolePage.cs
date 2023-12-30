using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model.ConsoleUI
{
    public class ConsolePage : IConsolePage
    {
        public List<ConsoleView> Views
            = new List<ConsoleView>();
        public IRenderable<string> Cursor;
        public int Position = 0;

        public ConsolePage(IRenderable<string> cursor)
        {
            Cursor = cursor;
            Views.Add(new ConsoleView(cursor));
        }

        public void Clear()
        {
            Views[Position].Clear();
        }

        public void Render(ConsoleMenu menu)
        {
            for (int i = 0; i < Views[Position].Length && i < menu.Length; i++)
            {
                ConsoleColor? color = null;
                if (menu.Position == i)
                    color = new ConsoleColor(ConsoleColor.EColor.AntiWhite);

                Views[Position].Write($"{i + 1}. {menu[i]}");

                if (menu.Position == i)
                    color?.Dispose();
            }
        }
    }

}
