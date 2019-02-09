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
    public class UnGroupCommand : IUserActionCommand
    {
        public List<PaintBase> ShapeList { get; set; }
        public Stack<List<PaintBase>> UndoStack { get; set; }
        public Stack<List<PaintBase>> RedoStack { get; set; }

        private readonly ICanvasPage _page;

        public UnGroupCommand(ICanvasPage page)
        {
            _page = page;
        }

        public void ExecuteUserAction()
        {
            ExecuteUserActionAsync().GetAwaiter().GetResult();
        }

        public Task ExecuteUserActionAsync()
        {
            List<PaintBase> selected = ShapeList.Where(pb => pb.Selected && 
                                                             !(pb is PaintShape) &&
                                                             (pb is PaintGroup || (pb as TextDecoration).InnerPaintBase is PaintGroup)).ToList();

            if (selected.Count > 0)
            {
                UndoStack.Push(ShapeList.DeepCopy());
                RedoStack.Clear();

                foreach (PaintBase paintBase in selected)
                {
                    PaintBase element = paintBase;
                    if (element is TextDecoration decor)
                    {
                        element = decor.GetDrawable();
                    }

                    if (element is PaintGroup group)
                    {
                        ShapeList.Remove(paintBase);
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
