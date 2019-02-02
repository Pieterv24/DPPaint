using System.Numerics;
using Newtonsoft.Json.Linq;

namespace DPPaint.Shapes
{
    public abstract class PaintBase : PaintBaseProperties
    {
        //public double Width { get; set; }
        //public double Height { get; set; }
        //public double X { get; set; }
        //public double Y { get; set; }
        //public string Decoration { get; set; }
        //public bool Selected { get; set; }
        //public DecoratorAnchor Anchor { get; set; }
        //public Vector3 Scale { get; set; } = new Vector3(1f);
        
        protected PaintBase()
        {

        }

        protected PaintBase(PaintBase paintBase)
        {
            Width = paintBase.Width;
            Height = paintBase.Height;
            X = paintBase.X;
            Y = paintBase.Y;
            Decoration = paintBase.Decoration;
            Anchor = paintBase.Anchor;
            Scale = paintBase.Scale;
        }

        public abstract void Add(PaintBase c);
        public abstract void Remove(PaintBase c);

        public override string ToString()
        {
            return ToJObject().ToString();
        }

        public virtual JObject ToJObject()
        {
            return new JObject
            {
                { "width", Width },
                { "height", Height },
                { "x", X },
                { "y", Y },
                { "decoration", Decoration },
                { "anchor", (int)Anchor },
                { "scale", new JObject
                {
                    { "x", Scale.X },
                    { "y", Scale.Y },
                    { "z", Scale.Z }
                }}
            };
        }
    }
}
