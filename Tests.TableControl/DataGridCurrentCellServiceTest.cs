using MagicSoftware.Common.Controls.Table.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tests.Common;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Tests.TableControl.UI;
using Tests.TableControl.Data;
using System.Windows.Threading;
using System.Windows;

namespace Tests.TableControl
{


   /// <summary>
   ///This is a test class for DataGridCurrentCellServiceTest and is intended
   ///to contain all DataGridCurrentCellServiceTest Unit Tests
   ///</summary>
   [TestClass()]
   public class DataGridCurrentCellServiceTest
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
      /// A test for DataGridCurrentCellService Constructor: Ensures that when the service is created
      /// it has the correct state as the data grid.
      ///</summary>
      [TestMethod()]
      public void DataGridCurrentCellServiceConstructorTest()
      {
         // Create a large enough data list, so that there will be scroll bar.
         var dataList = CreateTestDataList(100);

         DataGrid dataGrid;
         using (TestWindow.Show(dataList, out dataGrid))
         {
            ICurrentCellService target = new DataGridCurrentCellService();
            ((IUIService)target).AttachToElement(dataGrid);
            Assert.IsNull(target.CurrentCell.Item);

            dataGrid.CurrentCell = new DataGridCellInfo(dataList[1], dataGrid.Columns[0]);
            target = new DataGridCurrentCellService();
            ((IUIService)target).AttachToElement(dataGrid);
            Assert.AreSame(dataList[1], target.CurrentCell.Item);

            dataGrid.ScrollIntoView(dataList[70]);
            dataGrid.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() =>
            {
               dataGrid.CurrentCell = new DataGridCellInfo(dataList[70], dataGrid.Columns[0]);
            }));

