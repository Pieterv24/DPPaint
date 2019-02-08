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
    public interface ICanvasCommand
    {
        PointerRoutedEventArgs PointerEventArgs { get; set; }
        Canvas Canvas { get; set; }

        Stack<List<PaintBase>> UndoStack { get; set; }
        Stack<List<PaintBase>> RedoStack { get; set; }
        List<PaintBase> ShapeList { get; set; }

        void PointerPressedExecute();

        void PointerReleasedExecute();

        void PointerMovedExecute();
    }
}
