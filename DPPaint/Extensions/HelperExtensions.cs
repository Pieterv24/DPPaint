using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPPaint.Shapes;

namespace DPPaint.Extensions
{
    public static class HelperExtensions
    {
        public static List<PaintBase> DeepCopy(this List<PaintBase> list)
        {
            List<PaintBase> copy = list.Select(baseItem =>
            {
                if (baseItem is PaintShape shape)
                {
                    return new PaintShape(shape);
                }
                else if (baseItem is PaintGroup grp)
                {
                    return new PaintGroup(grp);
                }
                else
                {
                    return new PaintBase(baseItem);
                }
            }).ToList();

            return copy;
        }
    }
}
