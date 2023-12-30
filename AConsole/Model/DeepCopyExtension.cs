using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
    public static class DeepCopyExtension
    {
        public static T DeepCopy<T>(this IDeepCopy<T> obj)
            where T : new()
        {
            return obj.DeepCopy();
        }
    }
}
