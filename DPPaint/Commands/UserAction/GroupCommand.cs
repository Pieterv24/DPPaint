using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DPPaint.Decorators;
using DPPaint.Extensions;
using DPPaint.Shapes;
using DPPaint.Visitor;

namespace DPPaint.Commands.UserAction
{
    /// <summary>
    /// This command groups the selected items
    /// </summary>
    public class GroupCommand : IUserActionCommand
    {
        /// <inheritdoc />
        public List<PaintBase> ShapeList { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> UndoStack { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> RedoStack { get; set; }

        private readonly ICanvasPage _page;

        public GroupCommand(ICanvasPage page)
        {
            _page = page;
        }

        public void ExecuteUserAction()
        {
            ExecuteUserActionAsync().GetAwaiter().GetResult();
        }

        public Task ExecuteUserActionAsync()
        {
            // Get selected items from ShapeList
            List<PaintBase> selected = ShapeList.Where(pb => pb.Selected).ToList();

            // Only group if 2 or more items are selected
            if (selected.Count > 1)
            {
                // Add undo entry
                UndoStack.Push(ShapeList.DeepCopy());
                RedoStack.Clear();

                // Create new group
                PaintGroup newGroup = new PaintGroup()
                {
                    Selected = true
                };

                // Add selected items to the group
                // Remove selected items from the canvas itself
                foreach (PaintBase paintBase in selected)
                {
                    ShapeList.Remove(paintBase);
                    paintBase.Selected = false;

                    newGroup.Add(paintBase);
                }

                ShapeList.Add(newGroup);

                _page.Draw();
                _page.UpdateList();
            }

            return Task.CompletedTask;
        }
    }
}
