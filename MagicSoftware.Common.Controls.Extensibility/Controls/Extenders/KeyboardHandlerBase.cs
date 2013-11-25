using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using log4net;
using LogLevel = log4net.Core.Level;
using MagicSoftware.Common.Utils;

namespace MagicSoftware.Common.Controls.ExtendersX
{
   public abstract class KeyboardHandlerBase : BehaviorExtenderBase
   {
      protected ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


      /// <summary>
      /// 
      /// </summary>
      public KeyboardHandlerBase()
      {

      }

      protected override void Initialize()
      {
         RegisterKeyboardEvents(AttachedElement);
      }

      public override void DetachFromElement()
      {
         UnregisterKeyboardEvents(AttachedElement);
      }

      protected virtual void RegisterKeyboardEvents(UIElement element)
      {
         if (element != null)
         {
            element.PreviewKeyDown += element_PreviewKeyDown;
         }
      }

      protected virtual void UnregisterKeyboardEvents(UIElement element)
      {
         if (element != null)
         {
            element.PreviewKeyDown -= element_PreviewKeyDown;
         }
      }

      void element_PreviewKeyDown(object sender, KeyEventArgs e)
      {
         HandlePreviewKeyDown(sender, e);
      }

      protected abstract void HandlePreviewKeyDown(object sender, KeyEventArgs e);
   }
}
