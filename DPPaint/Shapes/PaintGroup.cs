using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using DPPaint.Decorators;
using DPPaint.Strategy;
using DPPaint.Visitor;
using Newtonsoft.Json.Linq;

namespace DPPaint.Shapes
{
    /// <summary>
    /// Composite
    /// </summary>
    public class PaintGroup : PaintBase
    {
        public override double Width { get; set; }
        public override double Height { get; set; }
        public override double X { get; set; }
        public override double Y { get; set; }
        public override bool Selected { get; set; }

        // Set public Get Property to enable read only access of children
        public IReadOnlyCollection<PaintBase> Children => _children;


        private readonly List<PaintBase> _children;

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
                else if (child is TextDecoration decoration)
                {
                    _children.Add(decoration.DeepCopy());
                }
            }
        }

        public override void Add(PaintBase c)
        {
            _children.Add(c);

            RecalculateDimensions();
        }

        public override void Remove(PaintBase c)
        {
            _children.Remove(c);

            RecalculateDimensions();
        }

        public override void DrawOnCanvas(Canvas canvas)
        {
            if (Selected) base.DrawSelector(canvas);

            foreach (PaintBase paintBase in Children)
            {
                paintBase.DrawOnCanvas(canvas);
            }
        }

        private void RecalculateDimensions()
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (PaintBase paintBase in _children)
            {
                minX = paintBase.X < minX ? paintBase.X : minX;
                minY = paintBase.Y < minY ? paintBase.Y : minY;

                maxX = paintBase.X + paintBase.Width > maxX ? paintBase.X + paintBase.Width : maxX;
                maxY = paintBase.Y + paintBase.Height > maxY ? paintBase.Y + paintBase.Height : maxY;
            }

            X = minX;
            Y = minY;
            Width = maxX - minX;
            Height = maxY - minY;
        }
    }
}
