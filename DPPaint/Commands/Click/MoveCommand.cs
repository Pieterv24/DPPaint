using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using DPPaint.Shapes;

namespace DPPaint.Commands.Click
{
    public class MoveCommand : ICanvasCommand
    {
        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        public Canvas Canvas { get; set; }
        public List<PaintBase> ShapeList { get; set; }

        private readonly ICanvasPage _page;

        private Point _prevPointer;
        private List<PaintBase> _selected;

        public MoveCommand(ICanvasPage page)
        {
            _page = page;
        }

        public void PointerPressedExecute()
        {
            _page.AddUndoEntry();
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
                    double newX = (paintBase.X + difference.X);
                    double newY = (paintBase.Y + difference.Y);

                    if (newY >= 0)
                    {
                        paintBase.Y = newY;
                    }
                    else
                    {
                        paintBase.Y = 0;
                    }

                    if (newX >= 0)
                    {
                        paintBase.X = newX;
                    }
                    else
                    {
                        paintBase.X = 0;
                    }
                }
            }

            _page.Draw();
        }
    }
}
