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
        public List<ConsoleRoute> Children { get; set; }
            = new List<ConsoleRoute>();
        public ConsoleRoute Parent { get; set; } 
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

            Children.AddRange(routes);
        }

        public void Add(ConsoleRoute? route)
        {
            if(route != null)
            {
                Children.Add(route);
                route.Parent = this;
            }
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Clear();
            visitor.Render(Menu);
            Hint.Clear();
            Hint.Render();
            foreach(var loop in Menu.Run())
            {
                Hint.Clear();
                Hint.Render();
                visitor.Clear();
                visitor.Render(Menu);
            }
        }

        public void Subscription(IObserver<ConsoleMenu> observer, ConsoleKey key)
        {
            Menu.Subscription(observer, key);
        }
    }

}
