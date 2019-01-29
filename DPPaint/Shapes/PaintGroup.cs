using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPPaint.Shapes
{
    public class PaintGroup : PaintBase
    {
        public PaintGroup()
        {

        }

        public PaintGroup(PaintGroup group) : base(group)
        {
            if (group.Children.Count > 0)
            {
                Children = group.Children.Select(child =>
                {
                    if (child is PaintShape shape)
                    {
                        return new PaintShape(shape);
                    } else if (child is PaintGroup grp)
                    {
                        return new PaintGroup(grp);
                    }
                    else
                    {
                        return new PaintBase(child);
                    }
                }).ToList();
            }
        }

        public List<PaintBase> Children { get; set; } = new List<PaintBase>();
    }
}
