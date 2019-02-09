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
    /// <summary>
    /// This command handles pointer events to draw an selection square to select multiple items
    /// </summary>
    public class SelectCommand : ICanvasCommand
    {
        /// <inheritdoc />
        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        /// <inheritdoc />
        public Canvas Canvas { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> UndoStack { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> RedoStack { get; set; }
        /// <inheritdoc />
        public List<PaintBase> ShapeList { get; set; }

        private readonly ICanvasPage _page;
        private Shape selectorSquare;
        private Point _pointerStart;

        public SelectCommand(ICanvasPage page)
        {
            _page = page;
        }

        /// <inheritdoc />
        public Task PointerPressedExecuteAsync()
        {
            _pointerStart = PointerEventArgs.GetCurrentPoint(Canvas).Position;

            // Create selector square
            selectorSquare = new Rectangle();

            selectorSquare.Width = 1;
            selectorSquare.Height = 1;
            selectorSquare.SetValue(Canvas.LeftProperty, _pointerStart.X);
            selectorSquare.SetValue(Canvas.TopProperty, _pointerStart.Y);

            // Give selector square blue acryl color
            Brush brush = new AcrylicBrush()
            {
                TintColor = Colors.Blue,
                TintOpacity = 0.1
            };

            selectorSquare.Fill = brush;

            Canvas.Children.Add(selectorSquare);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PointerReleasedExecuteAsync()
        {
            double x = (double)selectorSquare.GetValue(Canvas.LeftProperty);
            double y = (double)selectorSquare.GetValue(Canvas.TopProperty);
            double xMax = x + selectorSquare.Width;
            double yMax = y + selectorSquare.Height;

            // Find what elements are in the selector square
            foreach (PaintBase paintBase in ShapeList)
            {
                double eX = paintBase.X;
                double eY = paintBase.Y;
                double eXMax = eX + paintBase.Width;
                double eYMax = eY + paintBase.Height;

                // Check if paintBase is within the selector square
                if (((eX > x && eX < xMax) && (eY > y && eY < yMax)) &&
                    ((eXMax > x && eXMax < xMax) && (eYMax > y && eYMax < yMax)))
                {
                    paintBase.Selected = true;
                }
            }

            // Delete selector square
            Canvas.Children.Remove(selectorSquare);
            selectorSquare = null;

            _page.UpdateList();
            _page.Draw();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PointerMovedExecuteAsync()
        {
            if (PointerEventArgs.Pointer.IsInContact && selectorSquare != null)
            {
                // Update the size of the selector square
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

            return Task.CompletedTask;
        }
    }
}
