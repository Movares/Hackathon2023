using Hackathon2023;
using IMX.V500;
using System.Reflection;
using Terminal;
using Path = System.IO.Path;

namespace MatchMakers.Tests
{
    [TestClass]
    public class Opdracht1Tests
    {
        [TestMethod]
        public void Opdracht1_zoekMatchingPuics()
        {
            //Arrange
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var testFile_Ontwerp = Path.Combine(baseDir, "TestFiles", "Ontwerp IMX.V500.xml");
            var testFile_Uitlever = Path.Combine(baseDir, "TestFiles", "Uitlever IMX B.V500.xml");

            //Assert: Check if the created file exists in the deployment directory
            Assert.IsTrue(File.Exists(testFile_Ontwerp), "deployment failed: " + testFile_Ontwerp + " did not get deployed");
            //Act
            ImSpoor firstModel = ImxSerializer.ReadXml(testFile_Ontwerp);
            ImSpoor secondModel = ImxSerializer.ReadXml(testFile_Uitlever);


            IMXObjects.GetPuics(firstModel);

            IEnumerable<string> allMatching = IMXObjects.GetPuics(firstModel).Concat(IMXObjects.GetPuics(secondModel));
            var matchingPuics = allMatching.GroupBy(x => x)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            Assert.AreEqual(1251, matchingPuics.Count);

            //var imSpoor500 = SerializeHelper.ReadImx500(testFile_Ontwerp);
            ////Assert
            //Assert.IsNotNull(imSpoor500);


            ////Writing IMX500 to temp directory:
            ////Arrange
            //var testWriteFile = Path.Combine("C:/Temp/", "Case1Testresult.xml");
            ////Act
            //SerializeHelper.WriteImx500(imSpoor500, testWriteFile);
            ////Assert
            //Assert.IsTrue(File.Exists(testWriteFile));
        }
    }
}