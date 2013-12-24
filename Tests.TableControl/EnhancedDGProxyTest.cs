using MagicSoftware.Common.Controls.Table.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Controls;

namespace Tests.TableControl
{


   /// <summary>
   ///This is a test class for EnhancedDGProxyTest and is intended
   ///to contain all EnhancedDGProxyTest Unit Tests
   ///</summary>
   [TestClass()]
   public class EnhancedDGProxyTest
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
      ///A test for GetAdapter
      ///</summary>
      [TestMethod()]
      [DeploymentItem("MagicSoftware.Common.Controls.Table.dll")]
      public void GetAdapterTest()
      {
         DataGrid dg = new DataGrid();
         PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor> helper = new PrivateAccessHelper<EnhancedDGProxy, EnhancedDGProxy_Accessor>(new EnhancedDGProxy());
         helper.PrivateObject.Invoke("AttachTo", dg);
         var target = helper.Accessor;
         
         //Type adapterType = null; 
         //object expected = null; 
         //object actual;
         //actual = target.GetAdapter(adapterType);
         //Assert.AreEqual(expected, actual);
         Assert.Inconclusive("Verify the correctness of this test method.");
      }

   }


}
