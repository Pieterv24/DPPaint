using System.Numerics;

namespace DPPaint.Shapes
{
    public class PaintBase
    {
        public PaintBase()
        {

        }

        public PaintBase(PaintBase paintBase)
        {
            Width = paintBase.Width;
            Height = paintBase.Height;
            X = paintBase.X;
            Y = paintBase.Y;
            Decoration = paintBase.Decoration;
            Anchor = paintBase.Anchor;
            Scale = paintBase.Scale;
        }

        public double Width { get; set; }
        public double Height { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Decoration { get; set; }
        public bool Selected { get; set; }
        public DecoratorAnchor Anchor { get; set; }
        public Vector3 Scale { get; set; } = new Vector3(1f);
    }
}
