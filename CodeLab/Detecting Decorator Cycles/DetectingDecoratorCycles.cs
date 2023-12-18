using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLab.Detecting_Decorator_Cycles
{
    public class Test
    {
        public void test1()
        {
            var circle = new Circle(2);
            var colored1 = new ColoredShape(circle, "red");
            var colored2 = new TransparentShape(colored1, 0.8f);
            var colored3 = new TransparentShape(colored2, 0.8f);
            Console.WriteLine(colored3.AsString());
        }
    }
    public abstract class Shape
    {
        public virtual string AsString()
        {
            return $"a shape";
        }
    } 
    public class Circle : Shape
    {
        private float Radius;
        public Circle(float radius)
        {
            Radius = radius;
        }
        public void Resize(float factor)
        {
            Radius = factor * Radius;
        }
        public override string AsString()
        {
            return $"a circle with radius {Radius}";
        }
    }
    public class Square : Shape
    {
        private float side;
        public Square(float side)
        {
            this.side = side;
        }
        public override string AsString() => $"a square with side {side}";
    }
    public class TransparentShape : Shape
    {
        private Shape shape;
        private float transparency;
        public TransparentShape(Shape shape, float transparency)
        {
            this.shape = shape;
            this.transparency = transparency;
        }
        public override string AsString()
        {
            return $"{shape.AsString()} has {transparency * 100}% transparency";
        }
    }
    public abstract class ShapeDecorator : Shape
    {
        protected internal readonly List<Type> types = new();
        protected internal Shape shape;
        public ShapeDecorator(Shape shape)
        {
            this.shape = shape;
            if(shape is ShapeDecorator sd)
            {
                types.AddRange(sd.types);
            }
        }
    }
    public abstract class ShapeDecorator<TSelf, TCyclePolicy> : ShapeDecorator 
        where TCyclePolicy : ShapeDecoratorCyclePolicy, new()
    {
        protected readonly TCyclePolicy policy = new();
        protected ShapeDecorator(Shape shape) : base(shape)
        {
            if(policy.TypeAdditionAllowed(typeof(TSelf), types))
            {
                types.Add(typeof(TSelf));
            }
        }
    }
    public class ShapeDecoratorWithPolicy<T>
        : ShapeDecorator<T, ThrowOnCyclePolicy>
    {
        public ShapeDecoratorWithPolicy(Shape shape) : base(shape)
        {

        }
    }
    public class ColoredShape 
        : ShapeDecoratorWithPolicy<ColoredShape>
    {
        //private Shape shape;
        private string color;
        public ColoredShape(Shape shape, string color) : base(shape)
        {
            this.shape = shape;
            this.color = color;
        }
        public override string AsString()
        {
            return $"{shape.AsString()} has the color {color}";
        }
    }
    public abstract class ShapeDecoratorCyclePolicy
    {
        public abstract bool TypeAdditionAllowed(Type type, IList<Type> allTypes);
        public abstract bool ApplicationAllowed(Type type, IList<Type> allTypes);
    }
    public class ThrowOnCyclePolicy : ShapeDecoratorCyclePolicy
    {
        private bool handler(Type type, IList<Type> allTypes)
        {
            if (allTypes.Contains(type))
            {
                throw new InvalidOperationException($"Cycle detected! Type is already a {type.FullName}");
            }
            return true;
        }
        public override bool TypeAdditionAllowed(Type type, IList<Type> allTypes)
        {
            return handler(type, allTypes);
        }
        public override bool ApplicationAllowed(Type type, IList<Type> allTypes)
        {
            return handler(type, allTypes);
        }

    }
}
