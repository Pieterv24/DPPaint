using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPPaint.Decorators;
using DPPaint.Shapes;

namespace DPPaint.Visitor
{
    /// <summary>
    /// Visitor for moving the visited item
    /// </summary>
    public class MoveVisitor : IVisitor
    {
        private readonly double _deltaX;
        private readonly double _deltaY;

        /// <summary>
        /// Visitor to move visited item
        /// </summary>
        /// <param name="deltaX">delta X</param>
        /// <param name="deltaY">delta Y</param>
        public MoveVisitor(double deltaX, double deltaY)
        {
            _deltaX = deltaX;
            _deltaY = deltaY;
        }

        /// <summary>
        /// Move visited element
        /// If visited element is a group,
        /// Visit its children recursively
        /// </summary>
        /// <param name="element">Element to visit</param>
        public void Visit(PaintBase element)
        {
            if (element is TextDecoration decor)
            {
                element = decor.GetDrawable();
            }

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
