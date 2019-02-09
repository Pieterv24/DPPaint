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
    /// This command handles the deletion of a element from the canvas
    /// </summary>
    public class DeleteItemCommand : IUserActionCommand
    {
        /// <inheritdoc />
        public List<PaintBase> ShapeList { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> UndoStack { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> RedoStack { get; set; }

        private readonly ICanvasPage _page;

        public DeleteItemCommand(ICanvasPage page)
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
            // Add undo entry
            UndoStack.Push(ShapeList.DeepCopy());
            RedoStack.Clear();

            // Get selected items
            List<PaintBase> selected = ShapeList.Where(pb => pb.Selected).ToList();

            foreach (PaintBase paintBase in selected)
            {
                ShapeList.Remove(paintBase);
            }

            _page.Draw();
            _page.UpdateList();

            return Task.CompletedTask;
        }
    }
}
