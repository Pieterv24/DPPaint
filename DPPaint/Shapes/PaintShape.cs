using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml.Controls;
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
        public override double Width { get; set; }
        public override double Height { get; set; }
        public override double X { get; set; }
        public override double Y { get; set; }
        public override bool Selected { get; set; }

        private readonly IShapeBase _shape;

        public PaintShape(IShapeBase shape)
        {
            _shape = shape;
        }

        public PaintShape(PaintShape shape) : base(shape)
        {
            _shape = shape._shape;
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

        public override void DrawOnCanvas(Canvas canvas)
        {
            if (Selected) base.DrawSelector(canvas);

            canvas.Children.Add(_shape.GetDrawShape(this));
        }
    }
}
