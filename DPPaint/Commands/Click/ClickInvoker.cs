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
    public class ClickInvoker
    {
        public ClickInvoker()
        {
        }

        public async Task InvokePointerPressedAsync(ICanvasCommand cmd)
        {
            await cmd.PointerPressedExecuteAsync();
        }

        public async Task InvokePointerReleasedAsync(ICanvasCommand cmd)
        {
            await cmd.PointerReleasedExecuteAsync();
        }

        public async Task InvokePointerMovedAsync(ICanvasCommand cmd)
        {
            await cmd.PointerMovedExecuteAsync();
        }
    }
}
