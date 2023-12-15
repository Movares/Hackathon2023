using IMX.V500;
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
            string baseDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testFile = System.IO.Path.Combine(baseDir, "TestFiles", "Ontwerp IMX.v500.xml");
            string testFileCompare = System.IO.Path.Combine(baseDir, "TestFiles", "Uitlever IMX B.v500.xml");
            //Assert: Check if the created file exists in the deployment directory
            Assert.IsTrue(File.Exists(testFile), "deployment failed: " + testFile + " did not get deployed");
            //Act
            IMX.V500.ImSpoor? imSpoorDesign = ImxSerializer.ReadXml(testFile);
            IMX.V500.ImSpoor? imSpoorCompare = ImxSerializer.ReadXml(testFileCompare);

            tSituation? project = imSpoorDesign.Item as tSituation;
            tSituation project2 = imSpoorCompare.Item as tSituation;
            Dictionary<string, tBaseObject> baseObjectsProject1 = new Dictionary<string, tBaseObject>();
            Dictionary<string, tBaseObject> baseObjectsProject2 = new Dictionary<string, tBaseObject>();



            void ExploreIMXModel(object toExplore, ref List<string> expected, ref Dictionary<string, tBaseObject> tBaseObjects)
            {
                PropertyInfo[] props = toExplore.GetType().GetProperties();

                foreach(PropertyInfo propertyInfo in props)
                {
                    object objectValue = propertyInfo.GetValue(toExplore);
                    if (objectValue is IEnumerable<tBaseObject> baseObjects)
                    {
                        string[] puics = baseObjects.Select(x => x.puic).ToArray();
                        expected.AddRange(puics);
                        foreach (var item in baseObjects)
                        {
                            tBaseObjects.Add(item.puic, item);
                        }
                    }
                    else if(objectValue is IEnumerable<object> objects)
                    {
                        //do nothing
                    }
                    else if (objectValue is tBaseObject baseObject)
                    {

                    }
                    else if(objectValue != null && !(objectValue is ValueType))
                    {
                        ExploreIMXModel(objectValue, ref expected, ref tBaseObjects);
                    }
                }
            }

            List<string> firstProjectPuics = new();
            List<string> secondProjectPuics = new();

            ExploreIMXModel(project, ref firstProjectPuics, ref baseObjectsProject1);
            ExploreIMXModel(project2, ref secondProjectPuics, ref baseObjectsProject2);

            var intersect = firstProjectPuics.Intersect(secondProjectPuics).ToList();
            var except1 = firstProjectPuics.Except(secondProjectPuics).ToList();
            var except2 = secondProjectPuics.Except(firstProjectPuics).ToList();

            List<string> typeNamesProject1 = new List<string>();
            foreach(var p in intersect)
            {
                tBaseObject actualObject = baseObjectsProject1[p];
                tBaseObject actualObjectProject2 = baseObjectsProject2[p];
                typeNamesProject1.Add(actualObject.GetType().Name);

            }

            var myDictionary = except1
    .GroupBy(o => baseObjectsProject1[o].GetType().Name);

            var exceptTypes1 = except1.Select(x => x.GetType().Name).ToHashSet();
            var exceptTypes2 = except2.Select(x => baseObjectsProject2[x].GetType().Name).ToHashSet();
            int intersectCount = intersect.Count;
            var typesInXML = baseObjectsProject1.Select(x=>x.Value.GetType().Name).ToHashSet();
            var typesInXML2 = baseObjectsProject2.Select(x=>x.Value.GetType().Name).ToHashSet();
            HashSet<string> uniqueTypes = typeNamesProject1.ToHashSet();

            //Assert
            Assert.IsNotNull(intersect);
        }
    }
}
