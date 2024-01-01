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
    public abstract class BaseView : IRouteView<ConsoleRoute>
    {
        public ConsoleRoute? Route { get; set; }
        public AutoService Service { get; }
            = Autofac.auto.Get<AutoService>();
        public void Accept(IVisitor visitor) => Route?.Accept(visitor);
        protected RoutesEntry RoutesEntry { get; set; }
        public BaseView(RoutesEntry routesEntry)
        {
            RoutesEntry = routesEntry;
        }

        public IRouteView<ConsoleRoute> Add(IRouteView<ConsoleRoute> view)
        {
            Route?.Add(view.Route);
            Route?.Menu.Clear();
            foreach(var route in Route.Children)
            {
                Route.Menu.SetMenu(Tuple.Create(route.Name, route.Title));
            }
            return this;
        }
    }
}
