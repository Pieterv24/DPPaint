using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPPaint.Shapes;

namespace DPPaint.Visitor
{
    public class MoveVisitor : IVisitor
    {
        private readonly double _deltaX;
        private readonly double _deltaY;

        public MoveVisitor(double deltaX, double deltaY)
        {
            _deltaX = deltaX;
            _deltaY = deltaY;
        }


        public void Visit(PaintBase element)
        {
            if (element is PaintShape shape)
            {
                shape.X += _deltaX;
                shape.Y += _deltaY;
            } else if (element is PaintGroup group)
            {
                group.X += _deltaX;
                group.Y += _deltaY;

                foreach (PaintBase paintBase in group.Children)
                {
                    paintBase.Accept(this);
                }
            }
        }
    }
}
