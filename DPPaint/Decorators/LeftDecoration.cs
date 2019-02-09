﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using DPPaint.Shapes;

namespace DPPaint.Decorators
{
    public class LeftDecoration : TextDecoration
    {
        public LeftDecoration(PaintBase paintBase) : base(paintBase)
        {
        }

        public LeftDecoration(PaintBase paintBase, string decorationText) : base(paintBase)
        {
            DecorationText = decorationText;
        }

        public override TextDecoration GetClickedDecoration(double clickX, double clickY)
        {
            TextBlock tb = new TextBlock
            {
                Text = DecorationText,
                IsTextSelectionEnabled = false,
                TextWrapping = TextWrapping.NoWrap,
                Foreground = new SolidColorBrush(Colors.Black)
            };

            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            double newX = X - tb.ActualHeight;
            double newY = (Y + (Height / 2)) - (tb.ActualWidth / 2);

            double maxX = newX + tb.ActualHeight;
            double maxY = newY + tb.ActualWidth;

            if ((clickX > newX && clickX < maxX) && (clickY > newY && clickY < maxY))
            {
                return this;
            }
            else if (_paintBase is TextDecoration decoration)
            {
                TextDecoration decor = decoration.GetClickedDecoration(clickX, clickY);
                if (decor != null)
                {
                    return decor;
                }
            }

            return null;
        }

        public override void DrawOnCanvas(Canvas canvas)
        {
            TextBlock tb = new TextBlock
            {
                Text = DecorationText,
                IsTextSelectionEnabled = false,
                TextWrapping = TextWrapping.NoWrap,
                Foreground = new SolidColorBrush(Colors.Black)
            };

            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            Transform rotate = new RotateTransform()
            {
                Angle = -90
            };

            tb.RenderTransform = rotate;

            double newY = (Y + (Height / 2)) + (tb.ActualWidth / 2);

            tb.SetValue(Canvas.LeftProperty, X - tb.ActualHeight);
            tb.SetValue(Canvas.TopProperty, newY);

            canvas.Children.Add(tb);

            base.DrawOnCanvas(canvas);
        }

        public override DecoratorAnchor GetDecoratorPosition()
        {
            return DecoratorAnchor.Left;
        }
    }
}
