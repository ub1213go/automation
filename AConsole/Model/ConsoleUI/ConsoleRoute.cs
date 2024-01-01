using AConsole.MyAutofac;
using FlaUI.Core.AutomationElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model.ConsoleUI
{
    public class ConsoleRoute : IElement
    {
        private List<ConsoleRoute> Routes { get; set; }
            = new List<ConsoleRoute>();
        public ConsoleMenu Menu { get; set; }
        public ConsoleHint Hint { get; set; }
        public string Name {  get; set; }
        public string Title {  get; set; }
        public ConsoleRoute(string name, string title, params ConsoleRoute[] routes)
        {
            Name = name;
            Title = title;
            Menu = Autofac.auto.Get<ConsoleMenu>();
            Hint = Autofac.auto.Get<ConsoleHint>("hint");

            Routes.AddRange(routes);
        }

        public void Add(ConsoleRoute route)
        {
            Routes.Add(route);
        }

        public void Accept(IVisitor visitor)
        {
            foreach(var r in Routes)
            {
                Menu.SetMenu(r.Title);
            }
            visitor.Render(Menu);
            foreach(var loop in Menu.Run())
            {

            }
        }

        public void Subscription(IObserver observer, ConsoleKey key)
        {
            Menu.Subscription(observer, key);
        }
    }

    public interface IElement
    {
        void Accept(IVisitor visitor);
    }

    public interface IVisitor : IConsolePage
    {
        void Visit(IElement ele);
    }
}
