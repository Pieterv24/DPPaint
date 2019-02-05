using DPPaint.Commands.Click;
using DPPaint.Extensions;
using DPPaint.Shapes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using DPPaint.Commands.UserAction;

namespace DPPaint
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, ICanvasPage
    {
        private ICanvasCommand _cmd;
        private readonly ClickInvoker _canvasInvoker;
        private readonly UserActionInvoker _userInvoker;

        private List<PaintBase> _shapeList;
        private Stack<List<PaintBase>> _undoStack;
        private Stack<List<PaintBase>> _redoStack;

        public MainPage()
        {
            this.InitializeComponent();
            _shapeList = new List<PaintBase>();
            _undoStack = new Stack<List<PaintBase>>();
            _redoStack = new Stack<List<PaintBase>>();
            CircleToggle.IsChecked = true;
            SetDrawCommand(ShapeType.Circle);
            _canvasInvoker = new ClickInvoker();
            _userInvoker = new UserActionInvoker();
            Canvas.SetZIndex(BottomPanel, 100);
            TopBar.OverflowButtonVisibility = CommandBarOverflowButtonVisibility.Collapsed;
        }

        private void BrushToggle_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarToggleButton button)
            {
                switch (button.Name)
                {
                    case "MoveToggle":
                        MoveToggle.IsChecked = true;
                        ScaleToggle.IsChecked = false;
                        CircleToggle.IsChecked = false;
                        RectangleToggle.IsChecked = false;
                        _cmd = new MoveCommand(this)
                        {
                            Canvas = Canvas,
                            ShapeList = _shapeList
                        };
                        break;
                    case "ScaleToggle":
                        MoveToggle.IsChecked = false;
                        ScaleToggle.IsChecked = true;
                        CircleToggle.IsChecked = false;
                        RectangleToggle.IsChecked = false;
                        _cmd = new ScaleCommand(this)
                        {
                            Canvas = Canvas,
                            ShapeList = _shapeList
                        };
                        break;
                    case "RectangleToggle":
                        MoveToggle.IsChecked = false;
                        ScaleToggle.IsChecked = false;
                        CircleToggle.IsChecked = false;
                        RectangleToggle.IsChecked = true;
                        SetDrawCommand(ShapeType.Rectangle);
                        break;
                    case "CircleToggle":
                        MoveToggle.IsChecked = false;
                        ScaleToggle.IsChecked = false;
                        CircleToggle.IsChecked = true;
                        RectangleToggle.IsChecked = false;
                        SetDrawCommand(ShapeType.Circle);
                        break;
                }
            }
        }

        private void Canvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Register actions to be taken when the canvas is manipulated
            Canvas.PointerPressed += Canvas_OnPointerPressed;
            Canvas.PointerReleased += Canvas_OnPointerReleased;
            Canvas.PointerMoved += Canvas_OnPointerMoved;
        }

        private void Canvas_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _cmd.PointerEventArgs = e;
            _cmd.ShapeList = _shapeList;
            _canvasInvoker.InvokePointerPressed(_cmd);
        }

        private void Canvas_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _cmd.PointerEventArgs = e;
            _cmd.ShapeList = _shapeList;
            _canvasInvoker.InvokePointerReleased(_cmd);
        }

        private void Canvas_OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _cmd.PointerEventArgs = e;
            _cmd.ShapeList = _shapeList;
            _canvasInvoker.InvokePointerMoved(_cmd);
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddUndoEntry();
            _shapeList.Clear();

            Draw();
            UpdateList();
        }

        private void ShapeList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var added in e.AddedItems)
            {
                int index = ShapeList.Items.IndexOf(added);
                if (index >= 0 && index <= _shapeList.Count)
                {
                    _shapeList[index].Selected = true;
                }
            }

            foreach (var removed in e.RemovedItems)
            {
                int index = ShapeList.Items.IndexOf(removed);
                if (index >= 0 && index <= _shapeList.Count)
                {
                    _shapeList[index].Selected = false;
                }
            }

            Draw();
        }

        public void Draw()
        {
            Canvas.Children.Clear();

            foreach (PaintBase baseShape in _shapeList)
            {
                if (baseShape is PaintShape shape)
                {
                    DrawShape(shape);
                } else if (baseShape is PaintGroup group)
                {
                    DrawGroup(group);
                }



                if (baseShape.Selected)
                {
                    DrawSelector(baseShape);
                }
            }
        }

        public void DrawShape(PaintShape shape)
        {
            Shape canvasShape = shape.ShapeType.GetShape();

            canvasShape.Width = shape.Width;
            canvasShape.Height = shape.Height;
            canvasShape.SetValue(Canvas.LeftProperty, shape.X);
            canvasShape.SetValue(Canvas.TopProperty, shape.Y);
            canvasShape.Scale = shape.Scale != default(Vector3) ? shape.Scale : new Vector3(1f);

            canvasShape.Fill = new SolidColorBrush(Colors.Black);

            Canvas.Children.Add(canvasShape);
        }

        public void DrawGroup(PaintGroup group)
        {
            foreach (PaintBase groupChild in group.Children)
            {
                if (groupChild is PaintShape shape)
                {
                    DrawShape(shape);
                } else if (groupChild is PaintGroup childGroup)
                {
                    DrawGroup(childGroup);
                }
            }
        }

        public void AddUndoEntry()
        {
            _undoStack.Push(_shapeList.DeepCopy());
            _redoStack.Clear();
        }

        public void DrawSelector(PaintBase baseShape)
        {
            double width = baseShape.Width * baseShape.Scale.X;
            double height = baseShape.Height * baseShape.Scale.Y;

            double x = baseShape.X;
            double y = baseShape.Y;

            if (width < 0)
            {
                x = x + width;
                width = width * -1.0;
            }

            if (height < 0)
            {
                y = y + height;
                height = height * -1.0;
            }

            Shape selectorSquare = new Rectangle();
            selectorSquare.Width = width + 4;
            selectorSquare.Height = height + 4;
            selectorSquare.SetValue(Canvas.LeftProperty, x - 2);
            selectorSquare.SetValue(Canvas.TopProperty, y - 2);
            selectorSquare.Stroke = new SolidColorBrush(Colors.Blue);

            Canvas.Children.Add(selectorSquare);
        }

        public void UpdateList()
        {
            ShapeList.Items.Clear();

            foreach (PaintBase baseShape in _shapeList)
            {
                // Create name for list item
                string name = "undefined";
                if (!string.IsNullOrWhiteSpace(baseShape.Decoration))
                {
                    name = baseShape.Decoration;
                }
                else
                {
                    if (baseShape is PaintShape paintShape)
                    {
                        name = paintShape.ShapeType == ShapeType.Circle ? "Circle" : "Rectangle";
                    }
                    else if (baseShape is PaintGroup)
                    {
                        name = "Group";
                    }
                }

                ShapeList.Items.Add(new ListViewItem()
                {
                    Content = name,
                    IsSelected = baseShape.Selected
                });
            }
        }

        private void SetDrawCommand(ShapeType shape)
        {
            _cmd = new DrawShapeCommand(this)
            {
                Canvas = Canvas,
                ShapeType = shape,
                ShapeList = _shapeList
            };
        }

        private async void UndoButtonClick(object sender, RoutedEventArgs e)
        {
            IUserActionCommand cmd = new UndoCommand(this)
            {
                RedoStack = _redoStack,
                UndoStack = _undoStack,
                ShapeList = _shapeList
            };
            await _userInvoker.InvokeUserActionAsync(cmd);
        }

        private async void RedoButtonClick(object sender, RoutedEventArgs e)
        {
            IUserActionCommand cmd = new RedoCommand(this)
            {
                RedoStack = _redoStack,
                UndoStack = _undoStack,
                ShapeList = _shapeList
            };
            await _userInvoker.InvokeUserActionAsync(cmd);
        }

        private async void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            IUserActionCommand cmd = new SaveFileCommand()
            {
                ShapeList = _shapeList
            };
            await _userInvoker.InvokeUserActionAsync(cmd);
        }

        private async void OpenButtonClick(object sender, RoutedEventArgs e)
        {
            IUserActionCommand cmd = new OpenFileCommand(this)
            {
                ShapeList = _shapeList,
                UndoStack = _undoStack,
                RedoStack = _redoStack
            };
            await _userInvoker.InvokeUserActionAsync(cmd);
        }

        private async void GroupButtonClick(object sender, RoutedEventArgs e)
        {
            IUserActionCommand cmd = new GroupCommand(this)
            {
                ShapeList = _shapeList
            };
            await _userInvoker.InvokeUserActionAsync(cmd);
        }

        private async void UnGroupButtonClick(object sender, RoutedEventArgs e)
        {
            IUserActionCommand cmd = new UnGroupCommand(this)
            {
                ShapeList = _shapeList
            };
            await _userInvoker.InvokeUserActionAsync(cmd);
        }
    }
}
