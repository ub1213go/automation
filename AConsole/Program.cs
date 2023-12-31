using AConsole.MyAutofac;
using System.Dynamic;
using System.Collections;
using Automation;
using AConsole.Model;
using System.Drawing;
using AConsole.Model.ConsoleUI;
using Automation.Draw;

namespace AConsole
{
    public class Program
    {
        public static AutoService service = new AutoService();
        public static Draw? draw = null;
        static void Main(string[] args)
        {
            Console.ResetColor();

            var auto = new Autofac();
            auto.Register<IRenderable<string>>(typeof(ConsoleCursor));
            auto.Register(typeof(ConsolePage));
            auto.Register(typeof(ConsoleMenu));
            auto.Register(typeof(AutoService));
            auto.Single().Register(typeof(ConsoleCursor));

            auto.Register(typeof(ConsoleHint))
                .WithParameter("hint", new object[]{
                    auto.Get<ConsoleCursor>(),
                    new ConsoleView(auto.Get<ConsoleCursor>(), new Model.Rectangle(
                        0,
                        Console.WindowHeight / 6 * 5,
                        Console.WindowWidth,
                        Console.WindowHeight
                    )),
                    6,
                    2
                });

            var hint = auto.Get<ConsoleHint>("hint");
            var menu = auto.Get<ConsoleMenu>();
            var page = auto.Get<ConsolePage>();

            hint.SetHint("Q: 離開");
            hint.SetHint("J: 下一個");
            hint.SetHint("K: 上一個");
            hint.SetHint("M: 紅框標記");
            hint.SetHint("Enter: 下一層");
            hint.SetHint("ESC: 上一層");
            hint.SetHint("R: 記錄此動作");
            hint.SetHint("F: 取得焦點");
            hint.SetHint("C: 點擊");
            hint.SetHint("I: 輸入");

            AutoUI? autoUI = null;

            menu.Subscription(new KeyEvent(() =>
            {
                menu.Done = true;
            }), ConsoleKey.Q);
            menu.Subscription(new KeyEvent(() =>
            {
                menu.Position++;
            }), ConsoleKey.J);
            menu.Subscription(new KeyEvent(() =>
            {
                menu.Position--;
            }), ConsoleKey.K);
            menu.Subscription(new KeyEvent(() =>
            {
                if (autoUI == null)
                    autoUI = service.GetWindow(menu.Current);
                else
                    autoUI = service.GetControlByParent(autoUI, menu.Position);
                RefreshMenu(menu, ref autoUI);
            }), ConsoleKey.Enter);
            menu.Subscription(new KeyEvent(() =>
            {
                if (autoUI != null)
                    autoUI = new AutoUIWindow(service.Core, autoUI.AutomationElement?.Parent);
                RefreshMenu(menu, ref autoUI);
            }), ConsoleKey.Escape);
            menu.Subscription(new KeyEvent(() =>
            {
                if (draw != null)
                {
                    draw.Stop();
                    draw = null;
                }
                else
                {
                    // 紅框框起光標指到的項目
                    if (autoUI == null)
                    {
                        draw = new Draw(service.GetWindow(menu.Current));
                        draw.Start();
                    }
                    else
                    {
                        draw = new Draw(service.GetControlByParent(autoUI, menu.Position));
                        draw.Start();
                    }
                }
            }), ConsoleKey.M);
            menu.Subscription(new KeyEvent(() =>
            {
                AllObject(auto);
            }), ConsoleKey.T);
            menu.Subscription(new KeyEvent(() =>
            {
                menu.Notify(ConsoleKey.Enter);
                try
                {
                    autoUI.AutomationElement.Focus();
                }
                catch
                {

                }
            }), ConsoleKey.F);

            RefreshMenu(menu, ref autoUI);
            foreach (var count in menu.Run())
            {
                page.Clear();
                page.Render(menu);
            }
        }


        static void AllObject(Autofac auto)
        {
            foreach (var obj in auto.ObjectDict)
            {
                for (int i = 0; i < obj.Value.Count; i++)
                {
                    var o = obj.Value[i];

                    Console.WriteLine($"{i + 1}. " + o.GetType().Name);
                }
            }
        }

        static void RefreshMenu(ConsoleMenu menu, ref AutoUI? control)
        {
            menu.Clear();
            // 檢查是否為桌面，或者第一次讀取
            if(control == null || control.AutomationElement == null || control.AutomationElement.Parent == null)
            {
                control = null;
                foreach (var title in service.GetAllWindowTitle())
                {
                    menu.SetMenu($"{title}");
                }
            }
            else
            {
                foreach(var c in service.GetControlListByParent(control))
                {
                    menu.SetMenu(c);
                }
            }
        }
    }

}

