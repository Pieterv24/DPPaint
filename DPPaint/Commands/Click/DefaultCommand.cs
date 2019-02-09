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
    /// <summary>
    /// Default placeholder command, does nothing
    /// </summary>
    public class DefaultCommand : ICanvasCommand
    {
        /// <inheritdoc />
        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        /// <inheritdoc />
        public Canvas Canvas { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> UndoStack { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> RedoStack { get; set; }
        /// <inheritdoc />
        public List<PaintBase> ShapeList { get; set; }

        /// <inheritdoc />
        public Task PointerPressedExecuteAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PointerReleasedExecuteAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PointerMovedExecuteAsync()
        {
            return Task.CompletedTask;
        }
    }
}
