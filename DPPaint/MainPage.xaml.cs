using DPPaint.Commands.Click;
using DPPaint.Commands.UserAction;
using DPPaint.Extensions;
using DPPaint.Shapes;
using DPPaint.Strategy;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

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

            _canvasInvoker = new ClickInvoker();
            _userInvoker = new UserActionInvoker();

            CircleToggle.IsChecked = true;
            _cmd = new DrawShapeCommand(this)
            {
                ShapeType = CircleShape.Instance
            };

            // Set canvas Z indici
            Canvas.SetZIndex(BottomPanel, 100);
            Canvas.SetZIndex(TopBar, 100);
            Canvas.SetZIndex(ShapeList, 100);
        }

        private void BrushToggle_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarToggleButton button)
            {
                ClickSelectToggle.IsChecked = false;
                SelectToggle.IsChecked = false;
                MoveToggle.IsChecked = false;
                ScaleToggle.IsChecked = false;
                CircleToggle.IsChecked = false;
                RectangleToggle.IsChecked = false;

                switch (button.Name)
                {
                    case "ClickSelectToggle":
                        ClickSelectToggle.IsChecked = true;
                        _cmd = new ClickSelectCommand(this);
                        break;
                    case "SelectToggle":
                        SelectToggle.IsChecked = true;
                        _cmd = new SelectCommand(this);
                        break;
                    case "MoveToggle":
                        MoveToggle.IsChecked = true;
                        _cmd = new MoveCommand(this);
                        break;
                    case "ScaleToggle":
                        ScaleToggle.IsChecked = true;
                        _cmd = new ScaleCommand(this);
                        break;
                    case "RectangleToggle":
                        RectangleToggle.IsChecked = true;
                        _cmd = new DrawShapeCommand(this)
                        {
                            ShapeType = RectangleShape.Instance
                        };
                        break;
                    case "CircleToggle":
                        CircleToggle.IsChecked = true;
                        _cmd = new DrawShapeCommand(this)
                        {
                            ShapeType = CircleShape.Instance
                        };
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

        #region Canvas pointer actions

        private void Canvas_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _cmd.PointerEventArgs = e;
            _cmd.ShapeList = _shapeList;
            _cmd.RedoStack = _redoStack;
            _cmd.UndoStack = _undoStack;
            _cmd.Canvas = Canvas;
            _canvasInvoker.InvokePointerPressed(_cmd);
        }

        private void Canvas_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _cmd.PointerEventArgs = e;
            _cmd.ShapeList = _shapeList;
            _cmd.RedoStack = _redoStack;
            _cmd.UndoStack = _undoStack;
            _cmd.Canvas = Canvas;
            _canvasInvoker.InvokePointerReleased(_cmd);
        }

        private void Canvas_OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _cmd.PointerEventArgs = e;
            _cmd.ShapeList = _shapeList;
            _cmd.RedoStack = _redoStack;
            _cmd.UndoStack = _undoStack;
            _cmd.Canvas = Canvas;
            _canvasInvoker.InvokePointerMoved(_cmd);
        }

        #endregion

        #region User Action Methods

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            _undoStack.Push(_shapeList.DeepCopy());
            _redoStack.Clear();
            _shapeList.Clear();

            Draw();
            UpdateList();
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
                ShapeList = _shapeList,
                UndoStack = _undoStack,
                RedoStack = _redoStack
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
                ShapeList = _shapeList,
                UndoStack = _undoStack,
                RedoStack = _redoStack
            };
            await _userInvoker.InvokeUserActionAsync(cmd);
        }

        private async void UnGroupButtonClick(object sender, RoutedEventArgs e)
        {
            IUserActionCommand cmd = new UnGroupCommand(this)
            {
                ShapeList = _shapeList,
                UndoStack = _undoStack,
                RedoStack = _redoStack
            };
            await _userInvoker.InvokeUserActionAsync(cmd);
        }

        private async void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            IUserActionCommand cmd = new DeleteItemCommand(this)
            {
                ShapeList = _shapeList,
                UndoStack = _undoStack,
                RedoStack = _redoStack
            };
            await _userInvoker.InvokeUserActionAsync(cmd);
        }

        #endregion

        #region Draw Methods

        public void Draw()
        {
            Canvas.Children.Clear();

            foreach (PaintBase baseShape in _shapeList)
            {
                if (baseShape is PaintShape shape)
                {
                    DrawShape(shape);
                }
                else if (baseShape is PaintGroup group)
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
            Canvas.Children.Add(shape.GetDrawShape());
        }

        public void DrawGroup(PaintGroup group)
        {
            foreach (PaintBase groupChild in group.Children)
            {
                if (groupChild is PaintShape shape)
                {
                    DrawShape(shape);
                }
                else if (groupChild is PaintGroup childGroup)
                {
                    DrawGroup(childGroup);
                }
            }
        }

        public void DrawSelector(PaintBase baseShape)
        {
            double width = baseShape.Width;
            double height = baseShape.Height;

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

        #endregion

        #region Selector list Methods

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

        public void UpdateList()
        {
            ShapeList.Items?.Clear();

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
                        name = paintShape.ToString();
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

        #endregion
    }
}
