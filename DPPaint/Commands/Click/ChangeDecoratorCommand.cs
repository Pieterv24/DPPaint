using DPPaint.Decorators;
using DPPaint.Dialogs;
using DPPaint.Extensions;
using DPPaint.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace DPPaint.Commands.Click
{
    /// <summary>
    /// This command handles a click event and processes it to:
    /// add, edit or remove a decorator on one of the elements on the canvas
    /// </summary>
    public class ChangeDecoratorCommand : ICanvasCommand
    {
        #region Properties
        /// <inheritdoc />
        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        /// <inheritdoc />
        public Canvas Canvas { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> UndoStack { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> RedoStack { get; set; }
        /// <inheritdoc />
        public List<PaintBase> ShapeList { get; set; }

        #endregion

        #region Private variables

        private readonly ICanvasPage _page;

        #endregion

        /// <summary>
        /// Create ChangeDecoratorCommand
        /// </summary>
        /// <param name="page">Link to canvas page</param>
        public ChangeDecoratorCommand(ICanvasPage page)
        {
            _page = page;
        }

        #region Command pattern entry

        /// <inheritdoc />
        public async Task PointerPressedExecuteAsync()
        {
            // Check where the click was executed
            Point pointer = PointerEventArgs.GetCurrentPoint(Canvas).Position;

            // Querry a list of selected items
            List<PaintBase> selected = ShapeList.Where(pb => pb.Selected).ToList();

            // Check
            // if only one item is selected
            if (selected.Count == 1)
            {
                PaintBase paintBase = selected.First();

                // Check if the selected item is a decorator
                if (paintBase is TextDecoration decoration)
                {
                    // Check if a decorator was clicked
                    TextDecoration deco = decoration.GetClickedDecoration(pointer.X, pointer.Y);
                    if (deco != null)
                    {
                        // Create dialog for Decorator editing
                        DecoratorDialog dialog =
                            new DecoratorDialog(deco.DecorationText, deco.GetDecoratorPosition());

                        // When editing add a delete button
                        dialog.SecondaryButtonText = "Delete";

                        ContentDialogResult result = await dialog.ShowAsync();
                        if (result == ContentDialogResult.Primary)
                        {
                            deco.DecorationText = dialog.Decoration;

                            AddUndoEntry();

                            // Check if decorator should be moved
                            if (dialog.Position != GetDecoratorPosition(deco))
                            {
                                TextDecoration newDecoration = decoration.MovePosition(deco, dialog.Position);

                                ReplaceShapelistEntry(decoration, newDecoration);
                            }

                            _page.Draw();
                            _page.UpdateList();

                            // Return to prevent 2 decorations in one action
                            return;
                        } else if (result == ContentDialogResult.Secondary)
                        {
                            AddUndoEntry();

                            PaintBase newElement = decoration.RemoveDecorator(deco);
                            ReplaceShapelistEntry(decoration, newElement);

                            _page.Draw();
                            _page.UpdateList();
                        }
                    }
                }

                // when the item itself is clicked, add a new decorator
                if ((pointer.X > paintBase.X && pointer.X < paintBase.X + paintBase.Width) &&
                    (pointer.Y > paintBase.Y && pointer.Y < paintBase.Y + paintBase.Height))
                {
                    AddUndoEntry();

                    await AddNewDecorator(paintBase);
                    _page.Draw();
                    _page.UpdateList();
                }
            }
            else
            {
                // Show error
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Failed adding decorator",
                    Content = "You may only select one item to add a decorator to",
                    CloseButtonText = "Ok"
                };

                await dialog.ShowAsync();
            }
        }

        /// <inheritdoc />
        public Task PointerReleasedExecuteAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PointerMovedExecuteAsync()
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Get the position of the decoration
        /// </summary>
        /// <param name="decoration">Textdecoration to check</param>
        /// <returns>Position of the decoration</returns>
        private DecoratorAnchor GetDecoratorPosition(TextDecoration decoration)
        {
            if (decoration.GetType() == typeof(TopDecoration)) return DecoratorAnchor.Top;
            if (decoration.GetType() == typeof(BottomDecoration)) return DecoratorAnchor.Bottom;
            if (decoration.GetType() == typeof(LeftDecoration)) return DecoratorAnchor.Left;
            if (decoration.GetType() == typeof(RightDecoration)) return DecoratorAnchor.Right;

            return DecoratorAnchor.Top;
        }

        /// <summary>
        /// Create a new decorator instance
        /// </summary>
        /// <param name="decoratable">Item to be decorated</param>
        /// <param name="decoration">Decoration text</param>
        /// <param name="position">Position of the decoration</param>
        /// <returns>Decorated instance</returns>
        private TextDecoration CreateNewTextDecoration(PaintBase decoratable, string decoration,
            DecoratorAnchor position)
        {
            switch (position)
            {
                case DecoratorAnchor.Top:
                    return new TopDecoration(decoratable, decoration);
                case DecoratorAnchor.Bottom:
                    return new BottomDecoration(decoratable, decoration);
                case DecoratorAnchor.Left:
                    return new LeftDecoration(decoratable, decoration);
                case DecoratorAnchor.Right:
                    return new RightDecoration(decoratable, decoration);
            }

            return null;
        }

        /// <summary>
        /// Replace a PaintBase item in the ShapeList
        /// </summary>
        /// <param name="current">Item to replace</param>
        /// <param name="newItem">Item to replace with</param>
        private void ReplaceShapelistEntry(PaintBase current, PaintBase newItem)
        {
            int currentIndex = ShapeList.IndexOf(current);
            ShapeList.Remove(current);
            ShapeList.Insert(currentIndex, newItem);
        }

        /// <summary>
        /// Add a new decorator to an element
        /// </summary>
        /// <param name="paintBase">element to be decorated</param>
        private async Task AddNewDecorator(PaintBase paintBase)
        {
            // Open dialog
            DecoratorDialog dialog = new DecoratorDialog();

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ReplaceShapelistEntry(paintBase, CreateNewTextDecoration(paintBase, dialog.Decoration, dialog.Position));
            }
        }

        /// <summary>
        /// Add undo action to the stack
        /// </summary>
        private void AddUndoEntry()
        {
            UndoStack.Push(ShapeList.DeepCopy());
            RedoStack.Clear();
        }

        #endregion
    }
}
