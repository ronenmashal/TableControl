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
      ///A test for DataGridCurrentCellService Constructor
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
            Assert.IsNull(target.CurrentCell);
            Assert.IsNull(target.CurrentCellItem);

            dataGrid.CurrentCell = new DataGridCellInfo(dataList[1], dataGrid.Columns[0]);
            target = new DataGridCurrentCellService();
            ((IUIService)target).AttachToElement(dataGrid);
            //Assert.AreSame(dataList[1], target.CurrentCell);
            Assert.IsInstanceOfType(target.CurrentCell, typeof(DataGridCell));
            //var cell = target.CurrentCell.CellElementLocator.GetCell();

            dataGrid.ScrollIntoView(dataList[70]);
            dataGrid.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() =>
            {
               dataGrid.CurrentCell = new DataGridCellInfo(dataList[70], dataGrid.Columns[0]);
            }));

            target = new DataGridCurrentCellService();
            ((IUIService)target).AttachToElement(dataGrid);
            Assert.AreSame(dataList[70], target.CurrentCellItem);
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
            ICurrentItemService currentRowService = UIServiceProvider.GetService<ICurrentItemService>(dataGrid);
            var currentRowChangingEventHelper = new EventHandlerTestHelper<object, CancelableRoutedEventArgs>("PreviewCurrentCellChanging");
            currentRowService.PreviewCurrentChanging += currentRowChangingEventHelper.Handler;
            var rowChangedEventHelper = new EventHandlerTestHelper<object, RoutedEventArgs>("CurrentCellChanged");
            currentRowService.CurrentChanged += rowChangedEventHelper.Handler;

            // Move down, when cell was not yet set, should move to the first cell.
            Assert.IsTrue(target.MoveDown(1));
            Assert.AreSame(dataList[0], target.CurrentCellItem);
            Assert.IsNotNull(target.CurrentCell);
            Assert.IsTrue(rowChangedEventHelper.HandlerInvoked);
            Assert.IsTrue(currentRowChangingEventHelper.HandlerInvoked);
            Assert.AreSame(dataGrid.ColumnFromDisplayIndex(0), dataGrid.CurrentCell.Column);

            Assert.IsTrue(target.MoveDown(5));
            Assert.AreSame(dataList[5], target.CurrentCellItem);
            Assert.AreSame(dataList[5], dataGrid.CurrentCell.Item);

            Assert.IsTrue(target.MoveDown(40));
            Assert.IsTrue(target.IsCellVisible);
            Assert.AreSame(dataList[45], dataGrid.CurrentCell.Item);

            Assert.IsFalse(target.MoveDown(200));
            Assert.IsFalse(target.IsCellVisible);
            Assert.IsNull(target.CurrentCell);
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
         int distance = 0; // TODO: Initialize to an appropriate value
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
         int distance = 0; // TODO: Initialize to an appropriate value
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
            ICurrentItemService currentRowService = UIServiceProvider.GetService<ICurrentItemService>(dataGrid);
            var currentRowChangingEventHelper = new EventHandlerTestHelper<object, CancelableRoutedEventArgs>("PreviewCurrentCellChanging");
            currentRowService.PreviewCurrentChanging += currentRowChangingEventHelper.Handler;
            var rowChangedEventHelper = new EventHandlerTestHelper<object, RoutedEventArgs>("CurrentCellChanged");
            currentRowService.CurrentChanged += rowChangedEventHelper.Handler;

            Assert.IsFalse(target.MoveUp(1));
            Assert.IsNull(target.CurrentCellItem);
            Assert.IsFalse(rowChangedEventHelper.HandlerInvoked);
            Assert.IsFalse(currentRowChangingEventHelper.HandlerInvoked);

            Assert.IsTrue(target.MoveDown(5));
            Assert.AreSame(dataList[4], target.CurrentCellItem);
            Assert.AreSame(dataList[5], dataGrid.CurrentCell.Item);

            Assert.IsTrue(target.MoveDown(40));
            Assert.IsTrue(target.IsCellVisible);
            Assert.AreSame(dataList[45], dataGrid.CurrentCell.Item);

            Assert.IsFalse(target.MoveDown(200));
            Assert.IsFalse(target.IsCellVisible);
            Assert.IsNull(target.CurrentCell);
            Assert.IsFalse(target.MoveDown(1));
         }
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

      Company CreateCompany()
      {
         Company c = new Company() { Name = "Bit Cranching Inc.", Manager = new Employee() { Name = "EMP1", Address = "HOME1", Age = 50, IsActive = true }, Ticker = "BITC" };

         Department d = new Department() { Name = "Marketing", Manager = new Employee() { Name = "MARK MGR", Address = "MARK HOME", Age = 45, IsActive = true } };
         d.AddMember(new Employee() { Name = "Sell Better", Address = "On the road, 19", Age = 40, IsActive = true });
         d.AddMember(new Employee() { Name = "NotSelling Much", Address = "Around the office", Age = 40, IsActive = false });
         c.Add(d);

         d = new Department() { Name = "Development", Manager = new Employee() { Name = "DEV MGR", Address = "Dev HOME", Age = 45, IsActive = true } };
         d.AddMember(new Employee() { Name = "Bit Bit", Address = "In the office", Age = 40, IsActive = true });
         d.AddMember(new Employee() { Name = "Byte Byte", Address = "Under the desk", Age = 40, IsActive = true });
         d.AddMember(new Employee() { Name = "Java Java", Address = "Cell Phone", Age = 40, IsActive = true });
         d.AddMember(new Employee() { Name = "Proxy Proxy", Address = "Don't know", Age = 40, IsActive = true });
         d.AddMember(new Employee() { Name = "Nub Nub", Address = "Kitchen", Age = 25, IsActive = true });
         c.Add(d);


         return c;
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

      class CurrentCellConsumer
      {
         ICurrentItemService currentRowService;
         ICurrentItemService currentCellService;

         public CurrentCellConsumer(DataGrid dataGrid)
         {
            currentRowService = UIServiceProvider.GetService<ICurrentItemService>(dataGrid);
            UpdateCurrentCellService();
            currentRowService.PreviewCurrentChanging += new CancelableRoutedEventHandler(currentRowService_PreviewCurrentChanging);
            currentRowService.CurrentChanged += new RoutedEventHandler(currentRowService_CurrentChanged);
         }

         private void UpdateCurrentCellService()
         {
            /*currentC*/
         }

         void currentRowService_PreviewCurrentChanging(object sender, CancelableRoutedEventArgs eventArgs)
         {
            throw new NotImplementedException();
         }

         void currentRowService_CurrentChanged(object sender, RoutedEventArgs e)
         {
            throw new NotImplementedException();
         }

      }
   }


}
