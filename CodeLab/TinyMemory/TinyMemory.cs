using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLab.TinyMemory
{
    public class TinyMemory : IEnumerable<object>
    {
        private Lazy<List<object>> _Items { get; } = new Lazy<List<object>>(()=> new List<object>());
        public List<object> Items => _Items.Value;

        public TinyMemory(params object[] items)
        {
            foreach(var item in items)
            {
                Items.Add(item);
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
