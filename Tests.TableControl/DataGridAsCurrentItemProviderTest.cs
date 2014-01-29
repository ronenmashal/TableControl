using MagicSoftware.Common.Controls.Table.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Tests.Common;
using System.Windows;
using System.Windows.Automation.Peers;
using MagicSoftware.Common.Controls.Proxies;
using Microsoft.Test.Input;
using Tests.TableControl.UI;

namespace Tests.TableControl
{


   /// <summary>
   ///This is a test class for DataGridAsCurrentItemProviderTest and is intended
   ///to contain all DataGridAsCurrentItemProviderTest Unit Tests
   ///</summary>
   [TestClass()]
   public class DataGridAsCurrentItemProviderTest
   {


      private TestContext testContextInstance;

      /// <summary>
      ///Gets or sets the test context which provides
      ///information about and functionality for the current test run.
      ///</summary>
      public TestContext TestContext
      {
         get
         {
            return testContextInstance;
         }
         set
         {
            testContextInstance = value;
         }
      }

      #region Additional test attributes
      // 
      //You can use the following additional attributes as you write your tests:
      //
      //Use ClassInitialize to run code before running the first test in the class
      //[ClassInitialize()]
      //public static void MyClassInitialize(TestContext testContext)
      //{
      //}
      //
      //Use ClassCleanup to run code after all tests in a class have run
      //[ClassCleanup()]
      //public static void MyClassCleanup()
      //{
      //}
      //
      //Use TestInitialize to run code before running each test
      //[TestInitialize()]
      //public void MyTestInitialize()
      //{
      //}
      //
      //Use TestCleanup to run code after each test has run
      //[TestCleanup()]
      //public void MyTestCleanup()
      //{
      //}
      //
      #endregion


      /// <summary>
      ///A test for DataGrid_CurrentItemChanged
      ///</summary>
      [TestMethod()]
      [DeploymentItem("MagicSoftware.Common.Controls.Table.dll")]
      public void PreviewCurrentChangingEventTest()
      {
         ObservableCollection<TestData> dataList = new ObservableCollection<TestData>()
         {
            new TestData() { StrValue = "A" },
            new TestData() { StrValue = "B" },
            new TestData() { StrValue = "C" }
         };

         DataGrid dataGrid;
         var target = new DataGridCurrentItemService();
         UIServiceCollection serviceList = new UIServiceCollection();
         serviceList.Add(target);
         using (TestWindow.Show(dataList, serviceList, out dataGrid))
         {
            dataGrid.CurrentItem = null;
            var helper = new PrivateAccessHelper<DataGridCurrentItemService, DataGridCurrentItemService_Accessor>(target);
            var provider = helper.Target;
            var previewCurrentChangingEventHandlerHelper = new EventHandlerTestHelper<object, CancelableEventArgs>("PreviewCurrentChanging");
            provider.PreviewCurrentChanging += previewCurrentChangingEventHandlerHelper.Handler;

            dataGrid.CurrentCell = new DataGridCellInfo(dataList[0], dataGrid.Columns[0]);
            Assert.IsTrue(previewCurrentChangingEventHandlerHelper.HandlerInvoked);
            Assert.IsFalse(previewCurrentChangingEventHandlerHelper.LastInocationEventArgs.IsCancelable);
            previewCurrentChangingEventHandlerHelper.Reset();

            // Move to another line.
            dataGrid.CurrentCell = new DataGridCellInfo(dataList[1], dataGrid.Columns[0]);
            Assert.IsTrue(previewCurrentChangingEventHandlerHelper.HandlerInvoked);
            Assert.IsFalse(previewCurrentChangingEventHandlerHelper.LastInocationEventArgs.IsCancelable);
            previewCurrentChangingEventHandlerHelper.Reset();


            // Move to another cell on the same line
            dataGrid.CurrentCell = new DataGridCellInfo(dataList[1], dataGrid.Columns[1]);
            Assert.IsFalse(previewCurrentChangingEventHandlerHelper.HandlerInvoked);
            previewCurrentChangingEventHandlerHelper.Reset();

            // Move to another line using the provider.
            provider.MoveCurrentTo(dataList[2]);
            Assert.IsTrue(previewCurrentChangingEventHandlerHelper.HandlerInvoked);
            Assert.IsTrue(previewCurrentChangingEventHandlerHelper.LastInocationEventArgs.IsCancelable);


            provider.PreviewCurrentChanging -= previewCurrentChangingEventHandlerHelper.Handler;
         }
      }

