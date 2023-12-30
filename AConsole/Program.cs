using AConsole.MyAutofac;
using System.Dynamic;
using System.Collections;
using Automation;
using AConsole.Model;
using System.Drawing;
using AConsole.Model.ConsoleUI;

namespace AConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            var auto = new Autofac();
            auto.Register<IRenderable<string>>(typeof(ConsoleCursor));
            auto.Register(typeof(ConsolePage));
            auto.Register(typeof(ConsoleMenu));
            auto.Register(typeof(AutoService));

            var menu = auto.Get<ConsoleMenu>();
            var srv = auto.Get<AutoService>();

            foreach(var t in srv.GetAllWindowTitle())
            {
                menu.SetMenu(t);
            }

            // todo: 建立可換頁，可選擇的 menu
            var page = auto.Get<ConsolePage>();

            var page_hint = auto.Get<ConsolePage>();
            var menu_hint = auto.Get<ConsoleMenu>();
            menu_hint.SetMenu("A: 測試")
                .SetMenu("B: 測試");
            page_hint.Render(menu_hint);

            //while (true)
            //{
            //    page.Clear();
            //    page.Render(menu);
            //    var key = Console.ReadKey().Key;
            //    switch (key)
            //    {
            //        case ConsoleKey.UpArrow:
            //            menu.Position--;
            //            break;
            //        case ConsoleKey.DownArrow:
            //            menu.Position++;
            //            break;
            //        case ConsoleKey.Enter:
            //            break;
            //    }
            //}

            Console.ReadKey();


            //var service = new AutoService();

            //var count = 0;
            //foreach (var title in service.GetAllWindowTitle())
            //{
            //    view.Write($"{++count}. {title}");
            //}

            //var win = service.GetWindow("訊息公告");

            //var children = win.AutomationElement.FindAllChildren();
            //count = 0;
            //foreach(var child in children)
            //{
            //    if(count == 0) view.Clear();
            //    var text = child.ToString();
            //    text = text.Substring(0, text.Length > 100 ? 100 : text.Length);
            //    view.Write($"{++count}. {text}");
            //}
        }
    }
}

