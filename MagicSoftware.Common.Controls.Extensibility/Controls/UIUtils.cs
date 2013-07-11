using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace MagicSoftware.Common.Controls
{
   public static class UIUtils
   {
      public static T GetVisualChild<T>(Visual parent) where T : Visual
      {
         T child = default(T);
         int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
         for (int i = 0; i < numVisuals; i++)
         {
            Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
            child = v as T;
            if (child == null)
            {
               child = GetVisualChild<T>(v);
            }
            if (child != null)
            {
               break;
            }
         }
         return child;
      }

      public static T GetAncestor<T>(DependencyObject element)
         where T : DependencyObject
      {
         if (element == null)
            return null;

         if (element is T)
            return (T)element;

         return GetAncestor<T>(VisualTreeHelper.GetParent(element));
      }

   }


}
