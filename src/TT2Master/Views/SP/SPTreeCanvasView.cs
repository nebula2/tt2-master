using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TT2Master.Views.SP
{
    public class SPTreeCanvasView : SKCanvasView
    {
        public static readonly BindableProperty ColorProperty =
            BindableProperty.Create("Color", typeof(SKColor), typeof(SPTreeCanvasView), defaultValue: SKColors.Black, defaultBindingMode: BindingMode.TwoWay, propertyChanged: RedrawCanvas);

        public SKColor Color
        {
            get => (SKColor)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }


        private static void RedrawCanvas(BindableObject bindable, object oldvalue, object newvalue)
        {
            var bindableCanvas = bindable as SPTreeCanvasView;
            bindableCanvas.InvalidateSurface();
        }
    }
}
