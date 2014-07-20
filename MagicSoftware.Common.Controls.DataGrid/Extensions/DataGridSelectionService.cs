using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using log4net;
using MagicSoftware.Common.Controls.Table.Models;
using MagicSoftware.Common.Controls.Table.Utils;
using System.Windows.Threading;
using MagicSoftware.Common.Controls.Table.Extensions.Selection;

namespace MagicSoftware.Common.Controls.Table.Extensions
{
   public interface ISelectionService
   {
      void SetSelectionView(ISelectionView selectionView);
   }

   [ImplementedService(typeof(ISelectionService))]
   internal class DataGridSelectionService : ISelectionService, IUIService
   {


      public static ISelectionView GetSelectionView(DependencyObject obj)
      {
         return (ISelectionView)obj.GetValue(SelectionViewProperty);
      }

      public static void SetSelectionView(DependencyObject obj, ISelectionView value)
      {
         obj.SetValue(SelectionViewProperty, value);
      }

      public static readonly DependencyProperty SelectionViewProperty =
          DependencyProperty.RegisterAttached("SelectionView", typeof(ISelectionView), typeof(DataGridSelectionService), new UIPropertyMetadata(null, OnSelectionViewChanged));

      static void OnSelectionViewChanged(object obj, DependencyPropertyChangedEventArgs args)
      {
         var target = (FrameworkElement)obj;
         var selectionSvc = UIServiceProvider.GetService<DataGridSelectionService>(target);
         // Selection service may be null because it was either (1) not loaded yet or (2) not assigned.
         if (selectionSvc != null)
            selectionSvc.SetSelectionView((ISelectionView)args.NewValue);
      }

      public static readonly string LoggerName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace + ".SelectionService";
      private int id;
      private ILog log = log4net.LogManager.GetLogger(LoggerName);

      private SelectionModeManager selectionModeManager;

      public DataGridSelectionService()
      {
         id = IdGenerator.GetNewId(this);
      }

      private event Action Attached;

      public bool IsAttached
      {
         get { return TargetElement != null; }
      }

      private MultiSelector TargetElement { get; set; }

      public void AttachToElement(FrameworkElement element)
      {
         if (!(element is MultiSelector))
            throw new InvalidOperationException("The selection service requires a multi-selector.");

         TargetElement = (MultiSelector)element;
         UIServiceProvider.AddServiceProviderFullyAttachedHandler(element, Element_ServiceProviderIsFullyAttached);
         if (Attached != null)
            Attached();
      }

      public void DetachFromElement(FrameworkElement element)
      {
         if (selectionModeManager != null)
            selectionModeManager.Dispose();
         TargetElement = null;
         selectionModeManager = null;
      }

      public void Dispose()
      {
         DetachFromElement(TargetElement);
      }

      public void SetSelectionView(ISelectionView selectionView)
      {
         if (selectionModeManager != null)
            selectionModeManager.SetSelectionView(selectionView);
      }

      private void Element_ServiceProviderIsFullyAttached(object obj, RoutedEventArgs args)
      {
         UIServiceProvider.RemoveServiceProviderFullyAttachedHandler(TargetElement, Element_ServiceProviderIsFullyAttached);
         var currentItemProvider = UIServiceProvider.GetService<ICurrentItemService>(TargetElement);
         if (selectionModeManager != null)
            selectionModeManager.Dispose();
         selectionModeManager = new SelectionModeManager(TargetElement, currentItemProvider);
         var selectionView = GetSelectionView(TargetElement);
         if (selectionView != null)
            selectionModeManager.SetSelectionView(selectionView);
      }
   }
}