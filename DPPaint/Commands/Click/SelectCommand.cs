using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using DPPaint.Shapes;

namespace DPPaint.Commands.Click
{
    public class SelectCommand : ICanvasCommand
    {
        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        public Canvas Canvas { get; set; }
        public List<PaintBase> ShapeList { get; set; }

        private readonly ICanvasPage _page;
        private Shape selectorSquare;
        private Point _pointerStart;

        public SelectCommand(ICanvasPage page)
        {
            _page = page;
        }

        public void PointerPressedExecute()
        {
            _pointerStart = PointerEventArgs.GetCurrentPoint(Canvas).Position;
            selectorSquare = new Rectangle();

            selectorSquare.Width = 1;
            selectorSquare.Height = 1;
            selectorSquare.SetValue(Canvas.LeftProperty, _pointerStart.X);
            selectorSquare.SetValue(Canvas.TopProperty, _pointerStart.Y);

            Brush brush = new AcrylicBrush()
            {
                TintColor = Colors.Blue,
                TintOpacity = 0.1
            };

            selectorSquare.Fill = brush;

            Canvas.Children.Add(selectorSquare);
        }

        public void PointerReleasedExecute()
        {
            double x = (double)selectorSquare.GetValue(Canvas.LeftProperty);
            double y = (double)selectorSquare.GetValue(Canvas.TopProperty);
            double xMax = x + selectorSquare.Width;
            double yMax = y + selectorSquare.Height;

            foreach (PaintBase paintBase in ShapeList)
            {
                double eX = paintBase.X;
                double eY = paintBase.Y;
                double eXMax = eX + paintBase.Width;
                double eYMax = eY + paintBase.Height;

                if (((eX > x && eX < xMax) && (eY > y && eY < yMax)) &&
                    ((eXMax > x && eXMax < xMax) && (eYMax > y && eYMax < yMax)))
                {
                    paintBase.Selected = true;
                }
            }

            Canvas.Children.Remove(selectorSquare);
            selectorSquare = null;

            _page.UpdateList();
            _page.Draw();
        }

        public void PointerMovedExecute()
        {
            if (PointerEventArgs.Pointer.IsInContact && selectorSquare != null)
            {
                Point currentPoint = PointerEventArgs.GetCurrentPoint(Canvas).Position;
                Point difference = new Point(currentPoint.X - _pointerStart.X, currentPoint.Y - _pointerStart.Y);
                if (difference.X < 0)
                {
                    selectorSquare.Width = difference.X * -1.0;
                    selectorSquare.SetValue(Canvas.LeftProperty, _pointerStart.X + difference.X);
                }
                else
                {
                    selectorSquare.Width = difference.X;
                }

                if (difference.Y < 0)
                {
                    selectorSquare.Height = difference.Y * -1.0;
                    selectorSquare.SetValue(Canvas.TopProperty, _pointerStart.Y + difference.Y);
                }
                else
                {
                    selectorSquare.Height = difference.Y;
                }
            }
        }
    }
}
