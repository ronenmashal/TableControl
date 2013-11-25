using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace MagicSoftware.Common.Controls.ExtendersX
{
   public class DataGridExtendersCollection : List<DataGridExtenderBase>
   {

   }

   public static class BehaviorExtensions
   {
      #region Keyboard Handler Attached Property

      public static KeyboardHandlerBase GetKeyboardHandler(DependencyObject obj)
      {
         return (KeyboardHandlerBase)obj.GetValue(KeyboardHandlerProperty);
      }

      public static void SetKeyboardHandler(DependencyObject obj, KeyboardHandlerBase value)
      {
         obj.SetValue(KeyboardHandlerProperty, value);
      }

      public static readonly DependencyProperty KeyboardHandlerProperty =
          DependencyProperty.RegisterAttached("KeyboardHandler", typeof(KeyboardHandlerBase), typeof(BehaviorExtensions), new UIPropertyMetadata(null, OnKeyboardHandlerChanged));

      public static void OnKeyboardHandlerChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
      {
         ReplaceExtender<KeyboardHandlerBase>(obj, args.OldValue, args.NewValue);
      }

      #endregion

      #region Focus Manager Attached Property

      public static FocusManagerBase GetFocusManager(DependencyObject obj)
      {
         if (obj == null)
            return null;

         FocusManagerBase focusManager = (FocusManagerBase)obj.GetValue(FocusManagerProperty);
         if (focusManager == null)
            return GetFocusManager(VisualTreeHelper.GetParent(obj));

         return focusManager;
      }

      public static void SetFocusManager(DependencyObject obj, FocusManagerBase value)
      {
         obj.SetValue(FocusManagerProperty, value);
      }

      public static readonly DependencyProperty FocusManagerProperty =
          DependencyProperty.RegisterAttached("FocusManager", typeof(FocusManagerBase), typeof(BehaviorExtensions), new UIPropertyMetadata(null, OnFocusManagerChanged));

      static void OnFocusManagerChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
      {
         ReplaceExtender<FocusManagerBase>(obj, args.OldValue, args.NewValue);
      }

      #endregion

      #region Mouse Handler Attached Property

      public static DataGridMouseHandler GetMouseHandler(DependencyObject obj)
      {
         return (DataGridMouseHandler)obj.GetValue(MouseHandlerProperty);
      }

      public static void SetMouseHandler(DependencyObject obj, DataGridMouseHandler value)
      {
         obj.SetValue(MouseHandlerProperty, value);
      }

      // Using a DependencyProperty as the backing store for MouseHandler.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty MouseHandlerProperty =
          DependencyProperty.RegisterAttached("MouseHandler", typeof(DataGridMouseHandler), typeof(BehaviorExtensions), new UIPropertyMetadata(null, OnMouseHandlerChanged));

      static void OnMouseHandlerChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
      {
         ReplaceExtender<MouseHandlerBase>(obj, args.OldValue, args.NewValue);
      }

      #endregion

      #region Validation Extender Attached Property


      public static DataGridValidationExtender GetValidationExtender(DependencyObject obj)
      {
         return (DataGridValidationExtender)obj.GetValue(ValidationExtenderProperty);
      }

      public static void SetValidationExtender(DependencyObject obj, DataGridValidationExtender value)
      {
         obj.SetValue(ValidationExtenderProperty, value);
      }

      // Using a DependencyProperty as the backing store for ValidationExtender.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty ValidationExtenderProperty =
          DependencyProperty.RegisterAttached("ValidationExtender", typeof(DataGridValidationExtender), typeof(BehaviorExtensions), new UIPropertyMetadata(null, ValidationExtenderChanged));

      static void ValidationExtenderChanged(DependencyObject obj, DependencyPropertyChangedEventArgs eventArgs)
      {
         ReplaceExtender<DataGridValidationExtender>(obj, eventArgs.OldValue, eventArgs.NewValue);
      }

      #endregion

      #region Validation Extender Attached Property


      public static DataGridExtendersCollection GetExtenders(DependencyObject obj)
      {
         return (DataGridExtendersCollection)obj.GetValue(ExtendersProperty);
      }

      public static void SetExtenders(DependencyObject obj, DataGridExtendersCollection value)
      {
         obj.SetValue(ExtendersProperty, value);
      }

      // Using a DependencyProperty as the backing store for ValidationExtender.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty ExtendersProperty =
          DependencyProperty.RegisterAttached("Extenders", typeof(DataGridExtendersCollection), typeof(BehaviorExtensions), new UIPropertyMetadata(null, ExtendersChanged));

      static void ExtendersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs eventArgs)
      {
         var oldList = eventArgs.OldValue as DataGridExtendersCollection;
         var newList = eventArgs.NewValue as DataGridExtendersCollection;

         if (oldList != null)
            foreach (var extender in oldList)
               DetachExtender(obj, extender);

         if (newList != null)
            foreach (var extender in newList)
               AttachExtender(obj, extender);
      }

      #endregion

      static void ReplaceExtender<T>(DependencyObject obj, object oldExtender, object newExtender)
         where T : BehaviorExtenderBase
      {
         if (newExtender != null)
            Debug.Assert(newExtender is T);

         DetachExtender(obj, (T)oldExtender);
         AttachExtender(obj, (T)newExtender);
      }

      static void AttachExtender(DependencyObject obj, BehaviorExtenderBase newExtender)
      {
         if (newExtender != null)
            newExtender.AttachToElement((UIElement)obj);
      }

      static void DetachExtender(DependencyObject obj, BehaviorExtenderBase oldExtender)
      {
         if (oldExtender != null)
            oldExtender.Dispose();
      }
   }
}
