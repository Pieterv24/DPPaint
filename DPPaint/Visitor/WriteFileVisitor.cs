using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPPaint.Shapes;
using Newtonsoft.Json.Linq;

namespace DPPaint.Visitor
{
    public class WriteFileVisitor : IVisitor
    {
        private readonly JArray _jParent;

        public WriteFileVisitor(JArray jParent)
        {
            _jParent = jParent;
        }

        public void Visit(PaintBase element)
        {
            if (element is PaintShape shape)
            {
                JObject jObject = new JObject
                {
                    {"type", (int)PaintType.Shape},
                    { "shapeType", shape.ToString()}
                };
                jObject.Merge(getBaseJObject(shape));

                _jParent.Add(jObject);
            } else if (element is PaintGroup group)
            {
                JObject jObject = new JObject
                {
                    { "type", (int)PaintType.Group }
                };
                jObject.Merge(getBaseJObject(group));

                JArray children = new JArray();

                foreach (PaintBase paintBase in group.Children)
                {
                    paintBase.Accept(new WriteFileVisitor(children));
                }

                jObject.Add("children", children);

                _jParent.Add(jObject);
            }
        }

        private JObject getBaseJObject(PaintBase paintBase)
        {
            return new JObject
            {
                { "width", paintBase.Width },
                { "height", paintBase.Height },
                { "x", paintBase.X },
                { "y", paintBase.Y },
                { "decoration", paintBase.Decoration },
                { "anchor", (int)paintBase.Anchor },
            };
        }
    }
}
