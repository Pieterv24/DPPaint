using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DPPaint.Shapes;

namespace DPPaint.Commands.UserAction
{
    public class GroupCommand : IUserActionCommand
    {
        public List<PaintBase> ShapeList { get; set; }
        public Stack<List<PaintBase>> UndoStack { get; set; }
        public Stack<List<PaintBase>> RedoStack { get; set; }

        private ICanvasPage _page;

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
            List<PaintBase> selected = ShapeList.Where(pb => pb.Selected).ToList();

            if (selected.Count > 1)
            {
                _page.AddUndoEntry();

                PaintGroup newGroup = new PaintGroup()
                {
                    Selected = true
                };
                foreach (PaintBase paintBase in selected)
                {
                    ShapeList.Remove(paintBase);
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
