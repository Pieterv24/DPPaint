using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPPaint.Shapes
{
    public class PaintShape : PaintBase
    {
        public PaintShape()
        {

        }

        public PaintShape(PaintShape shape) : base(shape)
        {
            ShapeType = shape.ShapeType;
        }
        public ShapeType ShapeType { get; set; }
    }
}
