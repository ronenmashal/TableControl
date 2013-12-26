using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.Common.Controls.Proxies;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   class EnhancedDGProxy : ItemsControlProxy
   {
      #region Commands

      public event CanExecuteRoutedEventHandler PreviewCanExecute
      {
         add { CommandManager.AddPreviewCanExecuteHandler(ProxiedElement, value); }
         remove { CommandManager.RemovePreviewCanExecuteHandler(ProxiedElement, value); }
      }

      public void ExecuteCommand(RoutedCommand command, object commandParameter)
      {
         command.Execute(commandParameter, ProxiedElement);
      }

      #endregion

      public object ElementAsEventSource { get { return ProxiedElement; } }

      private DataGrid DataGridElement { get { return (DataGrid)ProxiedElement; } }

      public CollectionView ItemsView { get; private set; }

      protected override void Initialize()
      {
         base.Initialize();
      }

      protected override void Cleanup()
      {
         base.Cleanup();
      }

      public void AddRoutedEventHandler(RoutedEvent routedEvent, RoutedEventHandler handler)
      {
         ProxiedElement.AddHandler(routedEvent, handler);
      }

      public void RemoveRoutedEventHandler(RoutedEvent routedEvent, RoutedEventHandler handler)
      {
         ProxiedElement.RemoveHandler(routedEvent, handler);
      }

      public override void ScrollIntoView(object item)
      {
         if (item != null)
            DataGridElement.ScrollIntoView(item);
      }

      ICurrentItemProvider currentItemProvider = null;

      protected override object GetAdapter(Type adapterType)
      {
         if (adapterType == typeof(ICurrentItemProvider))
         {
            if (currentItemProvider == null)
               currentItemProvider = new DataGridAsCurrentItemProvider(DataGridElement);
            return currentItemProvider;
         }

         if (adapterType == typeof(IElementEditStateProxy))
            return new DataGridAsEditingAdapter(DataGridElement, this);

         return base.GetAdapter(adapterType);
      }

      public object SelectedItemContainer()
      {
         if (DataGridElement.SelectedIndex >= 0)
            return DataGridElement.ItemContainerGenerator.ContainerFromIndex(DataGridElement.SelectedIndex);

         else return null;
      }

      //TODO: Move to FrameworkElementProxy.
      /// <summary>
      /// Gets the proxied element's dispatcher.
      /// </summary>
      /// <returns>Dispatcher for UI operations.</returns>
      internal Dispatcher GetDispatcher()
      {
         return this.ProxiedElement.Dispatcher;
      }

      internal void RaiseEvent(RoutedEventArgs eventArgs)
      {
         ProxiedElement.RaiseEvent(eventArgs);
      }

      public ICollectionViewMoveAction GetMoveToFirstItemAction()
      {
         return new MoveCurrentToPositionAction() { NewPosition = 0 };
      }

      public ICollectionViewMoveAction GetMoveToLastItemAction()
      {
         return new MoveCurrentToPositionAction() { NewPosition = DataGridElement.Items.Count - 1 };
      }

   }
}
