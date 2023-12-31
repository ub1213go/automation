using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AConsole.Model
{
    public class Rectangle : IDeepCopy<Rectangle>
    {
        public int[] Points = new int[4];

        public Point LeftTop => new Point(Points[0], Points[1]);

        public Point RightBottom => new Point(Points[2], Points[3]);

        public int this[int index]
        {
            get => Points[index];
            set => Points[index] = value;
        }

        public Rectangle()
        {

        }

        public Rectangle(int x1, int y1, int x2, int y2)
        {
            Points[0] = x1;
            Points[1] = y1;
            Points[2] = x2;
            Points[3] = y2;
        }

        public void RightBottomCorner(Point point)
        {
            if(LeftTop.X > (point.X - 1))
            {
                Points[0] = (point.X - 1);
            }

            if(RightBottom.X > (point.X - 1))
            {
                Points[2] = (point.X - 1);
            }

            if(LeftTop.Y > (point.Y - 1))
            {
                Points[1] = (point.Y - 1);
            }

            if(RightBottom.Y > (point.Y - 1))
            {
                Points[3] = (point.Y - 1);
            }
        }

        public int Width => Points[2] - Points[0] + 1;
        public int Height => Points[3] - Points[1] + 1;

        public override string ToString()
        {
            return $"({Points[0]}, {Points[1]}), ({Points[2]}, {Points[3]})";
        }

        public void CopyTo(Rectangle obj)
        {
            Points.CopyTo(obj.Points, 0);
        }

        public Rectangle Bais(int x1, int y1, int x2, int y2)
        {
            var copy = this.DeepCopy();
            copy.Points[0] += x1;
            copy.Points[1] += y1;
            copy.Points[2] += x2;
            copy.Points[3] += y2;
            return copy;
        }
    }

}
