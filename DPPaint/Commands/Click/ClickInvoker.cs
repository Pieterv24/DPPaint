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

        public void InvokePointerPressed(ICanvasCommand cmd)
        {
            cmd.PointerPressedExecute();
        }

        public void InvokePointerReleased(ICanvasCommand cmd)
        {
            cmd.PointerReleasedExecute();
        }

        public void InvokePointerMoved(ICanvasCommand cmd)
        {
            cmd.PointerMovedExecute();
        }
    }
}
