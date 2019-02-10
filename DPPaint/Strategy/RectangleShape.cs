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
    /// <summary>
    /// Circle strategy
    /// </summary>
    public class RectangleShape : IShapeBase
    {
        private static RectangleShape _instance = null;
        private static readonly object Mutex = new object();

        private RectangleShape()
        {

        }

        public static RectangleShape Instance
        {
            get
            {
                lock (Mutex)
                {
                    if (_instance == null)
                    {
                        _instance = new RectangleShape();
                    }

                    return _instance;
                }
            }
        }

        /// <inheritdoc />
        public Shape GetDrawShape(PaintBase paintBase)
        {
            Shape drawShape = new Rectangle();

            double x = paintBase.Width < 0 ? paintBase.X + paintBase.Width : paintBase.X;
            double y = paintBase.Height < 0 ? paintBase.Y + paintBase.Height : paintBase.Y;

            drawShape.SetValue(Canvas.LeftProperty, x);
            drawShape.SetValue(Canvas.TopProperty, y);
            drawShape.Width = paintBase.Width < 0 ? paintBase.Width * -1 : paintBase.Width;
            drawShape.Height = paintBase.Height < 0 ? paintBase.Height * -1 : paintBase.Height;

            drawShape.Fill = new SolidColorBrush(Colors.Black);

            return drawShape;
        }

        /// <inheritdoc cref="IShapeBase"/>
        public override string ToString()
        {
            return "Rectangle";
        }
    }
}
