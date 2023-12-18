using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLab.Archive
{
    public class Test
    {
        public void Test1()
        {
            var nl = new NList<Node>();
            nl[0] = "哈囉";
            nl[1] = "你好";
            nl[2] = "我很好";
            Console.WriteLine(
                nl[0]?.ToString() + " " +
                nl[1]?.ToString() + " " +
                nl[2]?.ToString() + " " +
                ": 1 2 3"
            );
        }
    }

    public class Screen
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<IDisplayable> Displays { get; set; } = new List<IDisplayable>();
        public Screen(int width, int height)
        {
            Width = width;
            Height = height;
        }
        public Screen Add(IDisplayable obj)
        {
            Displays.Add(obj);

            return this;
        }
        public void Show()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Height; i++)
            {
                sb.Append(new string(' ', Width));
                sb.AppendLine();
            }
            for (int i = 0; i < Displays.Count; i++)
            {
                var str = Displays[i].ToDrawString();
                for (int j = 0; j < str.Length && !Displays[i].IsOverflow(j); j++)
                {
                    sb[Displays[i].Position(this) + j] = str[j];
                }
            }

            Console.WriteLine(sb.ToString());
        }
    }
    public class DisplayContext : IDisposable
    {
        public int Width;
        public int Height;
        public int X;
        public int Y;
        private static Stack<DisplayContext> Stack = new Stack<DisplayContext>();
        public static DisplayContext Current => Stack.Peek();
        static DisplayContext()
        {
            Stack.Push(new DisplayContext(0, 0, 0, 0));
        }
        public DisplayContext(int width, int height, int x, int y)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
            Stack.Push(this);
        }
        public void Dispose()
        {
            if (Stack.Count > 0)
                Stack.Pop();
        }
    }
    public class NodeDisplay : IDisplayable
    {
        public int Width { get; }
        public int Height { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Visible { get; set; }
        public Node? Node { get; set; } = new Node();
        public NodeDisplay(bool visible = true)
        {
            Width = DisplayContext.Current.Width;
            Height = DisplayContext.Current.Height;
            X = DisplayContext.Current.X;
            Y = DisplayContext.Current.Y;
            Visible = visible;
        }
        public NodeDisplay(int width, int height, int x, int y, bool visible)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
            Visible = visible;
        }
        public NodeDisplay SetValue(object value)
        {
            if (Node == null) throw new NullReferenceException();

            Node.Value = value;
            return this;
        }
        public int Position(Screen scn)
        {
            return Y * (scn.Width + Environment.NewLine.Length) + X;
        }
        public bool IsOverflow(int n)
        {
            return n >= Width;
        }
        public string ToDrawString()
        {
            return Node?.Value?.ToString() ?? "";
        }
    }
    public interface IDisplayable
    {
        string ToDrawString();
        int Position(Screen scn);
        bool IsOverflow(int n);
    }
    public interface INode : IComparable<INode>
    {
        object? Value { get; set; }
        INode Add(object? value);
        INode Next(int step);
    }
    public class Node : INode
    {
        public Node? Link { get; set; }
        public object? Value { get; set; }

        public Node()
        {

        }
        public Node(object? value)
        {
            Value = value;
        }
        public INode Add(object? value)
        {
            var now = this;
            while (now.Link != null)
            {
                now = now.Link;
            }
            now.Link = new Node(value);
            return this;
        }
        public int CompareTo(INode? other)
        {
            return Value?.ToString()?.Length.CompareTo(other?.Value?.ToString()?.Length) ?? 0;
        }
        public INode Next(int step)
        {
            Node? now = this;
            for (int i = 0; i < step; i++)
            {
                now = now.Link;
                if (now == null)
                {
                    throw new IndexOutOfRangeException();
                }
            }
            return now;
        }
    }
    public class TreeNode : INode
    {
        public TreeNode? Left { get; set; }
        public TreeNode? Right { get; set; }

        public object? Value { get; set; }
        public TreeNode(object? value)
        {
            Value = value;
        }
        public INode Add(object? value)
        {
            var tn = new TreeNode(value);
            var now = this;
            while (true)
            {
                if (now.CompareTo(tn) < 0)
                {
                    if (now.Left != null)
                    {
                        now = now.Left;
                    }
                    else
                    {
                        now.Left = tn;
                        break;
                    }
                }
                else
                {
                    if (now.Right != null)
                    {
                        now = now.Right;
                    }
                    else
                    {
                        now.Right = tn;
                        break;
                    }
                }
            }

            return this;
        }
        public int CompareTo(INode? other)
        {
            return Value?.ToString()?.Length.CompareTo(other?.Value?.ToString()?.Length) ?? 0;
        }

        public INode Next(int step)
        {
            throw new NotImplementedException();
        }
    }
    public class NList<T>
        where T : INode, new()
    {
        protected INode? Root { get; set; }
        public int Count { get; set; }
        public NList<T> Push(object? value)
        {
            if (Root == null)
            {
                Root = new T();
                Root.Value = value;
            }
            else
            {
                Root?.Add(value);
            }

            Count++;
            return this;
        }
        public object? Search(int index)
        {
            if (Root == null) throw new IndexOutOfRangeException();

            return Root?.Next(index).Value;
        }
        public object? this[int i]
        {
            get
            {
                return Search(i);
            }
            set
            {
                Push(value);
            }
        }
    }
}


