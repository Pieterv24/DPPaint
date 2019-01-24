using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace DPPaint.Shapes
{
    public class Group : RelativePanel
    {
        private int borderThickness = 2;

        public Group(List<Group> shapes)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;

            foreach(UIElement shape in shapes)
            {
                double X = (double)shape.GetValue(Canvas.LeftProperty);
                double Y = (double)shape.GetValue(Canvas.TopProperty);
                minX = X < minX ? X : minX;
                minY = Y < minY ? Y : minY;
                Children.Add(shape);
            }

            SetValue(Canvas.LeftProperty, minX - borderThickness);
            SetValue(Canvas.TopProperty, minY - borderThickness);

            BorderBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
            BorderThickness = new Thickness(borderThickness);
        }

        public List<UIElement> GetShapes()
        {
            List<UIElement> shapes = Children.OfType<UIElement>().ToList();

            return shapes;
        }
    }
}
