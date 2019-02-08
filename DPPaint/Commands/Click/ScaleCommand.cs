using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using DPPaint.Extensions;
using DPPaint.Shapes;
using DPPaint.Visitor;

namespace DPPaint.Commands.Click
{
    public class ScaleCommand : ICanvasCommand
    {
        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        public Canvas Canvas { get; set; }
        public Stack<List<PaintBase>> UndoStack { get; set; }
        public Stack<List<PaintBase>> RedoStack { get; set; }
        public List<PaintBase> ShapeList { get; set; }

        private readonly ICanvasPage _page;

        private Point _prevPointer;
        private List<PaintBase> _selected;

        public ScaleCommand(ICanvasPage page)
        {
            _page = page;
        }

        public void PointerPressedExecute()
        {
            UndoStack.Push(ShapeList.DeepCopy());
            RedoStack.Clear();

            _prevPointer = PointerEventArgs.GetCurrentPoint(Canvas).Position;
            _selected = ShapeList.Where(bs => bs.Selected).ToList();
        }

        public void PointerReleasedExecute()
        {
            _selected = null;
        }

        public void PointerMovedExecute()
        {
            if (PointerEventArgs.Pointer.IsInContact && _selected.Count > 0)
            {
                Point currentPoint = PointerEventArgs.GetCurrentPoint(Canvas).Position;
                Point difference = new Point(currentPoint.X - _prevPointer.X, currentPoint.Y - _prevPointer.Y);
                _prevPointer = currentPoint;

                foreach (PaintBase paintBase in _selected)
                {
                    paintBase.Accept(new ScaleVisitor(difference.X, difference.Y));
                }

                _page.Draw();
            }
        }
    }
}
