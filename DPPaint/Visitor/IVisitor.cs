using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPPaint.Shapes;

namespace DPPaint.Visitor
{
    public interface IVisitor
    {
        void Visit(PaintBase element);
    }
}
