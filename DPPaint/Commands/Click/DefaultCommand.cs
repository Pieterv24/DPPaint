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
        public Stack<List<PaintBase>> UndoStack { get; set; }
        public Stack<List<PaintBase>> RedoStack { get; set; }
        public List<PaintBase> ShapeList { get; set; }

        public Task PointerPressedExecuteAsync()
        {
            return Task.CompletedTask;
        }

        public Task PointerReleasedExecuteAsync()
        {
            return Task.CompletedTask;
        }

        public Task PointerMovedExecuteAsync()
        {
            return Task.CompletedTask;
        }
    }
}
