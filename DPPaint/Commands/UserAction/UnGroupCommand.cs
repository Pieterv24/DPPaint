using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            List<PaintBase> selected = ShapeList.Where(pb => pb.Selected && pb is PaintGroup).ToList();

            if (selected.Count > 0)
            {
                _page.AddUndoEntry();

                foreach (PaintBase paintBase in selected)
                {
                    if (paintBase is PaintGroup group)
                    {
                        ShapeList.Remove(group);
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
