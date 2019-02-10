using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPPaint.Commands.UserAction
{
    /// <summary>
    /// Invoker for user actions
    /// </summary>
    public class UserActionInvoker
    {
        public void InvokeUserAction(IUserActionCommand cmd)
        {
            cmd.ExecuteUserAction();
        }

        public Task InvokeUserActionAsync(IUserActionCommand cmd)
        {
            return cmd.ExecuteUserActionAsync();
        }
    }
}
