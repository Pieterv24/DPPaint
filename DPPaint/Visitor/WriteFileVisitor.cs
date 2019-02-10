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
    /// <summary>
    /// Visitor for creating json of the visited item
    /// </summary>
    public class WriteFileVisitor : IVisitor
    {
        private readonly JArray _jParent;

        /// <summary>
        /// Add visited item to supplied array
        /// </summary>
        /// <param name="jParent">Array to add json object to</param>
        public WriteFileVisitor(JArray jParent)
        {
            _jParent = jParent;
        }

        /// <summary>
        /// Crate json object for visited element
        /// If visited element is a group,
        /// Visit its children recursively
        /// </summary>
        /// <param name="element">Element to visit</param>
        public void Visit(PaintBase element)
        {
            // Create JObject for visited element
            JObject masterJObject = new JObject();
            // Array of decorators
            JArray decorators = null;

            // If visited element is decoration, add them to jArray
            if (element is TextDecoration decoration)
            {
                decorators = GetDecoratorArray(decoration);
                element = decoration.GetDrawable();
            }

            if (element is PaintShape shape)
            {
                // Convert PaintShape to JObject
                masterJObject = new JObject
                {
                    {"type", (int)PaintType.Shape},
                    { "shapeType", shape.ToString()}
                };
                masterJObject.Merge(getBaseJObject(shape));
            } else if (element is PaintGroup group)
            {
                // Convert PaintGroup to JObject and add it's children recursively
                masterJObject = new JObject
                {
                    { "type", (int)PaintType.Group }
                };
                masterJObject.Merge(getBaseJObject(group));

                // Add children
                JArray children = new JArray();

                foreach (PaintBase paintBase in group.Children)
                {
                    paintBase.Accept(new WriteFileVisitor(children));
                }

                masterJObject.Add("children", children);
            }

            // If decorators are found, add them to JObject
            if (decorators != null)
            {
                masterJObject.Add("decorators", decorators);
            }

            _jParent.Add(masterJObject);
        }

        /// <summary>
        /// Convert PaintBase into jsonObject
        /// </summary>
        /// <param name="paintBase"></param>
        /// <returns>JSON object of PaintBase</returns>
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

        /// <summary>
        /// Convert decorations into JArray
        /// </summary>
        /// <param name="decoration"></param>
        /// <returns>JArray with decorations</returns>
        private JArray GetDecoratorArray(TextDecoration decoration)
        {
            JArray decoratorArray = new JArray();
            return GetDecoratorArray(decoration, decoratorArray);
        }

        /// <summary>
        /// Add decorations recursively into JArray
        /// </summary>
        /// <param name="decoration"></param>
        /// <param name="array">Array to add decrorator to</param>
        /// <returns>JArray with decorations</returns>
        private JArray GetDecoratorArray(TextDecoration decoration, JArray array)
        {
            array.Add(new JObject
            {
                {"position", decoration.GetDecoratorPosition().ToString() },
                {"decoration", decoration.DecorationText }
            });

            if (decoration.InnerPaintBase is TextDecoration decor)
            {
                return GetDecoratorArray(decor, array);
            }

            return array;
        }
    }
}
