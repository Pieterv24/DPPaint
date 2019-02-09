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
using DPPaint.Decorators;
using DPPaint.Extensions;
using DPPaint.Shapes;
using DPPaint.Strategy;

namespace DPPaint.Commands.Click
{
    /// <summary>
    /// This command handles the click events to draw new shapes on the canvas
    /// </summary>
    public class DrawShapeCommand : ICanvasCommand
    {
        #region Properties

        /// <inheritdoc />
        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        /// <inheritdoc />
        public IShapeBase ShapeType { get; set; }
        /// <inheritdoc />
        public Canvas Canvas { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> UndoStack { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> RedoStack { get; set; }
        /// <inheritdoc />
        public List<PaintBase> ShapeList { get; set; }

        #endregion

        private readonly ICanvasPage _page;
        private Point _pointerStart;
        private PaintBase current;

        public DrawShapeCommand(ICanvasPage page)
        {
            _page = page;
        }

        #region Command pattern entries

        /// <inheritdoc />
        public Task PointerPressedExecuteAsync()
        {
            // Add entry to UndoStack
            UndoStack.Push(ShapeList.DeepCopy());
            RedoStack.Clear();

            // Set pointer starting point
            _pointerStart = PointerEventArgs.GetCurrentPoint(Canvas).Position;


            // Create new shape
            PaintBase shape = new PaintShape(ShapeType);

            shape.X = _pointerStart.X;
            shape.Y = _pointerStart.Y;

            ShapeList.Add(shape);
            current = shape;

            _page.Draw();
            _page.UpdateList();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PointerReleasedExecuteAsync()
        {
            // remove current item
            current = null;

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PointerMovedExecuteAsync()
        {
            // Update current item accoarding to mouse movements
            if (PointerEventArgs.Pointer.IsInContact && current != null)
            {
                Point currentPoint = PointerEventArgs.GetCurrentPoint(Canvas).Position;
                Point difference = new Point(currentPoint.X - _pointerStart.X, currentPoint.Y - _pointerStart.Y);
                if (difference.X < 0)
                {
                    current.Width = difference.X * -1.0;
                    current.X = _pointerStart.X + difference.X;
                }
                else
                {
                    current.Width = difference.X;
                }
                if (difference.Y < 0)
                {
                    current.Height = difference.Y * -1.0;
                    current.Y = _pointerStart.Y + difference.Y;
                }
                else
                {
                    current.Height = difference.Y;
                }

                _page.Draw();
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
