using AConsole.MyAutofac;
using System.Dynamic;
using System.Collections;
using Automation;
using AConsole.Model;
using System.Drawing;
using AConsole.Model.ConsoleUI;
using Automation.Draw;
using System.Text;
using System.Diagnostics;
using Interop.UIAutomationClient;
using ImpromptuInterface;
using AConsole.Views;

namespace AConsole
{
    public class Program
    {
        public static AutoService service = new AutoService();
        public static Draw? draw = null;
        static void Main(string[] args)
        {
            Autofac.auto.Single().Register<IRenderable<string>>(typeof(ConsoleCursor));
            Autofac.auto.Register(typeof(ConsolePage));
            Autofac.auto.Register(typeof(ConsoleMenu));
            Autofac.auto.Register(typeof(AutoService));
            Autofac.auto.Single().Register(typeof(RoutesEntry));
            Autofac.auto.Register(typeof(HomeView));
            Autofac.auto.Register(typeof(EditView));
            Autofac.auto.Register(typeof(LoadView));
            Autofac.auto.Single().Register(typeof(ConsoleCursor));
            Autofac.auto.Register(typeof(ConsoleHint))
                .WithParameter("hint", new object[]{
                    Autofac.auto.Get<ConsoleCursor>(),
                    new ConsoleView(Autofac.auto.Get<ConsoleCursor>(), new Model.Rectangle(
                        0,
                        Console.WindowHeight / 6 * 5,
                        Console.WindowWidth,
                        Console.WindowHeight
                    )),
                    6,
                    2
                });

            var rv = Autofac.auto.Get<RoutesEntry>();

            rv.EnterTo("Home");

            //var cursor = new ConsoleCursor();
            //var page = new ConsolePage(cursor);
            //var edit = new ConsoleRoute("Edit", "編輯");
            //var load = new ConsoleRoute("Load", "讀取");
            //var root = new ConsoleRoute("Home", "首頁");


            //root.Add(load);
            //root.Add(edit);
            //root.Add(new ConsoleRoute("Test", "測試"));
            //root.Add(new ConsoleRoute("Test", "測試"));
            //root.Add(new ConsoleRoute("Test", "測試"));
            //root.Add(new ConsoleRoute("Test", "測試"));
            //root.Add(new ConsoleRoute("Test", "測試"));
            //root.Add(new ConsoleRoute("Test", "測試"));
            //root.Add(new ConsoleRoute("Test", "測試"));
            //foreach(var d in defKeyEvents)
            //{
            //    root.Subscription(d.Value, d.Key);
            //}
            //root.Hint.SetHint("Q: 離開");
            //page.Visit(root);


            //var hint = auto.Get<ConsoleHint>("hint");
            //var menu = auto.Get<ConsoleMenu>();
            //var page = auto.Get<ConsolePage>();

            //hint.SetHint("Q: 離開");
            //hint.SetHint("J: 下一個");
            //hint.SetHint("K: 上一個");
            //hint.SetHint("M: 紅框標記");
            //hint.SetHint("Enter: 下一層");
            //hint.SetHint("ESC: 上一層");
            //hint.SetHint("R: 記錄此動作");
            //hint.SetHint("F: 取得焦點");
            //hint.SetHint("C: 點擊");
            //hint.SetHint("I: 輸入");
            //hint.SetHint("G: 5秒後取得焦點視窗");

            //AutoUI? autoUI = null;


            //RefreshMenu(menu, ref autoUI);
            //foreach (var count in menu.Run())
            //{
            //    page.Clear();
            //    page.Render(menu);
            //}
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

    }

}

