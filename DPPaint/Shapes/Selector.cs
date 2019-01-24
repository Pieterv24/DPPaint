using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace DPPaint.Shapes
{
    public class Selector : RelativePanel
    {
        private int borderThickness = 2;

        public Selector(FrameworkElement shape)
        {
            double X = (double)shape.GetValue(Canvas.LeftProperty);
            double Y = (double)shape.GetValue(Canvas.TopProperty);

            if (shape.Parent is Canvas canvas)
            {
                canvas.Children.Remove(shape);
            }
            Children.Add(shape);

            SetValue(Canvas.LeftProperty, X - borderThickness);
            SetValue(Canvas.TopProperty, Y - borderThickness);
            Width = shape.Width + (borderThickness * 2);
            Height = shape.Height + (borderThickness * 2);

            BorderBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
            BorderThickness = new Thickness(borderThickness);
        }

        public FrameworkElement ExtractShape()
        {
            FrameworkElement shape = Children.OfType<FrameworkElement>().First();
            Children.Remove(shape);
            if (Parent is Canvas canvas)
            {
                canvas.Children.Remove(this);
            }

            return shape;
        }
    }
}
