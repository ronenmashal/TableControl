using MagicSoftware.Common.Controls.Table.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tests.Common;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Tests.TableControl.UI;

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

      ObservableCollection<TestData> dataList = new ObservableCollection<TestData>()
         {
            new TestData() { StrValue = "A" },
            new TestData() { StrValue = "B" },
            new TestData() { StrValue = "C" }
         };


      /// <summary>
      ///A test for DataGridCurrentCellService Constructor
      ///</summary>
      [TestMethod()]
      public void DataGridCurrentCellServiceConstructorTest()
      {
         DataGrid dataGrid;
         using (TestWindow.Show(dataList, out dataGrid))
         {
            DataGridCurrentCellService target = new DataGridCurrentCellService();
            target.SetElement(dataGrid);
            Assert.IsNull(target.CurrentCell.Item);
            Assert.IsNull(target.CurrentCell.CellElementLocator);

            dataGrid.CurrentCell = new DataGridCellInfo(dataList[1], dataGrid.Columns[0]);
            target = new DataGridCurrentCellService();
            target.SetElement(dataGrid);
            Assert.AreSame(dataList[1], target.CurrentCell.Item);
            Assert.IsInstanceOfType(target.CurrentCell.CellElementLocator, typeof(FindCellByColumn));
            //var cell = target.CurrentCell.CellElementLocator.GetCell();
         }
      }

      /// <summary>
      ///A test for MoveDown
      ///</summary>
      [TestMethod()]
      public void MoveDownTest()
      {
         DataGridCurrentCellService target = new DataGridCurrentCellService(); // TODO: Initialize to an appropriate value
         int distance = 0; // TODO: Initialize to an appropriate value
         bool expected = false; // TODO: Initialize to an appropriate value
         bool actual;
         actual = target.MoveDown(distance);
         Assert.AreEqual(expected, actual);
         Assert.Inconclusive("Verify the correctness of this test method.");
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
         UniformCellInfo targetCellInfo = null; // TODO: Initialize to an appropriate value
         bool expected = false; // TODO: Initialize to an appropriate value
         bool actual;
         actual = target.MoveTo(targetCellInfo);
         Assert.AreEqual(expected, actual);
         Assert.Inconclusive("Verify the correctness of this test method.");
      }

      /// <summary>
      ///A test for MoveUp
      ///</summary>
      [TestMethod()]
      public void MoveUpTest()
      {
         DataGridCurrentCellService target = new DataGridCurrentCellService(); // TODO: Initialize to an appropriate value
         int distance = 0; // TODO: Initialize to an appropriate value
         bool expected = false; // TODO: Initialize to an appropriate value
         bool actual;
         actual = target.MoveUp(distance);
         Assert.AreEqual(expected, actual);
         Assert.Inconclusive("Verify the correctness of this test method.");
      }

      /// <summary>
      ///A test for CurrentCell
      ///</summary>
      [TestMethod()]
      public void CurrentCellTest()
      {
         DataGridCurrentCellService target = new DataGridCurrentCellService(); // TODO: Initialize to an appropriate value
         UniformCellInfo actual;
         actual = target.CurrentCell;
         Assert.Inconclusive("Verify the correctness of this test method.");
      }

   }


}
