using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hackathon2023
{
    [TestClass]
    public class Opdracht1Tests
    {
        [TestMethod]
        public void CheckPuics()
        {
            //Arrange
            string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testFile = Path.Combine(baseDir, "TestFiles", "Ontwerp IMX.v500.xml");
            string testFileCompare = Path.Combine(baseDir, "TestFiles", "Uitlever IMX B.v500.xml");
            //Assert: Check if the created file exists in the deployment directory
            Assert.IsTrue(File.Exists(testFile), "deployment failed: " + testFile + " did not get deployed");
            //Act
            var imSpoor500 = ImxSerializer.ReadXml(testFile);
            //Assert
            Assert.IsNotNull(imSpoor500);
        }
    }
}
