using DPPaint.Commands.Click;
using DPPaint.Commands.UserAction;
using DPPaint.Extensions;
using DPPaint.Shapes;
using DPPaint.Strategy;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using DPPaint.Decorators;

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

            // Instantiate lists and stacks to keep track of the state
            _shapeList = new List<PaintBase>();
            _undoStack = new Stack<List<PaintBase>>();
            _redoStack = new Stack<List<PaintBase>>();

            // Initialize invokers for the command pattern
            _canvasInvoker = new ClickInvoker();
            _userInvoker = new UserActionInvoker();

            // Set initial selection to circle
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

        /// <summary>
        /// Bind mouse events for the canvas to the corresponding Methods
        /// </summary>
        private void Canvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Register actions to be taken when the canvas is manipulated
            Canvas.PointerPressed += Canvas_OnPointerPressed;
            Canvas.PointerReleased += Canvas_OnPointerReleased;
            Canvas.PointerMoved += Canvas_OnPointerMoved;
        }

        #region Canvas pointer actions

        /// <summary>
        /// Invokes the command pattern using the active command when the mouse is pressed
        /// </summary>
        private async void Canvas_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _cmd.PointerEventArgs = e;
            _cmd.ShapeList = _shapeList;
            _cmd.RedoStack = _redoStack;
            _cmd.UndoStack = _undoStack;
            _cmd.Canvas = Canvas;
            await _canvasInvoker.InvokePointerPressedAsync(_cmd);
        }

        /// <summary>
        /// Invokes the command pattern using the active command when the mouse is released
        /// </summary>
        private async void Canvas_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _cmd.PointerEventArgs = e;
            _cmd.ShapeList = _shapeList;
            _cmd.RedoStack = _redoStack;
            _cmd.UndoStack = _undoStack;
            _cmd.Canvas = Canvas;
            await _canvasInvoker.InvokePointerReleasedAsync(_cmd);
        }

        /// <summary>
        /// Invokes the command pattern using the active command when the mouse is moved
        /// </summary>
        private async void Canvas_OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _cmd.PointerEventArgs = e;
            _cmd.ShapeList = _shapeList;
            _cmd.RedoStack = _redoStack;
            _cmd.UndoStack = _undoStack;
            _cmd.Canvas = Canvas;
            await _canvasInvoker.InvokePointerMovedAsync(_cmd);
        }

        #endregion

        #region Button Action Methods

        /// <summary>
        /// Run when the clear canvas button is clicked.
        /// </summary>
        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Add undo entry to the undo stack and clear redo stack
            _undoStack.Push(_shapeList.DeepCopy());
            _redoStack.Clear();

            // Clear current canvas
            _shapeList.Clear();

            // Redraw the canvas and itemlist
            Draw();
            UpdateList();
        }

        /// <summary>
        /// Handles UI toggles
        /// </summary>
        private void BrushToggle_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarToggleButton button)
            {
                // Make sure all toggles are off before turning a new one on
                ClickSelectToggle.IsChecked = false;
                SelectToggle.IsChecked = false;
                MoveToggle.IsChecked = false;
                ScaleToggle.IsChecked = false;
                CircleToggle.IsChecked = false;
                RectangleToggle.IsChecked = false;
                EditDecorator.IsChecked = false;

                // Check what button is toggled and change the active command accordingly
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
                    case "EditDecorator":
                        EditDecorator.IsChecked = true;
                        _cmd = new ChangeDecoratorCommand(this);
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

        /// <summary>
        /// Handles UI buttons and activates the command pattern
        /// </summary>
        private async void UserActionClick(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarButton button)
            {
                IUserActionCommand cmd = null;

                switch (button.Name)
                {
                    case "OpenButton":
                        cmd = new OpenFileCommand(this);
                        break;
                    case "SaveButton":
                        cmd = new SaveFileCommand();
                        break;
                    case "RedoButton":
                        cmd = new RedoCommand(this);
                        break;
                    case "UndoButton":
                        cmd = new UndoCommand(this);
                        break;
                    case "GroupButton":
                        cmd = new GroupCommand(this);
                        break;
                    case "UnGroupButton":
                        cmd = new UnGroupCommand(this);
                        break;
                    case "DeleteButton":
                        cmd = new DeleteItemCommand(this);
                        break;
                }

                if (cmd != null)
                {
                    cmd.RedoStack = _redoStack;
                    cmd.UndoStack = _undoStack;
                    cmd.ShapeList = _shapeList;

                    await _userInvoker.InvokeUserActionAsync(cmd);
                }
            }
        }

        #endregion

        #region Draw Methods

        /// <summary>
        /// Draws items in the shapelist onto the Canvas
        /// </summary>
        public void Draw()
        {
            // Clear the canvas
            Canvas.Children.Clear();

            // Draw each of the items onto the canvas
            foreach (PaintBase baseShape in _shapeList)
            {
                baseShape.DrawOnCanvas(Canvas);
            }
        }

        #endregion

        #region Selector list Methods

        /// <summary>
        /// Triggers when the selector on the ItemList in the UI is used and updates the states in the _shapeList accordingly
        /// </summary>
        private void ShapeList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check what items should be added
            foreach (var added in e.AddedItems)
            {
                int index = ShapeList.Items.IndexOf(added);
                if (index >= 0 && index <= _shapeList.Count)
                {
                    _shapeList[index].Selected = true;
                }
            }

            // Check what items should be removed
            foreach (var removed in e.RemovedItems)
            {
                int index = ShapeList.Items.IndexOf(removed);
                if (index >= 0 && index <= _shapeList.Count)
                {
                    _shapeList[index].Selected = false;
                }
            }

            // Run draw method to update selector squares
            Draw();
        }

        /// <summary>
        /// Update Shapelist, Should be ran when the _shapeList is modified
        /// </summary>
        public void UpdateList()
        {
            // Clear current list
            ShapeList.Items?.Clear();

            // Add items from _shapeList
            foreach (PaintBase baseShape in _shapeList)
            {
                // Create name for list item
                string name = "undefined";
                if (baseShape is TextDecoration decoration)
                {
                    name = decoration.DecorationText;
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