      /// <summary>
      ///A test for MoveCurrent
      ///</summary>
      [TestMethod()]
      public void MoveCurrentTest()
      {
         ObservableCollection<TestData> dataList = new ObservableCollection<TestData>()
         {
            new TestData() { StrValue = "A" },
            new TestData() { StrValue = "B" },
            new TestData() { StrValue = "C" }
         };

         DataGrid dataGrid;
         var target = new DataGridCurrentItemService();
         var serviceList = new UIServiceCollection();
         serviceList.Add(target);
         using (TestWindow.Show(dataList, serviceList, out dataGrid))
         {
            dataGrid.Items.MoveCurrentTo(null);
            var helper = new PrivateAccessHelper<DataGridCurrentItemService, DataGridCurrentItemService_Accessor>(target);
            var provider = helper.Target;
            var previewCurrentChangingEventHandlerHelper = new EventHandlerTestHelper<object, CancelableEventArgs>("PreviewCurrentChanging");
            provider.PreviewCurrentChanging += previewCurrentChangingEventHandlerHelper.Handler;
            var currentChangedEventHandlerHelper = new EventHandlerTestHelper<object, EventArgs>("CurrentChanged");
            provider.CurrentChanged += currentChangedEventHandlerHelper.Handler;

            provider.MoveCurrentTo(dataList[1]);
            Assert.IsTrue(previewCurrentChangingEventHandlerHelper.HandlerInvoked);
            Assert.IsTrue(currentChangedEventHandlerHelper.HandlerInvoked);
            previewCurrentChangingEventHandlerHelper.Reset();
            currentChangedEventHandlerHelper.Reset();

            // Block movement.
            previewCurrentChangingEventHandlerHelper.AdditionalHandling = (sender, args) => { args.Canceled = true; };
            Assert.IsFalse(provider.MoveCurrentTo(dataList[2]));
            Assert.IsTrue(previewCurrentChangingEventHandlerHelper.HandlerInvoked);
            Assert.IsFalse(currentChangedEventHandlerHelper.HandlerInvoked);
            previewCurrentChangingEventHandlerHelper.Reset();
            currentChangedEventHandlerHelper.Reset();

            previewCurrentChangingEventHandlerHelper.AdditionalHandling = null;

            // Move to the same row
            provider.MoveCurrentTo(provider.CurrentItem);
            Assert.IsFalse(previewCurrentChangingEventHandlerHelper.HandlerInvoked);
            Assert.IsFalse(currentChangedEventHandlerHelper.HandlerInvoked);
            previewCurrentChangingEventHandlerHelper.Reset();
            currentChangedEventHandlerHelper.Reset();

            provider.CurrentChanged -= currentChangedEventHandlerHelper.Handler;
            provider.PreviewCurrentChanging -= previewCurrentChangingEventHandlerHelper.Handler;
         }
      }

