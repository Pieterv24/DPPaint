using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using DPPaint.Shapes;

namespace DPPaint.Decorators
{
    public class TopDecoration : TextDecoration
    {
        public TopDecoration(PaintBase paintBase) : base(paintBase)
        {
        }

        public TopDecoration(PaintBase paintBase, string decorationText) : base(paintBase)
        {
            DecorationText = decorationText;
        }

        public override TextDecoration GetClickedDecoration(double clickX, double clickY)
        {
            TextBlock tb = new TextBlock
            {
                Text = DecorationText,
                IsTextSelectionEnabled = false,
                TextWrapping = TextWrapping.NoWrap,
                Foreground = new SolidColorBrush(Colors.Black)
            };

            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            double newX = (X + (Width / 2)) - (tb.ActualWidth / 2);
            double newY = Y - tb.ActualHeight;

            double maxX = newX + tb.ActualWidth;
            double maxY = newY + tb.ActualHeight;

            if ((clickX > newX && clickX < maxX) && (clickY > newY && clickY < maxY))
            {
                return this;
            }
            else if (_paintBase is TextDecoration decoration)
            {
                TextDecoration decor = decoration.GetClickedDecoration(clickX, clickY);
                if (decor != null)
                {
                    return decor;
                }
            }

            return null;
        }

        public override void DrawOnCanvas(Canvas canvas)
        {
            TextBlock tb = new TextBlock
            {
                Text = DecorationText,
                IsTextSelectionEnabled = false,
                TextWrapping = TextWrapping.NoWrap,
                Foreground = new SolidColorBrush(Colors.Black)
            };

            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            double newX = (X + (Width / 2)) - (tb.ActualWidth / 2);

            tb.SetValue(Canvas.LeftProperty, newX);
            tb.SetValue(Canvas.TopProperty, Y - tb.ActualHeight);

            canvas.Children.Add(tb);

            base.DrawOnCanvas(canvas);
        }

        public override DecoratorAnchor GetDecoratorPosition()
        {
            return DecoratorAnchor.Top;
        }
    }
}
