using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
    public interface IElement
    {
        void Accept(IVisitor visitor);
    }

}
