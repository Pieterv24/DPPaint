using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using DPPaint.Shapes;

namespace DPPaint.Commands.Click
{
    public class ScaleCommand : ICanvasCommand
    {
        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        public Canvas Canvas { get; set; }
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
                    double currentWidth = paintBase.Width * paintBase.Scale.X;
                    double currentHeight = paintBase.Height * paintBase.Scale.Y;

                    double newWidth = currentWidth + difference.X;
                    double newHeight = currentHeight + difference.Y;

                    var newScale = new Vector3((float)(newWidth / paintBase.Width), (float)(newHeight / paintBase.Height), 1f);

                    paintBase.Scale = newScale;
                }

                _page.Draw();
            }
        }
    }
}
