using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLab.Archive
{
    public class Neuron : IEnumerable<Neuron>
    {
        public float Value { get; set; }
        public Lazy<List<Neuron>> In = new Lazy<List<Neuron>>();
        public Lazy<List<Neuron>> Out = new Lazy<List<Neuron>>();

        public IEnumerator<Neuron> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class NeuronLayer : Collection<Neuron>
    {

    }

    public static class ExtensionMethods
    {
        public static void ConnectTo(this IEnumerable<Neuron> self,
            IEnumerable<Neuron> other)
        {
            if (ReferenceEquals(self, other)) return;

            foreach (var from in self)
            {
                foreach (var to in other)
                {
                    from.Out.Value.Add(to);
                    to.In.Value.Add(from);
                }
            }
        }
    }
}


