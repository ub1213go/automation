using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model.ConsoleUI
{
    public class ConsoleCursor : IRenderable<string>
    {
        public void Clear(Rectangle rect)
        {
            Render(rect, new string(' ', rect.Width() * rect.Height()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="obj"></param>
        /// <param name="flag">0: 回原點, 1: 回終點</param>
        public void Render(Rectangle rect, string obj, int flag = 0)
        {
            var (ox, oy) = Console.GetCursorPosition();

            // 起始點
            rect.RightBottomCorner(
                new Point(
                    Console.WindowWidth,
                    Console.WindowHeight
                )
            );
            // 寬度
            var w = rect.Width();
            // 長度
            var h = rect.Height();
            var strs = obj.ToString().Split("\r\n").ToList();
            for (int i = 0; i < h && i < strs.Count; i++)
            {
                var str = strs[i];
                var subh = (int)Math.Floor((double)str.Length / w) + 1;
                for (int j = 0; j < subh; j++)
                {
                    // 每個換行點
                    var pivot = j * w;
                    // 行長
                    // 按邏輯來說這裡不會有負的
                    var lineLen = str.Length - pivot;
                    // 超過一行的長度就是等於一行的長度
                    if (lineLen > w)
                    {
                        lineLen = w;
                    }
                    // cursor set
                    Console.SetCursorPosition(
                        rect.LeftTop.X,
                        rect.LeftTop.Y + i + j
                    );
                    // write sub string
                    Console.Write(str.Substring(pivot, lineLen));
                }
                i += subh - 1;
            }
            // 光標移至最後位置
            switch (flag)
            {
                case 0:
                    Console.SetCursorPosition(ox, oy);
                    break;
                case 1:
                    Console.SetCursorPosition(
                        rect.RightBottom.X,
                        rect.RightBottom.Y
                    );
                    break;
            }
        }
    }

}
