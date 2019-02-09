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
        /// <summary>
        /// List of items to be drawn on canvas
        /// </summary>
        List<PaintBase> ShapeList { get; set; }
        /// <summary>
        /// List of states to revert to on undo
        /// </summary>
        Stack<List<PaintBase>> UndoStack { get; set; }
        /// <summary>
        /// List of states to revert to on redo
        /// </summary>
        Stack<List<PaintBase>> RedoStack { get; set; }

        /// <summary>
        /// Execute command
        /// </summary>
        void ExecuteUserAction();

        /// <summary>
        /// Execute command async
        /// </summary>
        /// <returns></returns>
        Task ExecuteUserActionAsync();
    }
}
