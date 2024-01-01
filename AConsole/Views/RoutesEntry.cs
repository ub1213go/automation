using AConsole.Model;
using AConsole.Model.ConsoleUI;
using AConsole.MyAutofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Views
{
    public class RoutesEntry
    {
        public ConsolePage MainPage { get; set; }
            = Autofac.auto.Get<ConsolePage>();
        public Dictionary<ConsoleKey, KeyEvent<ConsoleMenu>> DefaultKeyEvents { get; set; }
        public Dictionary<string, IRouteView<ConsoleRoute>> Routes { get; set; }
            = new Dictionary<string, IRouteView<ConsoleRoute>>();
        public RoutesEntry()
        {
            DefaultKeyEvents = new Dictionary<ConsoleKey, KeyEvent<ConsoleMenu>>()
            {
                {
                    ConsoleKey.Q, new KeyEvent<ConsoleMenu>(e =>
                    {
                        e.Done = true;
                    })
                },
                {
                    ConsoleKey.J, new KeyEvent<ConsoleMenu>(e =>
                    {
                        if(int.TryParse(KeyInteractive<ConsoleMenu>.Buffer.ToString(), out var val))
                        {
                            e.Position += val;

                            KeyInteractive<ConsoleMenu>.Buffer.Clear();
                        }
                        else
                        {
                            e.Position++;
                        }
                        MainPage.Clear();
                        MainPage.Render(e);
                    })
                },
                {
                    ConsoleKey.K, new KeyEvent<ConsoleMenu>(e =>
                    {
                        if(int.TryParse(KeyInteractive<ConsoleMenu>.Buffer.ToString(), out var val))
                        {
                            e.Position -= val;

                            KeyInteractive<ConsoleMenu>.Buffer.Clear();
                        }
                        else
                        {
                            e.Position--;
                        }
                        MainPage.Clear();
                        MainPage.Render(e);
                    })
                },
                {
                    ConsoleKey.D1, new KeyEvent<ConsoleMenu>(e =>
                    {
                        KeyInteractive<ConsoleMenu>.Buffer.Append('1');
                    })
                },
                {
                    ConsoleKey.D2, new KeyEvent<ConsoleMenu>(e =>
                    {
                        KeyInteractive<ConsoleMenu>.Buffer.Append('2');
                    })
                },
                {
                    ConsoleKey.D3, new KeyEvent<ConsoleMenu>(e =>
                    {
                        KeyInteractive<ConsoleMenu>.Buffer.Append('3');
                    })
                },
                {
                    ConsoleKey.D4, new KeyEvent<ConsoleMenu>(e =>
                    {
                        KeyInteractive<ConsoleMenu>.Buffer.Append('4');
                    })
                },
                {
                    ConsoleKey.D5, new KeyEvent<ConsoleMenu>(e =>
                    {
                        KeyInteractive<ConsoleMenu>.Buffer.Append('5');
                    })
                },
                {
                    ConsoleKey.D6, new KeyEvent<ConsoleMenu>(e =>
                    {
                        KeyInteractive<ConsoleMenu>.Buffer.Append('6');
                    })
                },
                {
                    ConsoleKey.D7, new KeyEvent<ConsoleMenu>(e =>
                    {
                        KeyInteractive<ConsoleMenu>.Buffer.Append('7');
                    })
                },
                {
                    ConsoleKey.D8, new KeyEvent<ConsoleMenu>(e =>
                    {
                        KeyInteractive<ConsoleMenu>.Buffer.Append('8');
                    })
                },
                {
                    ConsoleKey.D9, new KeyEvent<ConsoleMenu>(e =>
                    {
                        KeyInteractive<ConsoleMenu>.Buffer.Append('9');
                    })
                },
            };

            //var home = Autofac.auto.Get<HomeView>();
            var home = new HomeView(this);
            var edit = new EditView(this);
            var load = new LoadView(this);

            home.Add(edit);
            home.Add(load);

            Routes.Add(home.Route.Name, home);
            Routes.Add(edit.Route.Name, edit);
            Routes.Add(load.Route.Name, load);
        }

        public void EnterTo(string name)
        {
            if(Routes.TryGetValue(name, out var val))
            {
                MainPage.Visit(val.Route);
            }
        }
    }
}
