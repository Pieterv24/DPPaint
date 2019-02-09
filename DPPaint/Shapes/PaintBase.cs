using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using DPPaint.Visitor;
using Newtonsoft.Json.Linq;

namespace DPPaint.Shapes
{
    public abstract class PaintBase : IVisitable
    {
        public abstract double Width { get; set; }
        public abstract double Height { get; set; }
        public abstract double X { get; set; }
        public abstract double Y { get; set; }
        public abstract bool Selected { get; set; }

        protected PaintBase()
        {

        }

        protected PaintBase(PaintBase paintBase)
        {
            Width = paintBase.Width;
            Height = paintBase.Height;
            X = paintBase.X;
            Y = paintBase.Y;
        }

        protected PaintBase(PaintBaseProperties props)
        {
            Width = props.Width;
            Height = props.Height;
            X = props.X;
            Y = props.Y;
            Selected = props.Selected;
        }

        public abstract void Add(PaintBase c);
        public abstract void Remove(PaintBase c);
        public abstract void DrawOnCanvas(Canvas canvas);

        public virtual void DrawSelector(Canvas canvas)
        {
            double width = Width;
            double height = Height;

            double x = X;
            double y = Y;

            if (width < 0)
            {
                x = x + width;
                width = width * -1.0;
            }

            if (height < 0)
            {
                y = y + height;
                height = height * -1.0;
            }

            Shape selectorSquare = new Rectangle();
            selectorSquare.Width = width + 4;
            selectorSquare.Height = height + 4;
            selectorSquare.SetValue(Canvas.LeftProperty, x - 2);
            selectorSquare.SetValue(Canvas.TopProperty, y - 2);
            selectorSquare.Stroke = new SolidColorBrush(Colors.Blue);

            canvas.Children.Add(selectorSquare);
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
