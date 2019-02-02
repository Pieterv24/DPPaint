using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using DPPaint.Shapes;

namespace DPPaint.Extensions
{
    public static class HelperExtensions
    {
        public static List<PaintBase> DeepCopy(this List<PaintBase> list)
        {
            List<PaintBase> copy = new List<PaintBase>();

            foreach (PaintBase baseItem in list)
            {
                if (baseItem is PaintShape shape)
                {
                    copy.Add(new PaintShape(shape));
                } else if (baseItem is PaintGroup group)
                {
                    copy.Add(new PaintGroup(group));
                }
            }

            return copy;
        }

        public static Shape GetShape(this ShapeType shapeType)
        {
            if (shapeType == ShapeType.Circle)
            {
                return new Ellipse();
            }

            if (shapeType == ShapeType.Rectangle)
            {
                return new Rectangle();
            }

            return null;
        }
    }
}
