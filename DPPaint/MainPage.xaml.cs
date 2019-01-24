using DPPaint.Commands;
using DPPaint.Extensions;
using DPPaint.Shapes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
 using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

namespace DPPaint
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ClickInvoker _invoker;
        private ICanvasCommand cmd;
        private List<BaseShape> _shapeList = new List<BaseShape>();
        private List<int> selectedItems = new List<int>();

        public MainPage()
        {
            this.InitializeComponent();
            CircleToggle.IsChecked = true;
            cmd = new DrawShapeCommand(Canvas, _shapeList) {ShapeType = ShapeType.Circle};
            _invoker = new ClickInvoker();
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
                        cmd = new DefaultCommand();
                        break;
                    case "ScaleToggle":
                        MoveToggle.IsChecked = false;
                        ScaleToggle.IsChecked = true;
                        CircleToggle.IsChecked = false;
                        RectangleToggle.IsChecked = false;
                        cmd = new DefaultCommand();
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
            //if (sender is AppBarToggleButton button)
            //{
            //    if (button.Name == RectangleToggle.Name)
            //    {
            //        CircleToggle.IsChecked = false;
            //        RectangleToggle.IsChecked = true;
            //        SetDrawCommand(ShapeType.Rectangle);
            //    } else if (button.Name == CircleToggle.Name)
            //    {
            //        RectangleToggle.IsChecked = false;
            //        CircleToggle.IsChecked = true;
            //        SetDrawCommand(ShapeType.Circle);
            //    }
            //}
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
            cmd.PointerEventArgs = e;
            _invoker.InvokePointerPressed(cmd);
            UpdateList();
        }

        private void Canvas_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            cmd.PointerEventArgs = e;
            _invoker.InvokePointerReleased(cmd);
        }

        private void Canvas_OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            cmd.PointerEventArgs = e;
            _invoker.InvokePointerMoved(cmd);
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            _shapeList.Clear();
            Canvas.Children.Clear();
            UpdateList();
        }

        public void UpdateList()
        {
            if (ShapeList.Items != null && ShapeList.Items.Count > 0)
            {
                ShapeList.Items.Clear();
            }

            for (int i = 0; i < _shapeList.Count; i++)
            {
                var item = new ListViewItem()
                {
                    Content = $"Shape {i}",
                    DataContext = _shapeList[i]
                };

                ShapeList.Items.Add(item);
            }
        }

        private void SetDrawCommand(ShapeType type)
        {
            if (cmd.GetType() != typeof(DrawShapeCommand) || cmd == null)
            {
                cmd = new DrawShapeCommand(Canvas, _shapeList);
            }

            (cmd as DrawShapeCommand).ShapeType = type;
        }

        private void ShapeList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<int> selectedIndici = new List<int>();
            foreach (ItemIndexRange range in ShapeList.SelectedRanges)
            {
                for (int i = 0; i < range.Length; i++)
                {
                    int index = range.FirstIndex + i;
                    selectedIndici.Add(index);
                }
            }

            List<int> newSel = selectedIndici.Where(i => !selectedItems.Contains(i)).ToList();
            List<int> newDeSel = selectedItems.Where(i => !selectedIndici.Contains(i)).ToList();
            
            newSel.ForEach(Select);
            newDeSel.ForEach(Deselect);

            selectedItems = selectedIndici;
        }

        private void Select(int listIndex)
        {
            _shapeList[listIndex] = _shapeList[listIndex].Select();
            Canvas.Children.Add(_shapeList[listIndex].Element);
        }

        private void Deselect(int selected)
        {
            if (selected >= 0 && selected < _shapeList.Count)
            {
                _shapeList[selected] = _shapeList[selected].Deselect();
                Canvas.Children.Add(_shapeList[selected].Element);
            }
        }
    }
}
