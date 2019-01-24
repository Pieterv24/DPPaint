using System;
using System.Collections.Generic;
using Windows.Foundation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
using DPPaint.Shapes;

namespace DPPaint.Commands
{
    public class DrawShapeCommand : ICanvasCommand, IDisposable
    {
        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        public ShapeType ShapeType { get; set; }

        private Point _pressStart;
        private List<BaseShape> _shapeList;
        private BaseShape current;
        private Canvas _canvas;

        public DrawShapeCommand(Canvas canvas, List<BaseShape> shapeList)
        {
            _shapeList = shapeList;
            _canvas = canvas;
        }

        public void PointerPressedExecute()
        {
            if (PointerEventArgs != null)
            {
                _pressStart = PointerEventArgs.GetCurrentPoint(_canvas).Position;
                current = new BaseShape()
                {
                    X = _pressStart.X,
                    Y = _pressStart.Y,
                    Element = GetCurrentShape()
                };

                _shapeList.Add(current);

                _canvas.Children.Add(current.Element);
            }
        }

        public void PointerReleasedExecute()
        {
            current = null;
        }

        public void PointerMovedExecute()
        {
            if (PointerEventArgs.Pointer.IsInContact && current != null)
            {
                PointerPoint currentPoint = PointerEventArgs.GetCurrentPoint(_canvas);
                Point difference = new Point(currentPoint.Position.X - _pressStart.X, currentPoint.Position.Y - _pressStart.Y);
                if (difference.X < 0)
                {
                    current.Width = difference.X * -1.0;
                    current.X = _pressStart.X + difference.X;
                }
                else
                {
                    current.Width = difference.X;
                }
                if (difference.Y < 0)
                {
                    current.Height = difference.Y * -1.0;
                    current.Y = _pressStart.Y + difference.Y;
                }
                else
                {
                    current.Height = difference.Y;
                }
            }
        }

        private Shape GetCurrentShape()
        {
            if (ShapeType == ShapeType.Circle)
            {
                return new Ellipse();
            } else
            {
                return new Rectangle();
            }
        }

        public void Dispose()
        {
            if (current?.Element != null)
            {
                _canvas.Children.Remove(current.Element);
            }
        }
    }
}
