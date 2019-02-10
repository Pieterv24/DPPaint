using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DPPaint.Decorators;
using DPPaint.Extensions;
using DPPaint.Shapes;

namespace DPPaint.Commands.UserAction
{
    /// <summary>
    /// This class handles the un grouping of elements on the canvas
    /// </summary>
    public class UnGroupCommand : IUserActionCommand
    {
        /// <inheritdoc />
        public List<PaintBase> ShapeList { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> UndoStack { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> RedoStack { get; set; }

        private readonly ICanvasPage _page;

        public UnGroupCommand(ICanvasPage page)
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
            // Get all selected elements that are of the type PaintGroup
            List<PaintBase> selected = ShapeList.Where(pb => pb.Selected && 
                                                             !(pb is PaintShape) &&
                                                             (pb is PaintGroup || (pb as TextDecoration).InnerPaintBase is PaintGroup)).ToList();

            // Only run if 1 or more groups are selected
            if (selected.Count > 0)
            {
                // Add undo entry to the undo stack
                UndoStack.Push(ShapeList.DeepCopy());
                RedoStack.Clear();

                foreach (PaintBase paintBase in selected)
                {
                    PaintBase element = paintBase;
                    // Check if element is wrapped in a decorator
                    if (element is TextDecoration decor)
                    {
                        // If so, get inner group
                        element = decor.GetDrawable();
                    }

                    if (element is PaintGroup group)
                    {
                        // Remove group from canvas
                        ShapeList.Remove(paintBase);

                        // Add the groups children back onto the canvas
                        foreach (PaintBase groupChild in group.Children.ToList())
                        {
                            group.Remove(groupChild);
                            ShapeList.Add(groupChild);
                        }
                    }
                }

                _page.Draw();
                _page.UpdateList();
            }

            return Task.CompletedTask;
        }
    }
}
