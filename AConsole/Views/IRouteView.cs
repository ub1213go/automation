using AConsole.Model;
using AConsole.Model.ConsoleUI;
using Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Views
{
    public interface IRouteView<T> : IElement 
    {
        ConsoleRoute? Route { get; set; }
        AutoService Service { get; }
        IRouteView<T> Add(IRouteView<T> route);
    }
}
