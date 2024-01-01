using Accessibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model.ConsoleUI
{
    public class ConsolePage : IConsolePage, IVisitor
    {
        private ConsoleView View;
        public IRenderable<string> Cursor;
        public int Position = 0;
        public Rectangle ViewRect { get; set; }
        public ConsolePage(IRenderable<string> cursor, Rectangle? viewRect = null)
        {
            Cursor = cursor;
            if(viewRect == null) 
            {
                ViewRect = new Rectangle(
                    0,
                    0,
                    Console.WindowWidth,
                    Console.WindowHeight / 2
                );
            }
            else ViewRect = viewRect;

            View = new ConsoleView(Cursor, ViewRect);
        }

        public void Render(ConsoleMenu menu)
        {
            for (int i = 0; i < menu.Length; i++)
            {
                Position = menu.Position / ViewRect.Height;

                if(Position * ViewRect.Height <= i && i < (Position + 1) * ViewRect.Height)
                {
                    ConsoleColor? color = null;
                    if (menu.Position == i)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    View.Write($"{i + 1}. {LimitLine(menu[i])}");

                    if (menu.Position == i)
                        Console.ResetColor();
                }
            }
        }

        public void Clear()
        {
            View.Clear();
        }

        private string LimitLine(string line)
        {
            var limit = Console.WindowWidth - 4;
            var real = 0;
            for(; real < line.Length && limit >= 0; real++)
            {
                limit -= char.GetUnicodeCategory(line[real]) == System.Globalization.UnicodeCategory.OtherLetter ? 2 : 1;
            }

            if (limit < 0) real -= 1;
            limit = real;

            if(line.Length > limit)
            {
                return line.Substring(0, limit);
            }
            return line;
        }

        public void Visit(IElement ele)
        {
            ele.Accept(this);
        }
    }

}
