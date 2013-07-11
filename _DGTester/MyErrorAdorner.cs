using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Globalization;

namespace _DGTester
{
   public class MyErrorAdorner : Adorner
   {
      public string Text { get; set; }

      public MyErrorAdorner(UIElement adornedElement):
         base(adornedElement)
      {

      }

      protected override void OnRender(DrawingContext drawingContext)
      {
         Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);

         SolidColorBrush renderBrush = new SolidColorBrush(Colors.Red);
         renderBrush.Opacity = 0.01;
         Pen renderPen = new Pen(new SolidColorBrush(Colors.Red), 1.5);
         //double renderRadius = 5.0;

         drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
         //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
         //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
         //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
         //drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);

         if (Text != null)
         {
            Typeface tf = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Condensed);
            FormattedText ft = new FormattedText(Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, tf, 12.0, Brushes.Black);
            Point labelOrigin = new Point(adornedElementRect.TopLeft.X, adornedElementRect.Bottom + 5);
            drawingContext.DrawRectangle(Brushes.Yellow, null, new Rect(labelOrigin, new Size(ft.WidthIncludingTrailingWhitespace, ft.Height)));
            drawingContext.DrawText(ft, labelOrigin);
         }
      }
   }
}
