using MagicSoftware.Common.Controls.Table.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using Tests.TableControl.UI;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using MagicSoftware.Common.Controls;
using Tests.Common;
using System.Collections.Generic;

namespace Tests.TableControl
{
    
    
    /// <summary>
    ///This is a test class for UIServiceProviderTest and is intended
    ///to contain all UIServiceProviderTest Unit Tests
    ///</summary>
   [TestClass()]
   public class UIServiceProviderTest
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
      /// Add services. Load element. Verify attachment. Verify disposal.
      ///</summary>
      [TestMethod()]
      [DeploymentItem("MagicSoftware.Common.Controls.Table.dll")]
      public void SetServiceProviderTest()
      {
         // Open a window with a control without any service.
         Window window = new Window();
         Button button = new Button();
         window.Content = button;
         using (TestUtils.AutoCloseWindow(window))
         {
            var serviceList = UIServiceProvider_Accessor.GetServiceList(button);
            Assert.IsNotNull(serviceList);
            List<IUIService> serviceListWrapper = new List<IUIService>(serviceList);
            Assert.AreEqual(0, serviceListWrapper.Count);
            Assert.IsNull(UIServiceProvider_Accessor.GetServiceProvider(button));
            var service = UIServiceProvider.GetService(button, typeof(TestServiceBase));
            Assert.IsNull(service);
         }

         // Open a window with a control that has services attached.
         window = new Window();
         button = new Button();
         window.Content = button;

         UIServiceCollection serviceCollection = new UIServiceCollection();
         var serviceA = new TestServiceA();
         serviceCollection.Add(serviceA);
         UIServiceProvider.SetServiceList(button, serviceCollection);

         using (TestUtils.AutoCloseWindow(window))
         {
            Assert.IsNotNull(UIServiceProvider_Accessor.GetServiceList(button));
            Assert.IsNotNull(UIServiceProvider_Accessor.GetServiceProvider(button));
            var service = UIServiceProvider.GetService(button, typeof(TestServiceBase));
            Assert.IsInstanceOfType(service, typeof(TestServiceA));
            Assert.IsFalse(serviceA.IsDisposed);
         }

         Assert.IsTrue(serviceA.IsDisposed);
      }

      
      /// <summary>
      /// 2. Add services (1). Replace services (2). Verify disposal (1). Reset (1). Load element. Verify attachment (2). Verify no attachment (1). Verify disposal (2).
      /// </summary>
      [TestMethod()]
      [DeploymentItem("MagicSoftware.Common.Controls.Table.dll")]
      public void ReplaceServiceListTest()
      {
         // Open a window with a control that has services attached.
         var window = new Window();
         var button = new Button();
         window.Content = button;

         UIServiceCollection serviceCollection = new UIServiceCollection();
         var serviceA = new TestServiceA();
         serviceCollection.Add(serviceA);
         UIServiceProvider.SetServiceList(button, serviceCollection);

         // Replace the assigned, yet unattached services.
         serviceCollection = new UIServiceCollection();
         var serviceA2 = new TestServiceA();
         serviceCollection.Add(serviceA2);
         UIServiceProvider.SetServiceList(button, serviceCollection);

         // Verify the old service was disposed.
         Assert.IsTrue(serviceA.IsDisposed);

         // Verify the old service does not become attached to the element.
         using (TestUtils.AutoCloseWindow(window))
         {
            Assert.IsFalse(serviceA.IsAttached);
            Assert.IsTrue(serviceA2.IsAttached);
         }

         Assert.IsTrue(serviceA2.IsDisposed);
      }

      //TODO: Test cases:
      // 3. Add services. Load element. Verify attachment. Unload element. Should dispose? Reload element. Should reattach?
      // 4. Recurrent service?
      // 5. Shared service.

      ObservableCollection<TestData> CreateTestDataList(int size)
      {
         ObservableCollection<TestData> list = new ObservableCollection<TestData>();
         for (int i = 0; i < size; i++)
         {
            list.Add(new TestData() { StrValue = "Item #" + i, IntValue = i });
         }
         return list;
      }


      [ImplementedService(typeof(TestServiceBase))]
      class TestServiceBase : IUIService
      {
         static int nextId = 1;
         int id;

         public bool IsAttached { get; private set; }
         public bool IsDetached { get; private set; }
         public bool IsDisposed { get; private set; }

         public TestServiceBase()
         {
            id = nextId++;
            Reset();
         }

         void Reset()
         {
            IsAttached = false;
            IsDetached = false;
            IsDisposed = false;
         }

         #region IUIService Members

         public void AttachToElement(FrameworkElement element)
         {
            IsAttached = true;
         }

         public void DetachFromElement(FrameworkElement element)
         {
            IsDetached = true;
         }

         #endregion

         #region IDisposable Members

         public void Dispose()
         {
            Assert.IsFalse(IsDisposed, "Disposal should happen only once.");
            IsAttached = false;
            IsDisposed = true;
         }

         #endregion
      }

      class TestServiceA : TestServiceBase
      {

      }

      class TestServiceB : TestServiceBase
      {

      }
   }
}
