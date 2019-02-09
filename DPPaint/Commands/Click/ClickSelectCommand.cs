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
    public class ClickSelectCommand : ICanvasCommand
    {
        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        public Canvas Canvas { get; set; }
        public Stack<List<PaintBase>> UndoStack { get; set; }
        public Stack<List<PaintBase>> RedoStack { get; set; }
        public List<PaintBase> ShapeList { get; set; }

        private readonly ICanvasPage _page;

        public ClickSelectCommand(ICanvasPage page)
        {
            _page = page;
        }

        public Task PointerPressedExecuteAsync()
        {
            Point pointer = PointerEventArgs.GetCurrentPoint(Canvas).Position;

            foreach (PaintBase paintBase in ShapeList)
            {
                if (((pointer.X > paintBase.X) && (pointer.X < paintBase.X + paintBase.Width)) &&
                    ((pointer.Y > paintBase.Y) && (pointer.Y < paintBase.Y + paintBase.Height)))
                {
                    paintBase.Selected = !paintBase.Selected;

                    break;
                }
            }

            _page.UpdateList();
            _page.Draw();

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
