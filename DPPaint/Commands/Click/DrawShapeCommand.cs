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
    public class DrawShapeCommand : ICanvasCommand
    { 
        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        public IShapeBase ShapeType { get; set; }
        public Canvas Canvas { get; set; }
        public Stack<List<PaintBase>> UndoStack { get; set; }
        public Stack<List<PaintBase>> RedoStack { get; set; }
        public List<PaintBase> ShapeList { get; set; }

        private readonly ICanvasPage _page;
        private Point _pointerStart;
        private PaintBase current;

        public DrawShapeCommand(ICanvasPage page)
        {
            _page = page;
        }

        public Task PointerPressedExecuteAsync()
        {
            UndoStack.Push(ShapeList.DeepCopy());
            RedoStack.Clear();

            _pointerStart = PointerEventArgs.GetCurrentPoint(Canvas).Position;

            PaintBase shape = new PaintShape(ShapeType);

            shape.X = _pointerStart.X;
            shape.Y = _pointerStart.Y;

            ShapeList.Add(shape);
            current = shape;

            _page.Draw();
            _page.UpdateList();

            return Task.CompletedTask;
        }

        public Task PointerReleasedExecuteAsync()
        {
            current = null;

            return Task.CompletedTask;
        }

        public Task PointerMovedExecuteAsync()
        {
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
    }
}
