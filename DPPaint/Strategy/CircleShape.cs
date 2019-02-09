using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using DPPaint.Shapes;

namespace DPPaint.Strategy
{
    public class CircleShape : IShapeBase
    {
        private static CircleShape _instance = null;
        private static readonly object Mutex = new object();

        private CircleShape()
        {

        }

        public static CircleShape Instance
        {
            get
            {
                lock (Mutex)
                {
                    if (_instance == null)
                    {
                        _instance = new CircleShape();
                    }

                    return _instance;
                }
            }
        }

        public Shape GetDrawShape(PaintBase paintBase)
        {
            Shape drawShape = new Ellipse();

            double x = paintBase.Width < 0 ? paintBase.X + paintBase.Width : paintBase.X;
            double y = paintBase.Height < 0 ? paintBase.Y + paintBase.Height : paintBase.Y;

            drawShape.SetValue(Canvas.LeftProperty, x);
            drawShape.SetValue(Canvas.TopProperty, y);
            drawShape.Width = paintBase.Width < 0 ? paintBase.Width * -1 : paintBase.Width;
            drawShape.Height = paintBase.Height < 0 ? paintBase.Height * -1 : paintBase.Height;

            drawShape.Fill = new SolidColorBrush(Colors.Black);

            return drawShape;
        }

        public override string ToString()
        {
            return "Circle";
        }
    }
}
