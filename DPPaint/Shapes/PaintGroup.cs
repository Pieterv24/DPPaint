using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DPPaint.Shapes
{
    /// <summary>
    /// Composite
    /// </summary>
    public class PaintGroup : PaintBase
    {
        private readonly List<PaintBase> _children;

        // Set public Get Property to enable read only access of children
        public IReadOnlyCollection<PaintBase> Children => _children;

        public PaintGroup()
        {
            _children = new List<PaintBase>();
        }

        /// <summary>
        /// Constructor recursively generates a deep copy of the group
        /// </summary>
        /// <param name="group">Group to create deep copy of</param>
        public PaintGroup(PaintGroup group) : base(group)
        {
            _children = new List<PaintBase>();
            foreach (PaintBase child in group.Children)
            {
                if (child is PaintShape shape)
                {
                    _children.Add(new PaintShape(shape));
                }
                else if (child is PaintGroup grp)
                {
                    _children.Add(new PaintGroup(grp));
                }
            }
        }

        public override void Add(PaintBase c)
        {
            _children.Add(c);
        }

        public override void Remove(PaintBase c)
        {
            _children.Remove(c);
        }

        public override string ToString()
        {
            return ToJObject().ToString();
        }

        public override JObject ToJObject()
        {
            JObject jObject = new JObject
            {
                { "type", (int)PaintType.Group }
            };
            jObject.Merge(base.ToJObject());

            JArray children = new JArray();
            foreach (PaintBase paintBase in _children)
            {
                children.Add(paintBase.ToJObject());
            }

            jObject.Add("children", children);

            return jObject;
        }
    }
}
