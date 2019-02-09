using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using DPPaint.Dialogs;
using DPPaint.Shapes;

namespace DPPaint.Decorators
{
    /// <summary>
    /// Decorator class
    /// All methods of this class are propagated down to the internal PaintBase,
    /// Which is set in the constructor 
    /// </summary>
    public abstract class TextDecoration : PaintBase
    {
        protected PaintBase _paintBase;

        /// <summary>
        /// Make changes to the PaintBase Properties propagate down to the inner PaintBase
        /// </summary>
        #region PaintBase Properties

        public override double Width
        {
            get => _paintBase.Width;
            set => _paintBase.Width = value;
        }

        public override double Height
        {
            get => _paintBase.Height;
            set => _paintBase.Height = value;
        }

        public override double X
        {
            get => _paintBase.X;
            set => _paintBase.X = value;
        }

        public override double Y
        {
            get => _paintBase.Y;
            set => _paintBase.Y = value;
        }

        public override bool Selected
        {
            get => _paintBase.Selected;
            set => _paintBase.Selected = value;
        }

        #endregion

        public string DecorationText { get; set; }
        public PaintBase InnerPaintBase => _paintBase;

        /// <summary>
        /// Create an instance of TextDecoration following the Decorator pattern
        /// </summary>
        /// <param name="paintBase">PaintBase to use as internal object</param>
        protected TextDecoration(PaintBase paintBase)
        {
            _paintBase = paintBase;
        }

        protected TextDecoration(PaintBase paintBase, string decorationText)
        {
            _paintBase = paintBase;
            DecorationText = decorationText;
        } 

        public abstract TextDecoration GetClickedDecoration(double clickX, double clickY);
        public abstract DecoratorAnchor GetDecoratorPosition();

        /// <summary>
        /// Propagate Add Method to the internal PaintBase
        /// </summary>
        /// <param name="c">PaintBase to add to composite structure</param>
        public override void Add(PaintBase c)
        {
            _paintBase.Add(c);
        }

        /// <summary>
        /// Propagate Remove Method to the internal PaintBase
        /// </summary>
        /// <param name="c">PaintBase to remove from composite structure</param>
        public override void Remove(PaintBase c)
        {
            _paintBase.Remove(c);
        }

        /// <summary>
        /// Run the DrawOnCanvas Method of the internal PaintBase
        /// </summary>
        /// <param name="canvas">Canvas the PaintBase should draw on</param>
        public override void DrawOnCanvas(Canvas canvas)
        {
            _paintBase.DrawOnCanvas(canvas);
        }

        public PaintBase GetDrawable()
        {
            if (_paintBase is TextDecoration decoration)
            {
                return decoration.GetDrawable();
            }

            return _paintBase;
        }

        public TextDecoration DeepCopy()
        {
            Type thisType = this.GetType();

            if (_paintBase is TextDecoration decoration)
            {
                TextDecoration td = (TextDecoration) Activator.CreateInstance(thisType, decoration.DeepCopy(), DecorationText);
                return td;
            }
            else
            {
                Type paintBaseType = _paintBase.GetType();
                PaintBase paintBase = (PaintBase) Activator.CreateInstance(paintBaseType, _paintBase);

                TextDecoration td = (TextDecoration) Activator.CreateInstance(thisType, paintBase, DecorationText);
                return td;
            }
        }

        public PaintBase RemoveDecorator(TextDecoration decoration)
        {
            if (decoration == this)
            {
                return _paintBase;
            }

            if (_paintBase == decoration)
            {
                _paintBase = decoration.InnerPaintBase;
            }
            else if (_paintBase is TextDecoration innerDecoration)
            {
                return innerDecoration.RemoveDecorator(decoration);
            }

            return this;
        }

        public TextDecoration MovePosition(TextDecoration original, DecoratorAnchor newPosition)
        {
            if (original == this)
            {
                return CreateNewDecoration(newPosition, _paintBase, original.DecorationText);
            }

            if (_paintBase == original && _paintBase is TextDecoration decoration)
            {
                _paintBase = CreateNewDecoration(newPosition, decoration.InnerPaintBase, original.DecorationText);
            }
            else if (_paintBase is TextDecoration decor)
            {
                decor.MovePosition(original, newPosition);
            }

            return this;
        }

        private TextDecoration CreateNewDecoration(DecoratorAnchor position, PaintBase innerPaintBase, string decorationText)
        {
            TextDecoration newDecoration = null;

            switch (position)
            {
                case DecoratorAnchor.Top:
                    newDecoration = new TopDecoration(innerPaintBase, decorationText);
                    break;
                case DecoratorAnchor.Bottom:
                    newDecoration = new BottomDecoration(innerPaintBase, decorationText);
                    break;
                case DecoratorAnchor.Left:
                    newDecoration = new LeftDecoration(innerPaintBase, decorationText);
                    break;
                case DecoratorAnchor.Right:
                    newDecoration = new RightDecoration(innerPaintBase, decorationText);
                    break;
            }

            return newDecoration;
        }
    }
}
