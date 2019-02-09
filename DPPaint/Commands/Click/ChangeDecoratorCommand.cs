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
    public class ChangeDecoratorCommand : ICanvasCommand
    {
        #region Properties

        public PointerRoutedEventArgs PointerEventArgs { get; set; }
        public Canvas Canvas { get; set; }
        public Stack<List<PaintBase>> UndoStack { get; set; }
        public Stack<List<PaintBase>> RedoStack { get; set; }
        public List<PaintBase> ShapeList { get; set; }

        #endregion

        #region Private variables

        private readonly ICanvasPage _page;

        #endregion

        public ChangeDecoratorCommand(ICanvasPage page)
        {
            _page = page;
        }

        public async Task PointerPressedExecuteAsync()
        {
            Point pointer = PointerEventArgs.GetCurrentPoint(Canvas).Position;

            List<PaintBase> selected = ShapeList.Where(pb => pb.Selected).ToList();

            if (selected.Count == 1)
            {
                foreach (PaintBase paintBase in selected)
                {
                    if (paintBase is TextDecoration decoration)
                    {
                        TextDecoration deco = decoration.GetClickedDecoration(pointer.X, pointer.Y);
                        if (deco != null)
                        {
                            DecoratorDialog dialog =
                                new DecoratorDialog(deco.DecorationText, deco.GetDecoratorPosition());
                            dialog.SecondaryButtonText = "Delete";

                            ContentDialogResult result = await dialog.ShowAsync();
                            if (result == ContentDialogResult.Primary)
                            {
                                deco.DecorationText = dialog.Decoration;

                                UndoStack.Push(ShapeList.DeepCopy());
                                RedoStack.Clear();

                                if (dialog.Position != GetDecoratorPosition(deco))
                                {
                                    TextDecoration newDecoration = decoration.MovePosition(deco, dialog.Position);

                                    int currentIndex = ShapeList.IndexOf(decoration);
                                    ShapeList.Remove(decoration);
                                    ShapeList.Insert(currentIndex, newDecoration);
                                }

                                _page.Draw();
                                _page.UpdateList();

                                // Return to prevent 2 decorations in one action
                                continue;
                            } else if (result == ContentDialogResult.Secondary)
                            {
                                PaintBase newElement = decoration.RemoveDecorator(deco);

                                int currentIndex = ShapeList.IndexOf(decoration);
                                ShapeList.Remove(decoration);
                                ShapeList.Insert(currentIndex, newElement);

                                _page.Draw();
                                _page.UpdateList();
                            }
                        }
                    }

                    if ((pointer.X > paintBase.X && pointer.X < paintBase.X + paintBase.Width) &&
                        (pointer.Y > paintBase.Y && pointer.Y < paintBase.Y + paintBase.Height))
                    {
                        UndoStack.Push(ShapeList.DeepCopy());
                        RedoStack.Clear();

                        await AddNewDecorator(paintBase);
                        _page.Draw();
                        _page.UpdateList();
                    }
                }
            }
            else
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Failed adding decorator",
                    Content = "You may only select one item to add a decorator to",
                    CloseButtonText = "Ok"
                };

                await dialog.ShowAsync();
            }
        }

        public Task PointerReleasedExecuteAsync()
        {
            return Task.CompletedTask;
        }

        public Task PointerMovedExecuteAsync()
        {
            return Task.CompletedTask;
        }

        private DecoratorAnchor GetDecoratorPosition(TextDecoration decoration)
        {
            if (decoration.GetType() == typeof(TopDecoration)) return DecoratorAnchor.Top;
            if (decoration.GetType() == typeof(BottomDecoration)) return DecoratorAnchor.Bottom;
            if (decoration.GetType() == typeof(LeftDecoration)) return DecoratorAnchor.Left;
            if (decoration.GetType() == typeof(RightDecoration)) return DecoratorAnchor.Right;

            return DecoratorAnchor.Top;
        }

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

        private void ReplaceShapelistEntry(PaintBase current, PaintBase newItem)
        {
            int currentIndex = ShapeList.IndexOf(current);
            ShapeList.Remove(current);
            ShapeList.Insert(currentIndex, newItem);
        }

        private async Task AddNewDecorator(PaintBase paintBase)
        {
            DecoratorDialog dialog = new DecoratorDialog();

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ReplaceShapelistEntry(paintBase, CreateNewTextDecoration(paintBase, dialog.Decoration, dialog.Position));
            }
        }
    }
}
