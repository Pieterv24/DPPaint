using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPPaint.Decorators;
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
            JObject masterJObject = new JObject();
            JArray decorators = null;
            if (element is TextDecoration decoration)
            {
                decorators = getDecoratorArray(decoration);
                element = decoration.GetDrawable();
            }

            if (element is PaintShape shape)
            {
                masterJObject = new JObject
                {
                    {"type", (int)PaintType.Shape},
                    { "shapeType", shape.ToString()}
                };
                masterJObject.Merge(getBaseJObject(shape));
            } else if (element is PaintGroup group)
            {
                masterJObject = new JObject
                {
                    { "type", (int)PaintType.Group }
                };
                masterJObject.Merge(getBaseJObject(group));

                JArray children = new JArray();

                foreach (PaintBase paintBase in group.Children)
                {
                    paintBase.Accept(new WriteFileVisitor(children));
                }

                masterJObject.Add("children", children);
            }

            if (decorators != null)
            {
                masterJObject.Add("decorators", decorators);
            }

            _jParent.Add(masterJObject);
        }

        private JObject getBaseJObject(PaintBase paintBase)
        {
            return new JObject
            {
                { "width", paintBase.Width },
                { "height", paintBase.Height },
                { "x", paintBase.X },
                { "y", paintBase.Y }
            };
        }

        private JArray getDecoratorArray(TextDecoration decoration)
        {
            JArray decoratorArray = new JArray();
            return getDecoratorArray(decoration, decoratorArray);
        }

        private JArray getDecoratorArray(TextDecoration decoration, JArray array)
        {
            array.Add(new JObject
            {
                {"position", decoration.GetDecoratorPosition().ToString() },
                {"decoration", decoration.DecorationText }
            });

            if (decoration.InnerPaintBase is TextDecoration decor)
            {
                return getDecoratorArray(decor, array);
            }

            return array;
        }
    }
}
