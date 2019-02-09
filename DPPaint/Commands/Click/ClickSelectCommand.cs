using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using DPPaint.Decorators;
using DPPaint.Shapes;

namespace DPPaint.Commands.Click
{
    /// <summary>
    /// This command handles the incoming pointer events to select a single element by clicking it on the canvas
    /// </summary>
    public class ClickSelectCommand : ICanvasCommand
    {
        #region Properties

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

        #endregion

        private readonly ICanvasPage _page;

        public ClickSelectCommand(ICanvasPage page)
        {
            _page = page;
        }

        #region Command actions

        /// <inheritdoc />
        public Task PointerPressedExecuteAsync()
        {
            Point pointer = PointerEventArgs.GetCurrentPoint(Canvas).Position;

            foreach (PaintBase paintBase in ShapeList)
            {
                // Check if the pointer location is withing the paintbase
                if (((pointer.X > paintBase.X) && (pointer.X < paintBase.X + paintBase.Width)) &&
                    ((pointer.Y > paintBase.Y) && (pointer.Y < paintBase.Y + paintBase.Height)))
                {
                    // update selection
                    paintBase.Selected = !paintBase.Selected;

                    break;
                }
            }

            _page.UpdateList();
            _page.Draw();

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

        #endregion
    }
}
