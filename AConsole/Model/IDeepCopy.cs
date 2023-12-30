using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
    public interface IDeepCopy<T>
        where T : new()
    {
        void CopyTo(T obj);

        public T DeepCopy()
        {
            var t = new T();
            CopyTo(t);
            return t;
        }
    }
}
