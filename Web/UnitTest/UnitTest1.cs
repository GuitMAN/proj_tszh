using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Web.Controllers;
using Web.Models;
using Web.Models.Repository;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Mock<Repo> mock = new Mock<Repo>();
            mock.Setup(m => m.Counter).Returns(new List<Counter>
            {
                new Counter { id = 1, place = "Место1",  serial = "45645654" },
            });
            UserController controller = new UserController(mock.Object);

            //controller.Gas();


        }
    }
}
