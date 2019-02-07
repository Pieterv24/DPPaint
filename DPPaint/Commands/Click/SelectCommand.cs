using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using DPPaint.Shapes;

namespace DPPaint.Commands.Click
{
    public class SelectCommand : ICanvasCommand
    {
        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        public Canvas Canvas { get; set; }
        public List<PaintBase> ShapeList { get; set; }

        private readonly ICanvasPage _page;

        public SelectCommand(ICanvasPage page)
        {
            _page = page;
        }

        public void PointerPressedExecute()
        {
            Point pointerLocation = PointerEventArgs.GetCurrentPoint(Canvas).Position;
            foreach (PaintBase paintBase in ShapeList)
            {
                if ((pointerLocation.X > paintBase.X && pointerLocation.X < (paintBase.X + paintBase.Width)
                     && (pointerLocation.Y > paintBase.Y && pointerLocation.Y < (paintBase.Y + paintBase.Height))))
                {
                    paintBase.Selected = !paintBase.Selected;
                    _page.Draw();
                    _page.UpdateList();
                    break;
                }
            }
        }

        public void PointerReleasedExecute()
        {
        }

        public void PointerMovedExecute()
        {
        }
    }
}
