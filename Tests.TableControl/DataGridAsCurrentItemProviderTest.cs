using MagicSoftware.Common.Controls.Table.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Tests.Common;
using System.Windows;

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
      ///A test for DataGridAsCurrentItemProvider Constructor
      ///</summary>
      //[TestMethod()]
      public void DataGridAsCurrentItemProviderConstructorTest()
      {
         ObservableCollection<TestData> dataList = new ObservableCollection<TestData>()
         {
            new TestData() { StrValue = "A" },
            new TestData() { StrValue = "B" },
            new TestData() { StrValue = "C" }
         };

         var w = new TestWindow();
         w.DataContext = new ListCollectionView(dataList);

         using (TestUtils.AutoCloseWindow(w))
         {
            DataGrid dataGrid = w.MainDataGrid;
            DataGridAsCurrentItemProvider target = new DataGridAsCurrentItemProvider(dataGrid);
         }
      }

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

         var w = new TestWindow();
         w.DataContext = new ListCollectionView(dataList);

         using (TestUtils.AutoCloseWindow(w))
         {
            var dataGrid = w.MainDataGrid;
            dataGrid.CurrentItem = null;
            var helper = new PrivateAccessHelper<DataGridAsCurrentItemProvider, DataGridAsCurrentItemProvider_Accessor>(new DataGridAsCurrentItemProvider(dataGrid));
            var provider = helper.Target;
            var previewCurrentChangingEventHandlerHelper = new EventHandlerTestHelper<object, CancelableRoutedEventArgs>("PreviewCurrentChanging");
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
      ///A test for Dispose
      ///</summary>
      [TestMethod()]
      public void DisposeTest()
      {
         DataGrid dataGrid = null; // TODO: Initialize to an appropriate value
         DataGridAsCurrentItemProvider target = new DataGridAsCurrentItemProvider(dataGrid); // TODO: Initialize to an appropriate value
         target.Dispose();
         Assert.Inconclusive("A method that does not return a value cannot be verified.");
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

         var w = new TestWindow();
         w.DataContext = new ListCollectionView(dataList);

         using (TestUtils.AutoCloseWindow(w))
         {
            var dataGrid = w.MainDataGrid;
            dataGrid.Items.MoveCurrentTo(null);
            var helper = new PrivateAccessHelper<DataGridAsCurrentItemProvider, DataGridAsCurrentItemProvider_Accessor>(new DataGridAsCurrentItemProvider(dataGrid));
            var provider = helper.Target;
            var previewCurrentChangingEventHandlerHelper = new EventHandlerTestHelper<object, CancelableRoutedEventArgs>("PreviewCurrentChanging");
            provider.PreviewCurrentChanging += previewCurrentChangingEventHandlerHelper.Handler;
            var currentChangedEventHandlerHelper = new EventHandlerTestHelper<object, RoutedEventArgs>("CurrentChanged");
            provider.CurrentChanged += currentChangedEventHandlerHelper.Handler;

            provider.MoveCurrentTo(dataList[1]);
            Assert.IsTrue(previewCurrentChangingEventHandlerHelper.HandlerInvoked);
            Assert.IsTrue(currentChangedEventHandlerHelper.HandlerInvoked);
            previewCurrentChangingEventHandlerHelper.Reset();
            currentChangedEventHandlerHelper.Reset();

            // Block movement.
            previewCurrentChangingEventHandlerHelper.AdditionalHandling = (sender, args) => { args.Canceled = true; };
            provider.MoveCurrentTo(dataList[2]);
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

         var w = new TestWindow();
         w.DataContext = new ListCollectionView(dataList);

         using (TestUtils.AutoCloseWindow(w))
         {
            var dataGrid = w.MainDataGrid;
            dataGrid.Items.MoveCurrentTo(null);
            var helper = new PrivateAccessHelper<DataGridAsCurrentItemProvider, DataGridAsCurrentItemProvider_Accessor>(new DataGridAsCurrentItemProvider(dataGrid));
            var provider = helper.Target;
            var currentChangedEventHandlerHelper = new EventHandlerTestHelper<object, RoutedEventArgs>("CurrentChanged");
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
         // This test should verify that the provider's CurrentItem follows the data grid's state:
         // 1) Immediately after creation
         // 2) When changing the current item using the provider (MoveCurrent())
         // 3) When the data grid changes the current item.
         Assert.Inconclusive("Verify the correctness of this test method.");
      }

      /// <summary>
      ///A test for CurrentPosition
      ///</summary>
      [TestMethod()]
      public void CurrentPositionTest()
      {
         // Same as CurrentItemTest. Maybe should be consolidated.
         Assert.Inconclusive("Verify the correctness of this test method.");
      }
   }


}
