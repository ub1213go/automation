using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model.ConsoleUI
{
    public interface IConsolePage
    {
        void Render(ConsoleMenu menu);

        void Clear();
    }

}
