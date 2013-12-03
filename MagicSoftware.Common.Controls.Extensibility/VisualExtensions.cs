using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows;

namespace MagicSoftware.Common.Controls.Extensibility
{
   public static class VisualExtensions
   {
      public static void ClearAdornersFor(this Visual owner, UIElement element)
      {
         var layer = AdornerLayer.GetAdornerLayer(owner);
         if (layer == null)
            return;

         ClearAdornersFromLayer(layer, element);
      }

      public static void ClearAdornersForEach<T>(this Visual owner)
         where T: UIElement
      {
         var layer = AdornerLayer.GetAdornerLayer(owner);
         if (layer == null)
            return;

         var children = UIUtils.GetVisualChildren<T>(owner, (v) => true);
         foreach (var child in children)
         {
            ClearAdornersFromLayer(layer, child);
         }
      }

      private static void ClearAdornersFromLayer(AdornerLayer layer, UIElement element)
      {
         var adorners = layer.GetAdorners(element);
         if (adorners == null)
            return;

         foreach (var adorner in adorners)
         {
            layer.Remove(adorner);
         }
      }
   }
}
