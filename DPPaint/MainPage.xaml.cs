using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DPPaint
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool clicked = false;
        private Point clickStart;
        private Ellipse current;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void BrushToggle_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarToggleButton button)
            {
                if (button.Name == RectangleToggle.Name)
                {
                    CircleToggle.IsChecked = false;
                } else if (button.Name == CircleToggle.Name)
                {
                    RectangleToggle.IsChecked = false;
                }
            }
        }

        private void Canvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            Canvas.PointerPressed += Canvas_OnPointerPressed;
            Canvas.PointerReleased += Canvas_OnPointerReleased;
            Canvas.PointerMoved += Canvas_OnPointerMoved;
        }

        private void Canvas_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            clicked = true;
            PointerPoint ptrPt = e.GetCurrentPoint(Canvas);
            clickStart = ptrPt.Position;
            current = new Ellipse();
            current.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
            current.Width = 0;
            current.Height = 0;
            current.SetValue(Canvas.LeftProperty, clickStart.X);
            current.SetValue(Canvas.TopProperty, clickStart.Y);
            Canvas.Children.Add(current);
            // throw new NotImplementedException();
        }

        private void Canvas_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            clicked = false;
        }

        private void Canvas_OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (clicked)
            {
                PointerPoint currentPoint = e.GetCurrentPoint(Canvas);
                Point difference = new Point(currentPoint.Position.X - clickStart.X, currentPoint.Position.Y - clickStart.Y);
                if (difference.X < 0)
                {
                    current.Width = difference.X * -1;
                    current.SetValue(Canvas.LeftProperty, clickStart.X + difference.X);
                }
                else
                {
                    current.Width = difference.X;
                }
                if (difference.Y < 0)
                {
                    current.Height = difference.Y * -1;
                    current.SetValue(Canvas.TopProperty, clickStart.Y + difference.Y);
                }
                else
                {
                    current.Height = difference.Y;
                }
            }
        }
    }
}
