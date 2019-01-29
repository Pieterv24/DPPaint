using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPPaint.Shapes;

namespace DPPaint
{
    public interface ICanvasPage
    {
        void Draw();
        void UpdateList();
        void AddUndoEntry();
        void ClearMemory();
    }
}