            target = new DataGridCurrentCellService();
            ((IUIService)target).AttachToElement(dataGrid);
            Assert.AreSame(dataList[70], target.CurrentCell.Item);
         }
      }

      /// <summary>
      /// Verifies the DataGridCurrentCellService tracks the data grid's current cell correctly.
      /// </summary>
      [TestMethod()]
      public void CurrentCellTrackingTest()
      {
         // Create a large enough data list, so that there will be scroll bar.
         var dataList = CreateTestDataList(100);

         DataGrid dataGrid;
         using (TestWindow.Show(dataList, out dataGrid))
         {
            ICurrentCellService target = new DataGridCurrentCellService();
            ((IUIService)target).AttachToElement(dataGrid);

            Assert.IsNull(target.CurrentCell.Item);

            using (ExpectNonCancelableEvents(target))
               dataGrid.CurrentCell = new DataGridCellInfo(dataList[1], dataGrid.Columns[0]);
            Assert.AreSame(dataList[1], target.CurrentCell.Item);
            Assert.AreEqual(0, target.CurrentCell.CellIndex);

            using (ExpectNonCancelableEvents(target))
               dataGrid.CurrentCell = new DataGridCellInfo(dataList[5], dataGrid.Columns[2]);
            Assert.AreSame(dataList[5], target.CurrentCell.Item);
            Assert.AreEqual(2, target.CurrentCell.CellIndex);

            using (ExpectNoEvents(target))
               dataGrid.CurrentColumn.DisplayIndex = 0;
            Assert.AreEqual(0, target.CurrentCell.CellIndex);
         }
      }

      /// <summary>
      ///A test for MoveDown
      ///</summary>
      [TestMethod()]
      public void MoveDownTest()
      {
         // Create a large enough data list, so that there will be scroll bar.
         var dataList = CreateTestDataList(100);

         DataGrid dataGrid;
         using (TestWindow.Show(dataList, out dataGrid))
         {
            ICurrentCellService target = new DataGridCurrentCellService();
            ((IUIService)target).AttachToElement(dataGrid);

            // Move down, when cell was not yet set, should move to the first cell.
            using (ExpectCancelableEvents(target))
               Assert.IsTrue(target.MoveDown(1));
            Assert.AreSame(dataList[0], target.CurrentCell.Item);
            Assert.AreSame(dataGrid.CurrentItem, target.CurrentCell.Item);
            Assert.IsNotNull(target.CurrentCell.Item);
            Assert.AreSame(dataGrid.ColumnFromDisplayIndex(0), dataGrid.CurrentCell.Column);

            using (ExpectCancelableEvents(target))
               Assert.IsTrue(target.MoveDown(5));
            Assert.AreSame(dataList[5], target.CurrentCell.Item);
            Assert.AreSame(dataList[5], dataGrid.CurrentCell.Item);

            using (ExpectCancelableEvents(target))
               Assert.IsTrue(target.MoveDown(40));
            Assert.AreSame(dataList[45], dataGrid.CurrentCell.Item);

            using (ExpectNoEvents(target))
               Assert.IsFalse(target.MoveDown(200));
            Assert.IsNotNull(target.CurrentCell.Item);

            // Move to the last item
            Assert.IsTrue(target.MoveDown((uint)(dataGrid.Items.Count - 45 - 1)));
            // Try to move one more item down
            using (ExpectNoEvents(target))
               Assert.IsFalse(target.MoveDown(1));
         }
      }

      /// <summary>
      ///A test for MoveLeft
      ///</summary>
      [TestMethod()]
      public void MoveLeftTest()
      {
         DataGridCurrentCellService target = new DataGridCurrentCellService(); // TODO: Initialize to an appropriate value
         uint distance = 0; // TODO: Initialize to an appropriate value
         bool expected = false; // TODO: Initialize to an appropriate value
         bool actual;
         actual = target.MoveLeft(distance);
         Assert.AreEqual(expected, actual);
         Assert.Inconclusive("Verify the correctness of this test method.");
      }

      /// <summary>
      ///A test for MoveRight
      ///</summary>
      [TestMethod()]
      public void MoveRightTest()
      {
         DataGridCurrentCellService target = new DataGridCurrentCellService(); // TODO: Initialize to an appropriate value
         uint distance = 0; // TODO: Initialize to an appropriate value
         bool expected = false; // TODO: Initialize to an appropriate value
         bool actual;
         actual = target.MoveRight(distance);
         Assert.AreEqual(expected, actual);
         Assert.Inconclusive("Verify the correctness of this test method.");
      }

      /// <summary>
      ///A test for MoveTo
      ///</summary>
      [TestMethod()]
      public void MoveToTest()
      {
         DataGridCurrentCellService target = new DataGridCurrentCellService(); // TODO: Initialize to an appropriate value
         //CellElementLocator targetCellInfo = null; // TODO: Initialize to an appropriate value
         bool expected = false; // TODO: Initialize to an appropriate value
         bool actual = false; ;
         //actual = target.MoveTo(targetCellInfo);
         Assert.AreEqual(expected, actual);
         Assert.Inconclusive("Verify the correctness of this test method.");
      }

      /// <summary>
      ///A test for MoveUp
      ///</summary>
      [TestMethod()]
      public void MoveUpTest()
      {
         // Create a large enough data list, so that there will be scroll bar.
         var dataList = CreateTestDataList(100);

         DataGrid dataGrid;
         using (TestWindow.Show(dataList, out dataGrid))
         {
            ICurrentCellService target = new DataGridCurrentCellService();
            ((IUIService)target).AttachToElement(dataGrid);
            var currentRowChangingEventHelper = new EventHandlerTestHelper<object, PreviewChangeEventArgs>("PreviewCurrentCellChanging");
            target.PreviewCurrentCellChanging += currentRowChangingEventHelper.Handler;
            var rowChangedEventHelper = new EventHandlerTestHelper<object, EventArgs>("CurrentCellChanged");
            target.CurrentCellChanged += rowChangedEventHelper.Handler;

            using (ExpectNoEvents(target))
            {
               Assert.IsFalse(target.MoveUp(1));
               Assert.IsNull(target.CurrentCell.Item);
            }
         }
      }

      [TestMethod()]
      public void CellVisibilityTest()
      {
         Assert.Inconclusive("Undone yet...");
      }

      /// <summary>
      ///A test for CurrentCell
      ///</summary>
      [TestMethod()]
      public void CurrentCellTest()
      {
         DataGridCurrentCellService target = new DataGridCurrentCellService(); // TODO: Initialize to an appropriate value
         //CellElementLocator actual;
         //actual = target.CurrentCell;
         Assert.Inconclusive("Verify the correctness of this test method.");
      }



      class TestData
      {
         public string StrValue { get; set; }
         public int IntValue { get; set; }
         public bool BoolValue { get; set; }
      }

      ObservableCollection<TestData> CreateTestDataList(int size)
      {
         ObservableCollection<TestData> list = new ObservableCollection<TestData>();
         for (int i = 0; i < size; i++)
         {
            list.Add(new TestData() { StrValue = "Item #" + i, IntValue = i, BoolValue = (i % 2) == 0 });
         }
         return list;
      }

      IDisposable ExpectNoEvents(ICurrentCellService target)
      {
         return new CurrentCellServiceEventsHelper(target, new CurrentCellServiceEventsHelper.EventInvocationValidator((pc, c) =>
            {
               Assert.IsFalse(pc.HandlerInvoked);
               Assert.IsFalse(c.HandlerInvoked);
            }));
      }

      IDisposable ExpectNonCancelableEvents(ICurrentCellService target)
      {
         return new CurrentCellServiceEventsHelper(target, new CurrentCellServiceEventsHelper.EventInvocationValidator((pc, c) =>
            {
               Assert.AreEqual(1, pc.HandlerInvocationCount);
               Assert.IsFalse(pc.LastInocationEventArgs.IsCancelable);
               Assert.AreEqual(1, c.HandlerInvocationCount);
            }));
      }

      IDisposable ExpectCancelableEvents(ICurrentCellService target)
      {
         return new CurrentCellServiceEventsHelper(target, new CurrentCellServiceEventsHelper.EventInvocationValidator((pc, c) =>
            {
               Assert.AreEqual(1, pc.HandlerInvocationCount);
               Assert.IsTrue(pc.LastInocationEventArgs.IsCancelable);
               Assert.AreEqual(1, c.HandlerInvocationCount);
            }));
      }

      class CurrentCellServiceEventsHelper : IDisposable
      {
         public delegate void EventInvocationValidator(EventHandlerTestHelper<object, PreviewChangeEventArgs> cellChangingEventHelper, EventHandlerTestHelper<object, EventArgs> cellChangedEventHelper);

         EventHandlerTestHelper<object, PreviewChangeEventArgs> cellChangingEventHelper = new EventHandlerTestHelper<object, PreviewChangeEventArgs>("PreviewCurrentCellChanging");
         EventHandlerTestHelper<object, EventArgs> cellChangedEventHelper = new EventHandlerTestHelper<object, EventArgs>("CurrentCellChanged");
         ICurrentCellService target;

         private EventInvocationValidator assertConditions;

         public CurrentCellServiceEventsHelper(ICurrentCellService target, EventInvocationValidator assertConditions)
         {
            this.target = target;
            this.assertConditions = assertConditions;
            target.PreviewCurrentCellChanging += cellChangingEventHelper.Handler;
            target.CurrentCellChanged += cellChangedEventHelper.Handler;
         }

         public EventHandler<PreviewChangeEventArgs> ChangingEventCallback
         {
            set { cellChangingEventHelper.AdditionalHandling = value; }
         }

         #region IDisposable Members

         public void Dispose()
         {
            assertConditions(cellChangingEventHelper, cellChangedEventHelper);

            target.PreviewCurrentCellChanging += cellChangingEventHelper.Handler;
            target.CurrentCellChanged += cellChangedEventHelper.Handler;
         }

         #endregion
      }
   }
}
