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
    /// <summary>
    /// Implementation of TextDecoration with logic to draw the decoration on the right
    /// </summary>
    public class RightDecoration : TextDecoration
    {
        public RightDecoration(PaintBase paintBase) : base(paintBase)
        {
        }

        public RightDecoration(PaintBase paintBase, string decorationText) : base(paintBase)
        {
            DecorationText = decorationText;
        }

        /// <inheritdoc />
        public override TextDecoration GetClickedDecoration(double clickX, double clickY)
        {
            // Create instance of TextBlock to use for measurement
            TextBlock tb = new TextBlock
            {
                Text = DecorationText,
                IsTextSelectionEnabled = false,
                TextWrapping = TextWrapping.NoWrap,
                Foreground = new SolidColorBrush(Colors.Black)
            };

            // Calculate actual size of the TextBlock
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            // Calculate Top left and bottom right coordinates of TextBlock on canvas
            double newX = X + Width;
            double newY = (Y + (Height / 2)) - (tb.ActualWidth / 2);

            double maxX = newX + tb.ActualHeight;
            double maxY = newY + tb.ActualWidth;

            // Check if that TextBlock was clicked
            if ((clickX > newX && clickX < maxX) && (clickY > newY && clickY < maxY))
            {
                return this;
            }
            // If not, and the decorated object is also a decoration, check that recursively
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

        /// <summary>
        /// Override DrawOnCanvas to add the decoration itself to the canvas
        /// </summary>
        /// <param name="canvas">Canvas to add decoration to</param>
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

            Transform rotate = new RotateTransform()
            {
                Angle = 90
            };

            tb.RenderTransform = rotate;

            double newY = (Y + (Height / 2)) - (tb.ActualWidth / 2);

            tb.SetValue(Canvas.LeftProperty, X + Width + tb.ActualHeight);
            tb.SetValue(Canvas.TopProperty, newY);

            canvas.Children.Add(tb);

            base.DrawOnCanvas(canvas);
        }

        /// <inheritdoc />
        public override DecoratorAnchor GetDecoratorPosition()
        {
            return DecoratorAnchor.Right;
        }
    }
}