      /// <summary>
      ///A test for RaiseCurrentChangedEvent
      ///</summary>
      [TestMethod()]
      [DeploymentItem("MagicSoftware.Common.Controls.Table.dll")]
      public void CurrentChangedEventTest()
      {
         ObservableCollection<TestData> dataList = new ObservableCollection<TestData>()
         {
            new TestData() { StrValue = "A" },
            new TestData() { StrValue = "B" },
            new TestData() { StrValue = "C" }
         };

         DataGrid dataGrid;
         var target = new DataGridCurrentItemService();
         var serviceList = new UIServiceCollection();
         serviceList.Add(target);
         using (TestWindow.Show(dataList, serviceList, out dataGrid))
         {
            dataGrid.Items.MoveCurrentTo(null);
            var helper = new PrivateAccessHelper<DataGridCurrentItemService, DataGridCurrentItemService_Accessor>(target); var provider = helper.Target;
            var currentChangedEventHandlerHelper = new EventHandlerTestHelper<object, EventArgs>("CurrentChanged");
            provider.CurrentChanged += currentChangedEventHandlerHelper.Handler;
            dataGrid.CurrentCell = new DataGridCellInfo(dataList[0], dataGrid.Columns[0]);
            Assert.IsTrue(currentChangedEventHandlerHelper.HandlerInvoked);
            currentChangedEventHandlerHelper.Reset();

            // Move to another line.
            dataGrid.CurrentCell = new DataGridCellInfo(dataList[1], dataGrid.Columns[0]);
            Assert.IsTrue(currentChangedEventHandlerHelper.HandlerInvoked);
            currentChangedEventHandlerHelper.Reset();


            // Move to another cell on the same line
            dataGrid.CurrentCell = new DataGridCellInfo(dataList[1], dataGrid.Columns[1]);
            Assert.IsFalse(currentChangedEventHandlerHelper.HandlerInvoked);
            currentChangedEventHandlerHelper.Reset();

            // Move to another line using the provider.
            provider.MoveCurrentTo(dataList[2]);
            Assert.IsTrue(currentChangedEventHandlerHelper.HandlerInvoked);

            provider.CurrentChanged -= currentChangedEventHandlerHelper.Handler;
         }
      }

      /// <summary>
      ///A test for CurrentItem
      ///</summary>
      [TestMethod()]
      public void CurrentItemTest()
      {
         ObservableCollection<TestData> dataList = new ObservableCollection<TestData>()
         {
            new TestData() { StrValue = "A" },
            new TestData() { StrValue = "B" },
            new TestData() { StrValue = "C" }
         };

         DataGrid dataGrid;
         var target = new DataGridCurrentItemService();
         var serviceList = new UIServiceCollection();
         serviceList.Add(target);
         using (TestWindow.Show(dataList, serviceList, out dataGrid))
         {
            dataGrid.Items.MoveCurrentTo(null);
            var helper = new PrivateAccessHelper<DataGridCurrentItemService, DataGridCurrentItemService_Accessor>(target); var provider = helper.Target;

            Assert.IsNull(provider.CurrentItem);
            Assert.AreEqual(-1, provider.CurrentPosition);

            Assert.IsTrue(provider.MoveCurrentToFirst());
            Assert.AreSame(dataList[0], provider.CurrentItem);
            Assert.AreSame(dataGrid.CurrentItem, provider.CurrentItem);
            Assert.AreEqual(0, provider.CurrentPosition);

            Assert.IsTrue(provider.MoveCurrentToNext());
            Assert.AreSame(dataList[1], provider.CurrentItem);
            Assert.AreSame(dataGrid.CurrentItem, provider.CurrentItem);
            Assert.AreEqual(1, provider.CurrentPosition);

            Assert.IsTrue(provider.MoveCurrentToLast());
            Assert.AreSame(dataList[2], provider.CurrentItem);
            Assert.AreSame(dataGrid.CurrentItem, provider.CurrentItem);
            Assert.AreEqual(2, provider.CurrentPosition);

            Assert.IsFalse(provider.MoveCurrentToNext());
            Assert.IsNull(provider.CurrentItem);
            Assert.AreSame(dataGrid.CurrentItem, provider.CurrentItem);
            Assert.AreEqual(dataGrid.Items.Count, provider.CurrentPosition);

            dataGrid.CurrentItem = dataList[1];
            Assert.AreSame(dataList[1], provider.CurrentItem);
            Assert.AreSame(dataGrid.CurrentItem, provider.CurrentItem);
            Assert.AreEqual(1, provider.CurrentPosition);

            dataGrid.CurrentItem = null;
            Assert.IsNull(provider.CurrentItem);
            Assert.AreSame(dataGrid.CurrentItem, provider.CurrentItem);
            Assert.AreEqual(-1, provider.CurrentPosition);
         }
      }

   }


}
