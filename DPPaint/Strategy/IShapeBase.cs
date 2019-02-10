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
        /// <summary>
        /// Create shape to draw on canvas, based on strategy and supplied paintbase
        /// </summary>
        /// <param name="paintBase"></param>
        /// <returns>Shape</returns>
        Shape GetDrawShape(PaintBase paintBase);

        /// <summary>
        /// Returns type of shape as string
        /// </summary>
        /// <returns>Type of shape</returns>
        string ToString();
    }
}
