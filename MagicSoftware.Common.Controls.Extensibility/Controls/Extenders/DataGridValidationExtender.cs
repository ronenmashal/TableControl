using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using MagicSoftware.Common.Controls.Proxies;
using MagicSoftware.Common.Controls.Extensibility.Controls.Extenders;
using _DGTester.Validation;
using MagicSoftware.Common.Controls.Extensibility.Controls;

namespace MagicSoftware.Common.Controls.Extenders
{
   public class DataGridValidationExtender : DataGridExtenderBase
   {
      Dictionary<object, Adorner[]> itemValidationAdorners = new Dictionary<object, Adorner[]>();

      public IValidationAdornerFactory AdornerFactory { get; set; }

      protected override void Initialize()
      {
         AttachedDG.CellEditEnding += AttachedDG_CellEditEnding;
         AttachedDG.LoadingRow += new EventHandler<DataGridRowEventArgs>(AttachedDG_LoadingRow);
         AttachedDG.UnloadingRow += new EventHandler<DataGridRowEventArgs>(AttachedDG_UnloadingRow);
      }

      void AttachedDG_UnloadingRow(object sender, DataGridRowEventArgs e)
      {
         var container = e.Row;
         var adornerLayer = AdornerLayer.GetAdornerLayer(container);
         Adorner[] adorners = adornerLayer.GetAdorners(container);

         if (adorners != null)
         {
            itemValidationAdorners[e.Row.Item] = adorners;
         }
      }

      void AttachedDG_LoadingRow(object sender, DataGridRowEventArgs e)
      {
         var container = e.Row;
         var adornerLayer = AdornerLayer.GetAdornerLayer(container);
         Adorner[] adorners = adornerLayer.GetAdorners(container);

         if (adorners != null)
         {
            foreach (var adorner in adorners)
            {
               //                if (adorner is MyErrorAdorner)
               //                   adornerLayer.Remove(adorner);
            }
            if (itemValidationAdorners.ContainsKey(e.Row.Item))
            {
               foreach (var adorner in itemValidationAdorners[e.Row.Item])
               {
                  adornerLayer.Add(adorner);
               }
            }
         }
      }

      public override void DetachFromElement()
      {
         AttachedDG.CellEditEnding -= AttachedDG_CellEditEnding;
      }


      void AttachedDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
      {
         Trace.WriteLine("Finished editing cell: " + e.EditAction + ", " + e.EditingElement);
         var container = UIUtils.GetAncestor<DataGridRow>(e.EditingElement);
         var adornerLayer = AdornerLayer.GetAdornerLayer(container);
         Adorner[] adorners = adornerLayer.GetAdorners(e.EditingElement);

         if (adorners != null)
         {
            foreach (var adorner in adorners)
            {
               adornerLayer.Remove(adorner);
            }
         }

         this.DGProxy.EnableCommands(SelectorProxy.NavigationCommands);

         ElementProxy proxy = ProxyFactory.GetProxy(e.EditingElement);
         DependencyProperty property = proxy.GetValueProperty();
         BindingExpressionBase beb = BindingOperations.GetBindingExpressionBase(e.EditingElement, property);
         if (beb != null)
         {
            BindingExpression be = beb as BindingExpression;
            if (be != null)
            {
               var result = ValidateBinding(be, e.EditingElement, property);
               if (result.ShouldShowIndication)
               {
                  if (AdornerFactory != null)
                     adornerLayer.Add(AdornerFactory.CreateAdorner(result, e.EditingElement));
               }
               if (result.ShouldBlock)
               {
                  this.DGProxy.DisableCommands(SelectorProxy.NavigationCommands);
                  e.Cancel = true;
               }
               
            }
         }
      }

      ExtendedValidationResult ValidateBinding(BindingExpression be, DependencyObject targetObject, DependencyProperty targetProperty)
      {
         Binding b = be.ParentBinding;
         List<ExtendedValidationResult> results = new List<ExtendedValidationResult>();
         foreach (var validationRule in b.ValidationRules)
         {
            if (validationRule is ExtendedValidationRule)
            {
               results.Add(EvaluateExtendedValidationRule(be, (ExtendedValidationRule)validationRule, targetObject, targetProperty));
            }
         }
         return ExtendedValidationResult.MergeResults(results);
      }

      private ExtendedValidationResult EvaluateExtendedValidationRule(BindingExpression be, ExtendedValidationRule extendedValidationRule, DependencyObject targetObject, DependencyProperty targetProperty)
      {
         Binding b = be.ParentBinding;
         return extendedValidationRule.Validate(be.DataItem, b.Path, targetObject.GetValue(targetProperty), null);
      }
   }
}
