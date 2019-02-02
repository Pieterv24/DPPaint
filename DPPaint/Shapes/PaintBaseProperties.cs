using System.Numerics;

namespace DPPaint.Shapes
{
    public class PaintBaseProperties
    {
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
