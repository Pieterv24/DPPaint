using DPPaint.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace DPPaint.Extensions
{
    public static class ShapeExtensions
    {
        public static BaseShape Select(this BaseShape shape)
        {
            Selector selected = new Selector(shape.Element);
            
            var a = new BaseShape()
            {
                Element = selected,
                Height = selected.Height,
                Width = selected.Width,
                X = (double)selected.GetValue(Canvas.LeftProperty),
                Y = (double)selected.GetValue(Canvas.TopProperty)
            };

            return a;
        }

        public static BaseShape Deselect(this BaseShape sel)
        {
            Selector selector = sel.Element as Selector;

            if (selector == null)
            {
                return null;
            }

            var shape = selector.ExtractShape();

            return new BaseShape()
            {
                Element = shape,
                Height = shape.Height,
                Width = shape.Width,
                X = (double)shape.GetValue(Canvas.LeftProperty),
                Y = (double)shape.GetValue(Canvas.TopProperty)
            };
        }
    }
}
