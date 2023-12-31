using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model.ConsoleUI
{
    public class ConsoleHint : IEnumerable<string>
    {
        public ConsoleView[] Views { get; set; }
        public ConsoleView VisibleView { get; set; }

        private int Position = 0;
        private int Columns = 0;
        private int Rows = 0; 

        public ConsoleHint(ConsoleCursor cursor, ConsoleView view, int columns, int rows) 
        {
            Columns = columns;
            Rows = rows;
            Views = new ConsoleView[Columns * Rows];
            VisibleView = view;


            for(int i = 0; i < Views.Length; i++)
            {
                Views[i] = new ConsoleView(cursor, GetPartRectangle(VisibleView, i));
            }
        }

        public ConsoleHint SetHint(string cnt)
        {
            Views[Position++].Write(cnt);

            if (Position == Views.Length) Position = 0;

            return this;
        }

        public IEnumerator<string> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private Rectangle GetPartRectangle(ConsoleView view, int idx)
        {
            var rect = new Rectangle(
                view.ViewRect.LeftTop.X + (view.Width / Columns) * ToTwoDimensions(idx).X,
                view.ViewRect.LeftTop.Y + (view.Height / Rows) * ToTwoDimensions(idx).Y,
                view.ViewRect.LeftTop.X + (view.Width / Columns) * (ToTwoDimensions(idx).X + 1),
                view.ViewRect.RightBottom.Y + (view.Height / Rows) * (ToTwoDimensions(idx).Y + 1)
            );
            return rect;
        }

        private Point ToTwoDimensions(int idx)
        {
            var res = new Point(
                idx % Columns,
                idx / Columns
            );
            return res;
        }
    }

}
