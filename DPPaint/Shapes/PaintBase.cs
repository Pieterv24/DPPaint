using System.Numerics;
using DPPaint.Visitor;
using Newtonsoft.Json.Linq;

namespace DPPaint.Shapes
{
    public abstract class PaintBase : PaintBaseProperties, IVisitable
    {
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
        }

        public abstract void Add(PaintBase c);
        public abstract void Remove(PaintBase c);

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
