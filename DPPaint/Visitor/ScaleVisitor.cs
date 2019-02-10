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
    /// Visitor for scaling the visited item
    /// </summary>
    public class ScaleVisitor : IVisitor
    {
        private readonly double _deltaX;
        private readonly double _deltaY;

        /// <summary>
        /// Visitor to scale visited item
        /// </summary>
        /// <param name="deltaX">delta X</param>
        /// <param name="deltaY">delta Y</param>
        public ScaleVisitor(double deltaX, double deltaY)
        {
            _deltaX = deltaX;
            _deltaY = deltaY;
        }

        /// <summary>
        /// Scale visited element
        /// If visited element is a group,
        /// Visit its children recursively
        /// </summary>
        /// <param name="element">Element to visit</param>
        public void Visit(PaintBase element)
        {
            // Get inner non decorated element if element is decorated
            if (element is TextDecoration decor)
            {
                element = decor.GetDrawable();
            }

            if (element is PaintShape shape)
            {
                // If element is a shape, apply the transforamtion
                if (shape.Width + _deltaX > 0 && shape.Height + _deltaY > 0)
                {
                    shape.Width += _deltaX;
                    shape.Height += _deltaY;
                }
            } else if (element is PaintGroup group)
            {
                // If element is a group, make sure inner element stay in correct place
                double originalX = group.X;
                double originalY = group.Y;

                // Percentage x and y transformation in regards to the new group size
                double groupXMultiplier = (group.Width + _deltaX) / group.Width;
                double groupYMultiplier = (group.Height + _deltaY) / group.Height;

                // Make sure scale is not negative
                if (group.Width + _deltaX > 0 && group.Height + _deltaY > 0)
                {
                    // Apply new size to group
                    group.Width += _deltaX;
                    group.Height += _deltaY;

                    foreach (PaintBase paintBase in group.Children)
                    {
                        double newX = (paintBase.X - originalX) * groupXMultiplier + originalX;
                        double newY = (paintBase.Y - originalY) * groupYMultiplier + originalY;

                        double newXScale = (paintBase.Width * groupXMultiplier) - paintBase.Width;
                        double newYScale = (paintBase.Height * groupYMultiplier) - paintBase.Height;

                        // Move child according to transformation of parent group.
                        paintBase.Accept(new MoveVisitor(newX - paintBase.X, newY - paintBase.Y));
                        // Scale child accoarding to transformation of parent group.
                        paintBase.Accept(new ScaleVisitor(newXScale, newYScale));
                    }
                }
            }
        }
    }
}
