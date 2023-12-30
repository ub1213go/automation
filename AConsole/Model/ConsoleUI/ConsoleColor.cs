using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model.ConsoleUI
{
    public class ConsoleColor : IDisposable
    {
        //文本顏色：
        //\u001b[30m: 黑色
        //\u001b[31m: 紅色
        //\u001b[32m: 綠色
        //\u001b[33m: 黃色
        //\u001b[34m: 藍色
        //\u001b[35m: 紫色
        //\u001b[36m: 青色
        //\u001b[37m: 白色
        //背景顏色：
        //\u001b[40m: 黑色
        //\u001b[41m: 紅色
        //\u001b[42m: 綠色
        //\u001b[43m: 黃色
        //\u001b[44m: 藍色
        //\u001b[45m: 紫色
        //\u001b[46m: 青色
        //\u001b[47m: 白色
        //樣式：
        //\u001b[0m: 重置（取消所有樣式和顏色）
        //\u001b[1m: 粗體（加粗）
        //\u001b[4m: 下劃線
        //\u001b[7m: 反白
        public enum EColor
        {
            /// <summary>
            /// 反白
            /// </summary>
            AntiWhite
        }

        public ConsoleColor(EColor color)
        {
            switch (color)
            {
                case EColor.AntiWhite:
                    Console.Write("\u001b[7m");
                    break;
            }
        }

        public void Dispose()
        {
            Console.Write("\u001b[0m");
        }
    }

}
