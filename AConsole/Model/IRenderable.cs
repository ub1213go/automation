using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
    public interface IRenderable<T>
    {
        void Render(Model.Rectangle triangle, T obj, int flag = 0);

        void Clear(Model.Rectangle rect);
    }
}
