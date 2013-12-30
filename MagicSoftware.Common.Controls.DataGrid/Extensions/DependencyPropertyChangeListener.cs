using System;
using System.ComponentModel;
using System.Windows;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class DependencyPropertyChangeListener : IDisposable
   {
      DependencyPropertyDescriptor propertyDescriptor;
      DependencyObject propertyOwner;
      EventHandler valueChangedHandler;

      public DependencyPropertyChangeListener(DependencyObject propertyOwner, DependencyProperty property, EventHandler valueChangedHandler)
      {
         this.propertyOwner = propertyOwner;
         this.valueChangedHandler = valueChangedHandler;
         propertyDescriptor = DependencyPropertyDescriptor.FromProperty(property, propertyOwner.GetType());
         propertyDescriptor.AddValueChanged(propertyOwner, valueChangedHandler);
      }

      #region IDisposable Members

      public void Dispose()
      {
         propertyDescriptor.RemoveValueChanged(propertyOwner, valueChangedHandler);
      }

      #endregion
   }
}

