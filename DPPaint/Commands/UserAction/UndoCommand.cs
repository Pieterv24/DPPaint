using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPPaint.Extensions;
using DPPaint.Shapes;

namespace DPPaint.Commands.UserAction
{
    /// <summary>
    /// This class handles undo user entries
    /// </summary>
    public class UndoCommand : IUserActionCommand
    {
        /// <inheritdoc />
        public List<PaintBase> ShapeList { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> UndoStack { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> RedoStack { get; set; }

        private readonly ICanvasPage _page;

        public UndoCommand(ICanvasPage page)
        {
            _page = page;
        }

        /// <inheritdoc />
        public void ExecuteUserAction()
        {
            ExecuteUserActionAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public Task ExecuteUserActionAsync()
        {
            if (UndoStack.Count > 0)
            {
                List<PaintBase> newState = UndoStack.Pop();
                RedoStack.Push(ShapeList.DeepCopy());
                
                ShapeList.Clear();
                ShapeList.AddRange(newState);

                _page.Draw();
                _page.UpdateList();
            }

            return Task.CompletedTask;
        }
    }
}
