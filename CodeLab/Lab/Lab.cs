using Autofac;
using CodeLab.Archive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CodeLab.Lab
{

    //var n = TreeNode.BlankTree
    //    .SetLeft(13)
    //    .SetRight(20)
    //    .Get();

    //var sb = new StringBuilder();
    //var a1 = new string[] { "R" };
    //var a2 = new string[] { "L", "R" };
    //var a3 = new string[] { "L", "R", "L", "R" };
    //var a4 = new string[] { "L", "R", "L", "R", "L", "R", "L", "R"};
    //var arr = new string[][] { a1, a2, a3, a4 };
    //for(int i = 0; i < arr.Length; i++)
    //{
    //    var len = arr.Length;
    //    sb.Append(new string(' ', (int)Math.Pow(len, 2) - (int)Math.Pow(i, 2)));
    //    for(int j = 0; j < arr[i].Length; j++)
    //    {
    //        sb.Append(arr[i][j]);
    //        sb.Append(new string(' ', (int)Math.Pow(len - i, 2) + (j % 2) * i));
    //    }
    //    sb.AppendLine();
    //}
    //Console.WriteLine(sb.ToString());


    public class TEST
    {
        private TEST()
        {

        }

        private static Lazy<TEST> _Instance = new Lazy<TEST>(() => new TEST());
        private static TEST Instance = _Instance.Value;
        public static TEST GetInstance()
        {
            return Instance;
        }

        public static bool CheckIsSameObject(object A, object B)
        {
            return ReferenceEquals(A, B);
        }

        public void SingletonTest(object A, object B)
        {
            var test = GetInstance();
            var test2 = GetInstance();
            var test_result = CheckIsSameObject(test, test2);

            Console.WriteLine($"{nameof(A)}_{A.GetType().Name} == {nameof(B)}_{B.GetType().Name} = {test_result}");
        }

        public void SingletonCheckTest()
        {
            GetInstance().SingletonTest(
                GetInstance(),
                GetInstance()
            );
        }

        public void AutofacTest()
        {
            var builder = new ContainerBuilder();

            using (var cb = builder.Build())
            {
                Console.WriteLine("TEST START.");
            }
        }

        public void NeuronTest()
        {
            var n1 = new Neuron();
            var n2 = new Neuron();

            n1.ConnectTo(n2);

            var layer1 = new NeuronLayer();
            var layer2 = new NeuronLayer();

            n1.ConnectTo(layer1);
            layer1.ConnectTo(layer2);
        }
    }
}
