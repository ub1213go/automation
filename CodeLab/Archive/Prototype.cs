using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLab.Archive.Prototype
{
    /*

    var l = new Line();
    l.Start = new Point() { X=1, Y=1 };
    l.End = new Point() { X=2, Y=2 };
    var nl = l.DeepCopy();
    nl.Start.X = 2;
    nl.End.X = 1;
    Console.WriteLine(l.ToString());
    Console.WriteLine(nl.ToString());

    var d = new Point3D() { X=1, Y=1, Z=1 };
    var d2 = d.DeepCopy();
    d2.Z = 2;
    Console.WriteLine(d.ToString());
    Console.WriteLine(d2.ToString());

     */
    public interface IDeepCopy<T> 
        where T : new()
    {
        void CopyTo(T obj);

        public T DeepCopy()
        {
            T t = new T();
            CopyTo(t);
            return t;
        }
    }
    public static class ExtensionModel
    {
        public static T DeepCopy<T>(this IDeepCopy<T> item)
            where T : new()
        {
            return item.DeepCopy();
        }

        public static T DeepCopy<T>(this T point)
            where T : Point, new()
        {
            return ((IDeepCopy<T>)point).DeepCopy();
        }
    }
    public class Point : IDeepCopy<Point>
    {
      public int X, Y;

        public void CopyTo(Point obj)
        {
            obj.X = X;
            obj.Y = Y;
        }

        public override string ToString()
        {
            return "[" + nameof(X) + ": " + X.ToString() + ", " + nameof(Y) + ": " + Y.ToString() + "]";
        }
    }

    public class Point3D : Point, IDeepCopy<Point3D>
    {
        public int Z;

        public void CopyTo(Point3D obj)
        {
            obj.X = X;
            obj.Y = Y;
            obj.Z = Z;
        }

        public override string ToString()
        {
            return "[" + nameof(X) + ": " + X.ToString() + ", " + nameof(Y) + ": " + Y.ToString() + ", " + nameof(Z) + ": " + Z.ToString() + "]";
        }
    }

    public class Line : IDeepCopy<Line>
    {
      public Point Start, End;

        public void CopyTo(Line obj)
        {
            obj.Start = Start.DeepCopy();
            obj.End = End.DeepCopy(); 
        }

        public override string ToString()
        {
            return nameof(Start) + ": " + Start.ToString() + ", " + nameof(End) + ": " + End.ToString();
        }
    }
}
