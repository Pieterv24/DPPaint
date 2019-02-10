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
    /// This class handles the redo command.
    /// When the command is ran, the prior state(if any)
    /// Will be pushed onto the current state
    /// </summary>
    public class RedoCommand : IUserActionCommand
    {
        /// <inheritdoc />
        public List<PaintBase> ShapeList { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> UndoStack { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> RedoStack { get; set; }

        private readonly ICanvasPage _page;

        public RedoCommand(ICanvasPage page)
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
            if (RedoStack.Count > 0)
            {
                // Pop prior state from stack
                List<PaintBase> newState = RedoStack.Pop();
                // Push current state onto the undo stack
                UndoStack.Push(ShapeList.DeepCopy());

                // Add prior state as current state
                ShapeList.Clear();
                ShapeList.AddRange(newState);

                _page.Draw();
                _page.UpdateList();
            }

            return Task.CompletedTask;
        }
    }
}
