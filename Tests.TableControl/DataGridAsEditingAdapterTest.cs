using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Controls;
using MagicSoftware.Common.Controls.Table.Extensions;
using System.Collections.Generic;
using System.Windows;
using Tests.Common;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Tests.TableControl
{


   /// <summary>
   ///This is a test class for DataGridAsEditingAdapterTest and is intended
   ///to contain all DataGridAsEditingAdapterTest Unit Tests
   ///</summary>
   [TestClass()]
   public class DataGridAsEditingAdapterTest
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
      ///A test for DataGridAsEditingAdapter Constructor
      ///</summary>
      [TestMethod()]
      public void DataGridAsEditingAdapterConstructorTest()
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
            dataGrid.CurrentCell = new System.Windows.Controls.DataGridCellInfo(dataList[0], dataGrid.Columns[0]);

            PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor> helper = new PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor>(new EnhancedDGProxy());
            helper.Invoke("AttachTo", dataGrid);
            DataGridAsEditingAdapter target;
            
            // Instantiate a new editing adapter and verify its IsEditing property is correct.
            using (target = new DataGridAsEditingAdapter(dataGrid, helper.Target))
            {
               Assert.IsFalse(target.IsEditing);
            }

            // Begin editing on the data grid and verify that a _newly_ created adapter will show
            // that the grid is editing.
            if (!dataGrid.BeginEdit())
               Assert.Inconclusive("Could not begin edit on data grid.");
            using (target = new DataGridAsEditingAdapter(dataGrid, helper.Target))
            {
               Assert.IsTrue(target.IsEditing);
            }

            // Now commit the row to exit editing.
            if (!dataGrid.CommitEdit(DataGridEditingUnit.Row, true))
               Assert.Inconclusive("Could not commit edit on data grid.");

            using (target = new DataGridAsEditingAdapter(dataGrid, helper.Target))
            {
               Assert.IsFalse(target.IsEditing);
            }

         }
      }

      /// <summary>
      ///A test for BeginEdit
      ///</summary>
      [TestMethod()]
      public void BeginEditTest()
      {
         DataGrid dataGrid = null; // TODO: Initialize to an appropriate value
         EnhancedDGProxy dataGridProxy = null; // TODO: Initialize to an appropriate value
         DataGridAsEditingAdapter target = new DataGridAsEditingAdapter(dataGrid, dataGridProxy); // TODO: Initialize to an appropriate value
         bool expected = false; // TODO: Initialize to an appropriate value
         bool actual;
         actual = target.BeginEdit();
         Assert.AreEqual(expected, actual);
         Assert.Inconclusive("Verify the correctness of this test method.");
      }

      /// <summary>
      ///A test for CancelEdit
      ///</summary>
      [TestMethod()]
      public void CancelEditTest()
      {
         DataGrid dataGrid = null; // TODO: Initialize to an appropriate value
         EnhancedDGProxy dataGridProxy = null; // TODO: Initialize to an appropriate value
         DataGridAsEditingAdapter target = new DataGridAsEditingAdapter(dataGrid, dataGridProxy); // TODO: Initialize to an appropriate value
         bool expected = false; // TODO: Initialize to an appropriate value
         bool actual;
         actual = target.CancelEdit();
         Assert.AreEqual(expected, actual);
         Assert.Inconclusive("Verify the correctness of this test method.");
      }

      /// <summary>
      ///A test for CommitEdit
      ///</summary>
      [TestMethod()]
      public void CommitEditTest()
      {
         DataGrid dataGrid = null; // TODO: Initialize to an appropriate value
         EnhancedDGProxy dataGridProxy = null; // TODO: Initialize to an appropriate value
         DataGridAsEditingAdapter target = new DataGridAsEditingAdapter(dataGrid, dataGridProxy); // TODO: Initialize to an appropriate value
         bool expected = false; // TODO: Initialize to an appropriate value
         bool actual;
         actual = target.CommitEdit();
         Assert.AreEqual(expected, actual);
         Assert.Inconclusive("Verify the correctness of this test method.");
      }

      /// <summary>
      ///A test for Dispose
      ///</summary>
      [TestMethod()]
      public void DisposeTest()
      {
         DataGrid dataGrid = null; // TODO: Initialize to an appropriate value
         EnhancedDGProxy dataGridProxy = null; // TODO: Initialize to an appropriate value
         DataGridAsEditingAdapter target = new DataGridAsEditingAdapter(dataGrid, dataGridProxy); // TODO: Initialize to an appropriate value
         target.Dispose();
         Assert.Inconclusive("A method that does not return a value cannot be verified.");
      }

      /// <summary>
      ///A test for CurrentEdit
      ///</summary>
      [TestMethod()]
      [DeploymentItem("MagicSoftware.Common.Controls.Table.dll")]
      public void CurrentEditTest()
      {
         PrivateObject param0 = null; // TODO: Initialize to an appropriate value
         DataGridAsEditingAdapter_Accessor target = new DataGridAsEditingAdapter_Accessor(param0); // TODO: Initialize to an appropriate value
         object expected = null; // TODO: Initialize to an appropriate value
         object actual;
         target.CurrentEdit = expected;
         actual = target.CurrentEdit;
         Assert.AreEqual(expected, actual);
         Assert.Inconclusive("Verify the correctness of this test method.");
      }

      /// <summary>
      ///A test for IsEditing
      ///</summary>
      [TestMethod()]
      [DeploymentItem("MagicSoftware.Common.Controls.Table.dll")]
      public void IsEditingTest()
      {
         PrivateObject param0 = null; // TODO: Initialize to an appropriate value
         DataGridAsEditingAdapter_Accessor target = new DataGridAsEditingAdapter_Accessor(param0); // TODO: Initialize to an appropriate value
         bool expected = false; // TODO: Initialize to an appropriate value
         bool actual;
         target.IsEditing = expected;
         actual = target.IsEditing;
         Assert.AreEqual(expected, actual);
         Assert.Inconclusive("Verify the correctness of this test method.");
      }
   }

   class TestData : IEditableObject
   {
      public string StrValue { get; set; }

      #region IEditableObject Members

      public void BeginEdit()
      {

      }

      public void CancelEdit()
      {

      }

      public void EndEdit()
      {

      }

      #endregion
   }


}
