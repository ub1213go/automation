using AConsole.Model;
using AConsole.Model.ConsoleUI;
using AConsole.MyAutofac;
using Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Views
{
    public class LoadView : BaseView
    {
        public LoadView(RoutesEntry routesEntry) : base(routesEntry)
        {
            Route = new ConsoleRoute("Load", "讀取");
            Route.Hint.SetHint("Q: 離開");
            Route.Hint.SetHint("J: 下一個");
            Route.Hint.SetHint("K: 上一個");
            foreach (var d in RoutesEntry.DefaultKeyEvents)
            {
                Route.Subscription(d.Value, d.Key);
            }
        }
    }
}
