using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;
using DPPaint.Shapes;

namespace DPPaint.Commands
{
    public interface ICanvasCommand
    {
        PointerRoutedEventArgs PointerEventArgs { get; set; }

        void PointerPressedExecute();

        void PointerReleasedExecute();

        void PointerMovedExecute();
    }
}
