using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using MagicSoftware.Common.Controls.Table.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.TableControl.UI;

namespace Tests.TableControl
{
   /// <summary>
   ///This is a test class for DataGridAsEditingAdapterTest and is intended
   ///to contain all DataGridAsEditingAdapterTest Unit Tests
   ///</summary>
   [TestClass()]
   public class DataGridEditingServiceTest
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

      #endregion Additional test attributes

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

         var target = new DataGridEditStateService();
         var serviceList = new UIServiceCollection();
         serviceList.Add(new ServiceFactoryMock(target));
         serviceList.Add(new ParameterlessUIServiceFactory() { ServiceType = typeof(DataGridCurrentCellService) });
         serviceList.Add(new ParameterlessUIServiceFactory() { ServiceType = typeof(DefaultCommandRegulationService) });

         DataGrid dataGrid;
         using (TestWindow.Show(dataList, serviceList, out dataGrid))
         {
            using (var eventsSink = new DGEventsSink(dataGrid))
            {
               Assert.IsFalse(target.BeginItemEdit(), "Begin edit should have failed, because the data grid has no selected cell.");
               Assert.IsFalse(eventsSink.BeginningEditEventRaised);

               // Select a cell eligible for editing.
               dataGrid.CurrentCell = new System.Windows.Controls.DataGridCellInfo(dataList[0], dataGrid.Columns[0]);
               Assert.IsTrue(target.BeginItemEdit(), "Begin edit should have succeeded.");
               Assert.IsTrue(eventsSink.BeginningEditEventRaised);
               Assert.IsTrue(target.IsEditingField);

               eventsSink.Reset();
               Assert.IsTrue(target.BeginItemEdit(), "Although no new editing began, begin edit should return true - meaning the grid is in edit mode.");
               Assert.IsFalse(eventsSink.BeginningEditEventRaised);
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

         var target = new DataGridEditStateService();
         var serviceList = new UIServiceCollection();
         serviceList.Add(new ServiceFactoryMock(target));
         serviceList.Add(new ParameterlessUIServiceFactory() { ServiceType = typeof(DataGridCurrentCellService) });
         serviceList.Add(new ParameterlessUIServiceFactory() { ServiceType = typeof(DefaultCommandRegulationService) });

         DataGrid dataGrid;
         using (TestWindow.Show(dataList, serviceList, out dataGrid))
         {
            // Begin edit and ensure the grid is indeed in edit mode.
            dataGrid.CurrentCell = new System.Windows.Controls.DataGridCellInfo(dataList[0], dataGrid.Columns[0]);
            Assert.IsTrue(target.BeginItemEdit(), "Begin edit should have succeeded.");
            Assert.IsTrue(target.IsEditingField);

            using (var eventsSink = new DGEventsSink(dataGrid))
            {
               Assert.IsTrue(target.CancelItemEdit());
               Assert.IsTrue(eventsSink.RowEditEndingEventRaised);
               Assert.IsTrue(eventsSink.CellEditEndingEventRaised);

               eventsSink.Reset();
               Assert.IsTrue(target.CancelItemEdit());
               Assert.IsFalse(eventsSink.RowEditEndingEventRaised);
               Assert.IsFalse(eventsSink.CellEditEndingEventRaised);
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

         var target = new DataGridEditStateService();
         var serviceList = new UIServiceCollection();
         serviceList.Add(new ServiceFactoryMock(target));
         serviceList.Add(new ParameterlessUIServiceFactory() { ServiceType = typeof(DataGridCurrentCellService) });
         serviceList.Add(new ParameterlessUIServiceFactory() { ServiceType = typeof(DefaultCommandRegulationService) });

         DataGrid dataGrid;
         using (TestWindow.Show(dataList, serviceList, out dataGrid))
         {
            // Begin edit and ensure the grid is indeed in edit mode.
            dataGrid.CurrentCell = new System.Windows.Controls.DataGridCellInfo(dataList[0], dataGrid.Columns[0]);
            Assert.IsTrue(target.BeginItemEdit(), "Begin edit should have succeeded.");
            Assert.IsTrue(target.IsEditingField);

            using (var eventsSink = new DGEventsSink(dataGrid))
            {
               Assert.IsTrue(target.CommitItemEdit());
               Assert.IsTrue(eventsSink.RowEditEndingEventRaised);
               Assert.IsTrue(eventsSink.CellEditEndingEventRaised);

               eventsSink.Reset();
               Assert.IsTrue(target.CommitItemEdit());
               Assert.IsFalse(eventsSink.RowEditEndingEventRaised);
               Assert.IsFalse(eventsSink.CellEditEndingEventRaised);
            }
         }
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

         var target = new DataGridEditStateService();
         var serviceList = new UIServiceCollection();
         serviceList.Add(new ServiceFactoryMock(target));
         serviceList.Add(new ParameterlessUIServiceFactory() { ServiceType = typeof(DataGridCurrentCellService) });
         serviceList.Add(new ParameterlessUIServiceFactory() { ServiceType = typeof(DefaultCommandRegulationService) });

         DataGrid dataGrid;
         using (TestWindow.Show(dataList, serviceList, out dataGrid))
         {
            // Begin edit and ensure the grid is indeed in edit mode.
            dataGrid.CurrentCell = new DataGridCellInfo(dataList[0], dataGrid.Columns[0]);
            Precondition(target.BeginItemEdit(), "Begin edit should have succeeded.");

            Assert.AreEqual(dataList[0], target.CurrentEdit);
            Precondition(target.CommitItemEdit(), "Failed committing");
            Assert.IsNull(target.CurrentEdit);

            dataGrid.CurrentCell = new DataGridCellInfo(dataList[2], dataGrid.Columns[0]);
            Precondition(dataGrid.BeginEdit(), "Begin edit should have succeeded.");
            Assert.AreEqual(dataList[2], target.CurrentEdit);
            Precondition(target.CancelItemEdit(), "Failed canceling");
            Assert.IsNull(target.CurrentEdit);
         }
      }

      /// <summary>
      /// Test whether the editing state service correctly detects the current data grid edit state.
      ///</summary>
      [TestMethod()]
      public void DataGridEditingStateDetectionTest()
      {
         ObservableCollection<TestData> dataList = new ObservableCollection<TestData>()
         {
            new TestData() { StrValue = "A" },
            new TestData() { StrValue = "B" },
            new TestData() { StrValue = "C" }
         };

         var target = new DataGridEditStateService();
         var serviceList = new UIServiceCollection();
         serviceList.Add(new ServiceFactoryMock(target));
         serviceList.Add(new ParameterlessUIServiceFactory() { ServiceType = typeof(DataGridCurrentCellService) });
         serviceList.Add(new ParameterlessUIServiceFactory() { ServiceType = typeof(DefaultCommandRegulationService) });

         DataGrid dataGrid;
         using (TestWindow.Show(dataList, serviceList, out dataGrid))
         {
            dataGrid.CurrentCell = new System.Windows.Controls.DataGridCellInfo(dataList[0], dataGrid.Columns[0]);

            // Verify IsEditing property is correct.
            Assert.IsFalse(target.IsEditingField);

            // Begin editing on the data grid and verify that a _newly_ created adapter will show
            // that the grid is editing.
            if (!dataGrid.BeginEdit())
               Assert.Inconclusive("Could not begin edit on data grid.");

            Assert.IsTrue(target.IsEditingField);

            // Now commit the row to exit editing.
            if (!dataGrid.CommitEdit(DataGridEditingUnit.Row, true))
               Assert.Inconclusive("Could not commit edit on data grid.");

            Assert.IsFalse(target.IsEditingField);
         }
      }

      private void Precondition(bool condition, string message)
      {
         if (!condition)
            Assert.Inconclusive(message);
      }
   }

   /// <summary>
   /// Class for handling data grid events and signaling whether they were raised or not.
   /// Designed as IDisposable to ensure detaching from the data grid when object is no longer
   /// necessary.
   /// </summary>
   internal class DGEventsSink : IDisposable
   {
      private DataGrid dataGrid;

      public DGEventsSink(DataGrid dataGrid)
      {
         this.dataGrid = dataGrid;
         dataGrid.BeginningEdit += dataGrid_BeginningEdit;
         dataGrid.CellEditEnding += dataGrid_CellEditEnding;
         dataGrid.RowEditEnding += dataGrid_RowEditEnding;
         Reset();
      }

      public bool BeginningEditEventRaised { get; private set; }

      public bool CellEditEndingEventRaised { get; private set; }

      public bool RowEditEndingEventRaised { get; private set; }

      public void Reset()
      {
         BeginningEditEventRaised = false;
         RowEditEndingEventRaised = false;
         CellEditEndingEventRaised = false;
      }

      private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
      {
         BeginningEditEventRaised = true;
      }

      private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
      {
         CellEditEndingEventRaised = true;
      }

      private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
      {
         RowEditEndingEventRaised = true;
      }

      #region IDisposable Members

      public void Dispose()
      {
         dataGrid.BeginningEdit -= dataGrid_BeginningEdit;
         dataGrid.CellEditEnding -= dataGrid_CellEditEnding;
         dataGrid.RowEditEnding -= dataGrid_RowEditEnding;
         dataGrid = null;
      }

      #endregion IDisposable Members
   }

   internal class TestData : IEditableObject
   {
      public int IntValue { get; set; }

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

      #endregion IEditableObject Members

      public override string ToString()
      {
         return String.Format("{{TestData: {0}, {1}}}", StrValue, IntValue);
      }
   }
}