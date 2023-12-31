using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model.ConsoleUI
{
    public class ConsoleView : IConsoleView
    {
        public Rectangle ViewRect;
        public IRenderable<string> Cursor;
        public int Position = 0;
        public int Length => ViewRect.Height;

        public ConsoleView(IRenderable<string> cursor, Rectangle rect)
        {
            Cursor = cursor;
            ViewRect = rect;
        }

        public void Clear()
        {
            Cursor.Clear(ViewRect);
            Position = 0;
        }

        public void Write(string str)
        {
            Cursor.Render(ViewRect.Bais(0, Position, 0, 0), str, 1);
            if (Position < ViewRect.RightBottom.Y)
                Position++;
        }

        public int Width => ViewRect.RightBottom.X - ViewRect.LeftTop.X;
        public int Height => ViewRect.RightBottom.Y - ViewRect.LeftTop.Y;
    }

}
