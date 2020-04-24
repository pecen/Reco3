using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reco3_Simulation;
using Reco3_Simulation.Controllers;

namespace Reco3_Simulation.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Home Page", result.ViewBag.Title);
        }
    }
}
