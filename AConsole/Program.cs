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
            var count = 0;

            hint.SetHint("Q: 離開");
            hint.SetHint("J: 下一個");
            hint.SetHint("K: 上一個");
            hint.SetHint("M: 紅框標記");
            hint.SetHint("Enter: 下一層");
            hint.SetHint("ESC: 上一層");
            hint.SetHint("F: 下一頁");
            hint.SetHint("B: 下一頁");
            hint.SetHint("A: 動作");
            AutoUI? autoUI = null;
            RefreshMenu(menu, ref autoUI);
            while (true)
            {
                page.Clear();
                page.Render(menu);
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Q:
                        goto Done;
                    case ConsoleKey.J:
                        menu.Position++;
                        break;
                    case ConsoleKey.K:
                        menu.Position--;
                        break;
                    case ConsoleKey.Enter:
                        // control == null -> 在桌面
                        if (autoUI == null)
                        {
                            autoUI = service.GetWindow(menu.Menus[menu.Position]);
                        }
                        // control != null -> 視窗
                        else
                        {
                            autoUI = service
                                .GetControlByParent(autoUI, menu.Position);
                        }
                        RefreshMenu(menu, ref autoUI);
                        break;
                    case ConsoleKey.Escape:
                        if(autoUI != null)
                        {
                            autoUI = new AutoUIWindow(
                                service.Core,
                                autoUI.AutomationElement?.Parent
                            );

                        }
                        RefreshMenu(menu, ref autoUI);
                        break;
                    case ConsoleKey.M:
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
                                draw = new Draw(
                                    service.GetWindow(
                                        menu.Current
                                    )
                                );
                                draw.Start();
                            }
                            else
                            {
                                draw = new Draw(
                                    service.GetControlByParent(
                                        autoUI,
                                        menu.Position
                                    )
                                );
                                draw.Start();
                            }
                        }
                        break;
                    case ConsoleKey.T:
                        AllObject(auto);
                        break;
                }

            }

        Done: return;
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
            menu.Position = 0;
            menu.Menus.Clear();
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

