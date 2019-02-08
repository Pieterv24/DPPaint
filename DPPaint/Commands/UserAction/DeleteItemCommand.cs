using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPPaint.Extensions;
using DPPaint.Shapes;

namespace DPPaint.Commands.UserAction
{
    public class DeleteItemCommand : IUserActionCommand
    {
        public List<PaintBase> ShapeList { get; set; }
        public Stack<List<PaintBase>> UndoStack { get; set; }
        public Stack<List<PaintBase>> RedoStack { get; set; }

        private readonly ICanvasPage _page;

        public DeleteItemCommand(ICanvasPage page)
        {
            _page = page;
        }

        public void ExecuteUserAction()
        {
            ExecuteUserActionAsync().GetAwaiter().GetResult();
        }

        public Task ExecuteUserActionAsync()
        {
            UndoStack.Push(ShapeList.DeepCopy());
            RedoStack.Clear();

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
