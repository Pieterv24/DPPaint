using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using DPPaint.Shapes;

namespace DPPaint.Commands.Click
{
    public class DefaultCommand : ICanvasCommand
    {
        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        public Canvas Canvas { get; set; }
        public List<PaintBase> ShapeList { get; set; }

        public void PointerPressedExecute()
        {
        }

        public void PointerReleasedExecute()
        {
        }

        public void PointerMovedExecute()
        {
        }
    }
}
