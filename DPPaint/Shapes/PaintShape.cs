using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using DPPaint.Visitor;
using Newtonsoft.Json.Linq;

namespace DPPaint.Shapes
{
    /// <summary>
    /// Leaf for composite pattern
    /// </summary>
    public class PaintShape : PaintBase
    {
        public ShapeType ShapeType { get; set; }

        public PaintShape()
        {

        }

        public PaintShape(PaintShape shape) : base(shape)
        {
            ShapeType = shape.ShapeType;
        }

        public override void Add(PaintBase c)
        {
        }

        public override void Remove(PaintBase c)
        {
        }

        public override string ToString()
        {
            return ToJObject().ToString();
        }

        public override JObject ToJObject()
        {
            JObject jObject = new JObject
            {
                {"type", (int)PaintType.Shape},
                { "shapeType", (int) ShapeType}
            };

            jObject.Merge(base.ToJObject());

            return jObject;
        }
    }
}
