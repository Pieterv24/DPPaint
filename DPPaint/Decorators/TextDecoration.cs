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

        /// <summary>
        /// Read only access to the decorated instance
        /// </summary>
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

        #region Inherited abstract impelmentations

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

        #endregion

        #region Methods to implement by decorators
        /// <summary>
        /// Check if click is made on the location of the decoration.
        /// Recurcively searches if the TextDecoration of the TextDecoration of a child was clicked
        /// </summary>
        /// <param name="clickX">X coordinate of the click</param>
        /// <param name="clickY">Y coordinate of the click</param>
        /// <returns>Return TextDecoration that was clicked, null if none found</returns>
        public abstract TextDecoration GetClickedDecoration(double clickX, double clickY);
        public abstract DecoratorAnchor GetDecoratorPosition();

        #endregion

        #region Common decorator methods

        /// <summary>
        /// Get undecorated PaintBase
        /// </summary>
        /// <returns>Undecorated PaintBase</returns>
        public PaintBase GetDrawable()
        {
            if (_paintBase is TextDecoration decoration)
            {
                return decoration.GetDrawable();
            }

            return _paintBase;
        }

        /// <summary>
        /// Create deep copy of decorated element (recursively when needed)
        /// </summary>
        /// <returns>Deep copy of instance</returns>
        public TextDecoration DeepCopy()
        {
            // Get the type of instance
            Type thisType = this.GetType();

            // If decorated instance is also decorated, recursively create a deep copy of that aswell
            if (_paintBase is TextDecoration decoration)
            {
                // Create new object of the same type of the current instance
                // Would be the same as new T(decoratable, decorationText); where T is an implementation of TextDecoration
                TextDecoration td = (TextDecoration) Activator.CreateInstance(thisType, decoration.DeepCopy(), DecorationText);
                return td;
            }
            else
            {
                // Create new instance of decorated object
                Type paintBaseType = _paintBase.GetType();
                PaintBase paintBase = (PaintBase) Activator.CreateInstance(paintBaseType, _paintBase);

                // Create new object of the same type of the current instance
                // Would be the same as new T(decoratable, decorationText); where T is an implementation of TextDecoration
                TextDecoration td = (TextDecoration) Activator.CreateInstance(thisType, paintBase, DecorationText);
                return td;
            }
        }

        /// <summary>
        /// Removes decorator from a decorated object.
        /// If the decorator is not the current instantiated object.
        /// Find decorator to remove recursively.
        /// Does nothing if specified decorator is not found
        /// </summary>
        /// <param name="decoration">Decorator to remove</param>
        /// <returns>Object in which specified decorator is removed(if found)</returns>
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
                _paintBase = innerDecoration.RemoveDecorator(decoration);
            }

            return this;
        }

        /// <summary>
        /// Change the position of a specified decoration
        /// If not this object, check recursively for inner decorator that matches
        /// </summary>
        /// <param name="original">Decorator that should be changed</param>
        /// <param name="newPosition">New position of decorator</param>
        /// <returns>Object where the specified decorator is changed to the new position</returns>
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

        /// <summary>
        /// Create new instance of object that implements TextDecoration based on the decorator anchor
        /// </summary>
        /// <param name="position">Position of TextDecoration to create</param>
        /// <param name="innerPaintBase">Item to decorate</param>
        /// <param name="decorationText">Decoration text</param>
        /// <returns>New instance of TextDecoration in correct position</returns>
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

        #endregion
    }
}
