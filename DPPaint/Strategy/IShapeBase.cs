using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using DPPaint.Shapes;

namespace DPPaint.Strategy
{
    public interface IShapeBase
    {
        Shape GetDrawShape(PaintBase paintBase);
        ShapeType GetShapeType();
        string ToString();
    }
}
