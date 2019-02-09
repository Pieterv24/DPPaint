using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPPaint.Decorators;
using DPPaint.Shapes;

namespace DPPaint.Visitor
{
    public class ScaleVisitor : IVisitor
    {
        private readonly double _deltaX;
        private readonly double _deltaY;

        public ScaleVisitor(double deltaX, double deltaY)
        {
            _deltaX = deltaX;
            _deltaY = deltaY;
        }


        public void Visit(PaintBase element)
        {
            if (element is TextDecoration decor)
            {
                element = decor.GetDrawable();
            }

            if (element is PaintShape shape)
            {
                if (shape.Width + _deltaX > 0 && shape.Height + _deltaY > 0)
                {
                    shape.Width += _deltaX;
                    shape.Height += _deltaY;
                }
            } else if (element is PaintGroup group)
            {
                double originalX = group.X;
                double originalY = group.Y;

                double groupXMultiplier = (group.Width + _deltaX) / group.Width;
                double groupYMultiplier = (group.Height + _deltaY) / group.Height;

                if (group.Width + _deltaX > 0 && group.Height + _deltaY > 0)
                {
                    group.Width += _deltaX;
                    group.Height += _deltaY;

                    foreach (PaintBase paintBase in group.Children)
                    {
                        double newX = (paintBase.X - originalX) * groupXMultiplier + originalX;
                        double newY = (paintBase.Y - originalY) * groupYMultiplier + originalY;

                        double newXScale = (paintBase.Width * groupXMultiplier) - paintBase.Width;
                        double newYScale = (paintBase.Height * groupYMultiplier) - paintBase.Height;

                        paintBase.Accept(new MoveVisitor(newX - paintBase.X, newY - paintBase.Y));
                        paintBase.Accept(new ScaleVisitor(newXScale, newYScale));
                    }
                }
            }
        }
    }
}
