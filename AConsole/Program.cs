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

