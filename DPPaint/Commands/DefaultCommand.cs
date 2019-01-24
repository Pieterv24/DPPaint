using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;

namespace DPPaint.Commands
{
    public class DefaultCommand : ICanvasCommand
    {
        public PointerRoutedEventArgs PointerEventArgs { get; set; }

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
