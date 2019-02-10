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
    /// Implementation of TextDecoration with logic to draw the decoration on the bottom
    /// </summary>
    public class BottomDecoration : TextDecoration
    {
        public BottomDecoration(PaintBase paintBase) : base(paintBase)
        {
        }

        public BottomDecoration(PaintBase paintBase, string decorationText) : base(paintBase, decorationText)
        {
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
            double newX = (X + (Width / 2)) - (tb.ActualWidth / 2);
            double newY = Y + Height;

            double maxX = newX + tb.ActualWidth;
            double maxY = newY + tb.ActualHeight;

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

            double newX = (X + (Width / 2)) - (tb.ActualWidth / 2);

            tb.SetValue(Canvas.LeftProperty, newX);
            tb.SetValue(Canvas.TopProperty, Y + Height);

            canvas.Children.Add(tb);

            base.DrawOnCanvas(canvas);
        }

        /// <inheritdoc />
        public override DecoratorAnchor GetDecoratorPosition()
        {
            return DecoratorAnchor.Bottom;
        }
    }
}
