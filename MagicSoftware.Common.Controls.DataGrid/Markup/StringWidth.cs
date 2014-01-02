using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace MagicSoftware.Common.Controls.Table.Markup
{
   [MarkupExtensionReturnType(typeof(double))]
   public class TextRenderWidth : MarkupExtension
   {
      public String Text { get; set; }

      public TextRenderWidth(string text)
      {
         Text = text;
      }

      public override object ProvideValue(IServiceProvider serviceProvider)
      {
         var provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

         var fontInfoSource = provideValueTarget.TargetObject as Control;

         FormattedText ft = new FormattedText(Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            new Typeface(fontInfoSource.FontFamily, fontInfoSource.FontStyle, fontInfoSource.FontWeight, FontStretches.Normal),
            fontInfoSource.FontSize, System.Windows.Media.Brushes.Black);

         return ft.Width;
         
      }
   }
}
