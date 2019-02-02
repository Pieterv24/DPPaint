using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPPaint.Shapes;

namespace DPPaint.Commands.UserAction
{
    public interface IUserActionCommand
    {
        List<PaintBase> ShapeList { get; set; }
        Stack<List<PaintBase>> UndoStack { get; set; }
        Stack<List<PaintBase>> RedoStack { get; set; }

        void ExecuteUserAction();

        Task ExecuteUserActionAsync();
    }
}
