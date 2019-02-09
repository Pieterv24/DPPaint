using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using DPPaint.Shapes;

namespace DPPaint.Commands.Click
{
    public interface ICanvasCommand
    {
        #region Properties

        /// <summary>
        /// Event arguments for mouse action
        /// </summary>
        PointerRoutedEventArgs PointerEventArgs { get; set; }
        /// <summary>
        /// Context to canvas for calculations
        /// </summary>
        Canvas Canvas { get; set; }
        /// <summary>
        /// Stack with undo items, contains deep copies of <the cref="ShapeList"/>
        /// </summary>
        Stack<List<PaintBase>> UndoStack { get; set; }
        /// <summary>
        /// Stack with redo items, contains deep copies of <the cref="ShapeList"/>
        /// </summary>
        Stack<List<PaintBase>> RedoStack { get; set; }
        /// <summary>
        /// List of items currently on the Canvas
        /// </summary>
        List<PaintBase> ShapeList { get; set; }

        #endregion

        /// <summary>
        /// Command pattern entry for mouse down event
        /// </summary>
        /// <returns></returns>
        Task PointerPressedExecuteAsync();

        /// <summary>
        /// Command pattern entry for mouse up event
        /// </summary>
        /// <returns></returns>
        Task PointerReleasedExecuteAsync();

        /// <summary>
        /// Command pattern entry for mouse move event
        /// </summary>
        /// <returns></returns>
        Task PointerMovedExecuteAsync();
    }
}
