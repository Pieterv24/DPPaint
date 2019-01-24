using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using DPPaint.Extensions;

namespace DPPaint.Shapes
{
    public class BaseShape
    {
        private double _width;
        private double _height;
        private double _x;
        private double _y;
        private FrameworkElement _element;

        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                if (_element != null)
                {
                    _element.Width = _width;
                }
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                if (_element != null)
                {
                    _element.Height = _height;
                }
            }
        }

        public double X
        {
            get => _x;
            set
            {
                _x = value;
                _element?.SetValue(Canvas.LeftProperty, _x);
            }
        }
        public double Y {
            get => _y;
            set
            {
                _y = value;
                _element?.SetValue(Canvas.TopProperty, _y);
            }
        }

        public FrameworkElement Element
        {
            get => _element;
            set
            {
                if (value is Shape shape && shape.Fill == null)
                {
                    shape.Fill = new SolidColorBrush(Colors.Blue);
                }

                if (_y != default(double))
                {
                    value?.SetValue(Canvas.TopProperty, _y);
                }
                if (_x != default(double))
                {
                    value?.SetValue(Canvas.LeftProperty, _x);
                }
                if (value != null && _width != default(double))
                {
                    value.Width = _width;
                }
                if (value != null && _height != default(double))
                {
                    value.Height = _height;
                }

                _element = value;
            }
        }
    }
}
