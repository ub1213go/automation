using System;
using System.Collections;
using System.Collections.Generic;
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

        public ConsoleHint(ConsoleCursor cursor, ConsoleView view, int parts) 
        {
            Views = new ConsoleView[parts];
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

        private Rectangle GetPartRectangle(ConsoleView view, int idx)
        {
            var rect = new Rectangle(
                view.ViewRect.LeftTop.X + (view.Width / Views.Length) * idx,
                view.ViewRect.LeftTop.Y ,
                view.ViewRect.LeftTop.X + (view.Width / Views.Length) * (idx + 1),
                view.ViewRect.RightBottom.Y
            );
            return rect;
        }

        public IEnumerator<string> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

}
