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
            PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor> helper = new PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor>(new EnhancedDGProxy());
            helper.Invoke("AttachTo", dataGrid);
            DataGridAsEditingAdapter target;

            using (target = new DataGridAsEditingAdapter(dataGrid, helper.Target))
            {
               using (var eventsSink = new DGEventsSink(dataGrid))
               {
                  Assert.IsFalse(target.BeginEdit(), "Begin edit should have failed, because the data grid has no selected cell.");
                  Assert.IsFalse(eventsSink.BeginningEditEventRaised);

                  // Select a cell eligible for editing.
                  dataGrid.CurrentCell = new System.Windows.Controls.DataGridCellInfo(dataList[0], dataGrid.Columns[0]);
                  Assert.IsTrue(target.BeginEdit(), "Begin edit should have succeeded.");
                  Assert.IsTrue(eventsSink.BeginningEditEventRaised);
                  Assert.IsTrue(target.IsEditing);

                  eventsSink.Reset();
                  Assert.IsTrue(target.BeginEdit(), "Although no new editing began, begin edit should return true - meaning the grid is in edit mode.");
                  Assert.IsFalse(eventsSink.BeginningEditEventRaised);
               }
            }
         }
      }

      /// <summary>
      ///A test for CancelEdit
      ///</summary>
      [TestMethod()]
      public void CancelEditTest()
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
            PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor> helper = new PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor>(new EnhancedDGProxy());
            helper.Invoke("AttachTo", dataGrid);
            DataGridAsEditingAdapter target;

            using (target = new DataGridAsEditingAdapter(dataGrid, helper.Target))
            {
               // Begin edit and ensure the grid is indeed in edit mode.
               dataGrid.CurrentCell = new System.Windows.Controls.DataGridCellInfo(dataList[0], dataGrid.Columns[0]);
               Assert.IsTrue(target.BeginEdit(), "Begin edit should have succeeded.");
               Assert.IsTrue(target.IsEditing);

               using (var eventsSink = new DGEventsSink(dataGrid))
               {
                  Assert.IsTrue(target.CancelEdit());
                  Assert.IsTrue(eventsSink.RowEditEndingEventRaised);
                  Assert.IsTrue(eventsSink.CellEditEndingEventRaised);

                  eventsSink.Reset();
                  Assert.IsTrue(target.CancelEdit());
                  Assert.IsFalse(eventsSink.RowEditEndingEventRaised);
                  Assert.IsFalse(eventsSink.CellEditEndingEventRaised);
               }
            }
         }
      }

      /// <summary>
      ///A test for CommitEdit
      ///</summary>
      [TestMethod()]
      public void CommitEditTest()
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
            PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor> helper = new PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor>(new EnhancedDGProxy());
            helper.Invoke("AttachTo", dataGrid);
            DataGridAsEditingAdapter target;

            using (target = new DataGridAsEditingAdapter(dataGrid, helper.Target))
            {
               // Begin edit and ensure the grid is indeed in edit mode.
               dataGrid.CurrentCell = new System.Windows.Controls.DataGridCellInfo(dataList[0], dataGrid.Columns[0]);
               Assert.IsTrue(target.BeginEdit(), "Begin edit should have succeeded.");
               Assert.IsTrue(target.IsEditing);

               using (var eventsSink = new DGEventsSink(dataGrid))
               {
                  Assert.IsTrue(target.CommitEdit());
                  Assert.IsTrue(eventsSink.RowEditEndingEventRaised);
                  Assert.IsTrue(eventsSink.CellEditEndingEventRaised);

                  eventsSink.Reset();
                  Assert.IsTrue(target.CommitEdit());
                  Assert.IsFalse(eventsSink.RowEditEndingEventRaised);
                  Assert.IsFalse(eventsSink.CellEditEndingEventRaised);
               }
            }
         }
      }

      /// <summary>
      ///A test for Dispose
      ///</summary>
      [TestMethod()]
      public void DisposeTest()
      {
         ObservableCollection<TestData> dataList = new ObservableCollection<TestData>()
         {
            new TestData() { StrValue = "A" },
            new TestData() { StrValue = "B" },
            new TestData() { StrValue = "C" }
         };

         var w = new TestWindow();
         w.DataContext = new ListCollectionView(dataList);
         WeakReference dataGridReference = new WeakReference(w.MainDataGrid);

         using (TestUtils.AutoCloseWindow(w))
         {
            DataGridAsEditingAdapter target;
            DataGrid dataGrid = w.MainDataGrid;
            dataGrid.CurrentCell = new System.Windows.Controls.DataGridCellInfo(dataList[0], dataGrid.Columns[0]);
            PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor> proxyHelper = new PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor>(new EnhancedDGProxy());
            proxyHelper.Invoke("AttachTo", dataGrid);
            var adapterHelper = new PrivateAccessHelper<DataGridAsEditingAdapter, DataGridAsEditingAdapter_Accessor>(new DataGridAsEditingAdapter(dataGrid, proxyHelper.Target));
            target = adapterHelper.Target;
            target.Dispose();

            Precondition(dataGrid.BeginEdit(), "Failed beginning edit");
            Assert.IsFalse(target.IsEditing);

            adapterHelper.Accessor.IsEditing = true;

            Precondition(dataGrid.CommitEdit(), "Failed to commit");
            Assert.IsTrue(target.IsEditing);

            Precondition(dataGrid.BeginEdit(), "Failed beginning edit");
            Precondition(dataGrid.CancelEdit(), "Failed to cancel");
            Assert.IsTrue(target.IsEditing);

            Assert.IsNull(adapterHelper.Accessor.dataGrid);
         }
      }

      void Precondition(bool condition, string message)
      {
         if (!condition)
            Assert.Inconclusive(message);
      }

      /// <summary>
      ///A test for CurrentEdit
      ///</summary>
      [TestMethod()]
      [DeploymentItem("MagicSoftware.Common.Controls.Table.dll")]
      public void CurrentEditTest()
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
            PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor> helper = new PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor>(new EnhancedDGProxy());
            helper.Invoke("AttachTo", dataGrid);
            DataGridAsEditingAdapter target;

            using (target = new DataGridAsEditingAdapter(dataGrid, helper.Target))
            {
               // Begin edit and ensure the grid is indeed in edit mode.
               dataGrid.CurrentCell = new DataGridCellInfo(dataList[0], dataGrid.Columns[0]);
               Precondition(target.BeginEdit(), "Begin edit should have succeeded.");

               Assert.AreEqual(dataList[0], target.CurrentEdit);
               Precondition(target.CommitEdit(), "Failed committing");
               Assert.IsNull(target.CurrentEdit);

               dataGrid.CurrentCell = new DataGridCellInfo(dataList[2], dataGrid.Columns[0]);
               Precondition(dataGrid.BeginEdit(), "Begin edit should have succeeded.");
               Assert.AreEqual(dataList[2], target.CurrentEdit);
               Precondition(target.CancelEdit(), "Failed canceling");
               Assert.IsNull(target.CurrentEdit);
            }
         }
      }
   }

   class TestData : IEditableObject
   {
      public string StrValue { get; set; }
      public int IntValue { get; set; }

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

      public override string ToString()
      {
         return String.Format("{{TestData: {0}, {1}}}", StrValue, IntValue);
      }
   }

   /// <summary>
   /// Class for handling data grid events and signaling whether they were raised or not.
   /// Designed as IDisposable to ensure detaching from the data grid when object is no longer
   /// necessary.
   /// </summary>
   class DGEventsSink : IDisposable
   {
      DataGrid dataGrid;

      public bool BeginningEditEventRaised { get; private set; }
      public bool RowEditEndingEventRaised { get; private set; }
      public bool CellEditEndingEventRaised { get; private set; }

      public DGEventsSink(DataGrid dataGrid)
      {
         this.dataGrid = dataGrid;
         dataGrid.BeginningEdit += dataGrid_BeginningEdit;
         dataGrid.CellEditEnding += dataGrid_CellEditEnding;
         dataGrid.RowEditEnding += dataGrid_RowEditEnding;
         Reset();
      }

      public void Reset()
      {
         BeginningEditEventRaised = false;
         RowEditEndingEventRaised = false;
         CellEditEndingEventRaised = false;
      }

      void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
      {
         BeginningEditEventRaised = true;
      }

      void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
      {
         RowEditEndingEventRaised = true;
      }

      void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
      {
         CellEditEndingEventRaised = true;
      }

      #region IDisposable Members

      public void Dispose()
      {
         dataGrid.BeginningEdit -= dataGrid_BeginningEdit;
         dataGrid.CellEditEnding -= dataGrid_CellEditEnding;
         dataGrid.RowEditEnding -= dataGrid_RowEditEnding;
         dataGrid = null;
      }

      #endregion
   }


}
