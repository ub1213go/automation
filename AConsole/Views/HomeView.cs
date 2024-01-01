using AConsole.Model;
using AConsole.Model.ConsoleUI;
using AConsole.MyAutofac;
using Automation;
using ImpromptuInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Views
{
    public class HomeView : BaseView
    {
        public HomeView(RoutesEntry routesEntry) : base(routesEntry)
        {
            Route = new ConsoleRoute("Home", "首頁");
            Route.Hint.SetHint("Q: 離開");
            Route.Hint.SetHint("J: 下一個");
            Route.Hint.SetHint("K: 上一個");
            Route.Hint.SetHint("Enter: 進入頁面");
            foreach (var d in RoutesEntry.DefaultKeyEvents)
            {
                Route.Subscription(d.Value, d.Key);
            }
            Route.Menu.Subscription(new KeyEvent<ConsoleMenu>(e =>
            {
                RoutesEntry.EnterTo(e.Current.Item1);
            }), ConsoleKey.Enter);
        }
    }
}
