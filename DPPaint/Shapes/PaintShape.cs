using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml.Shapes;
using DPPaint.Strategy;
using DPPaint.Visitor;
using Newtonsoft.Json.Linq;

namespace DPPaint.Shapes
{
    /// <summary>
    /// Leaf for composite pattern
    /// </summary>
    public class PaintShape : PaintBase
    {
        private readonly IShapeBase _shape;

        public PaintShape(IShapeBase shape)
        {
            _shape = shape;
        }

        public PaintShape(PaintShape shape) : base(shape)
        {
            _shape = shape._shape;
        }

        public Shape GetDrawShape()
        {
            return _shape.GetDrawShape(this);
        }

        public override string ToString()
        {
            return _shape.ToString();
        }

        public override void Add(PaintBase c)
        {
        }

        public override void Remove(PaintBase c)
        {
        }
    }
}
