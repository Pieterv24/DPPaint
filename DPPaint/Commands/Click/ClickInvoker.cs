using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;

namespace DPPaint.Commands.Click
{
    /// <summary>
    /// Command pattern invoker for click events
    /// This class invokes the click events
    /// </summary>
    public class ClickInvoker
    {
        public ClickInvoker()
        {
        }

        /// <summary>
        /// Invokes the command for pointer press
        /// </summary>
        /// <param name="cmd">Command</param>
        public async Task InvokePointerPressedAsync(ICanvasCommand cmd)
        {
            await cmd.PointerPressedExecuteAsync();
        }

        /// <summary>
        /// Invokes the command for pointer release
        /// </summary>
        /// <param name="cmd">Command</param>
        public async Task InvokePointerReleasedAsync(ICanvasCommand cmd)
        {
            await cmd.PointerReleasedExecuteAsync();
        }

        /// <summary>
        /// Invokes the command for pointer move
        /// </summary>
        /// <param name="cmd">Command</param>
        public async Task InvokePointerMovedAsync(ICanvasCommand cmd)
        {
            await cmd.PointerMovedExecuteAsync();
        }
    }
}
