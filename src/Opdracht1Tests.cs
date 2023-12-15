using IMX.V500;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;


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

            var intersect = firstProjectPuics.Intersect(secondProjectPuics).ToList(); // Correctly matched puics
            var except1 = firstProjectPuics.Except(secondProjectPuics).ToList();      // Puics found in Project 1 but not in 2
            var except2 = secondProjectPuics.Except(firstProjectPuics).ToList();      // Puics found in Project 2 but not in 1

            List<string> typeNamesProject1 = new List<string>();
            foreach(var p in intersect)
            {
                tBaseObject actualObject = baseObjectsProject1[p];
                tBaseObject actualObjectProject2 = baseObjectsProject2[p];
                typeNamesProject1.Add(actualObject.GetType().Name);

            }

            //var myDictionary = except1.GroupBy(o => baseObjectsProject1[o].GetType().Name);

            var exceptTypes1 = except1.Select(x => baseObjectsProject1[x].GetType().Name).ToHashSet();
            var exceptTypes2 = except2.Select(x => baseObjectsProject2[x].GetType().Name).ToHashSet();
            int intersectCount = intersect.Count;
            var typesInXML = baseObjectsProject1.Select(x=>x.Value.GetType().Name).ToHashSet();
            var typesInXML2 = baseObjectsProject2.Select(x=>x.Value.GetType().Name).ToHashSet();
            HashSet<string> uniqueTypes = typeNamesProject1.ToHashSet();

            var countPerTypeDict1 = new Dictionary<string, int>();
            var countPerTypeDict2 = new Dictionary<string, int>();
            foreach (string type in exceptTypes1.Union(exceptTypes2)) 
            {
                countPerTypeDict1[type] = baseObjectsProject1.Where(x => x.Value.GetType().Name == type && except1.Contains(x.Key)).Count();
                countPerTypeDict2[type] = baseObjectsProject2.Where(x => x.Value.GetType().Name == type && except2.Contains(x.Key)).Count();
            }

            var matchingTypesDict = new Dictionary<string, int>();
            foreach (string type in uniqueTypes)
            {
                matchingTypesDict[type] = baseObjectsProject1.Where(x => x.Value.GetType().Name == type && intersect.Contains(x.Key)).Count();
            }

            List<tBaseObject> filteredModel1 = baseObjectsProject1.Where(x => except1.Contains(x.Key)).Select(x => x.Value).ToList();
            List<tBaseObject> filteredModel2 = baseObjectsProject2.Where(x => except2.Contains(x.Key)).Select(x => x.Value).ToList();

            CheckAttributes(filteredModel1, filteredModel2);
            //Assert
            Assert.IsNotNull(intersect);

            CheckLocation(baseObjectsProject1, baseObjectsProject2);
        }

        private CultureInfo DefaultFormat = CultureInfo.InvariantCulture;
        /// <summary>
        /// Converts a String to Coordinate.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        public Coordinate ParseCoordinate(string value)
        {
            string[] coordinates = value.Split(',');

            if (coordinates.Count() > 0)
            {
                double x = 0.0;
                double y = 0.0;
                double z = 0.0;

                _ = double.TryParse(coordinates[0], NumberStyles.Number, DefaultFormat, out x);
                _ = double.TryParse(coordinates[1], NumberStyles.Number, DefaultFormat, out y);

                Coordinate coordinate = new Coordinate(x, y);

                if (coordinates.Length == 3)
                {
                    _ = double.TryParse(coordinates[2], NumberStyles.Number, DefaultFormat, out z);
                    Coordinate coordinateZ = new CoordinateZ(x, y, z);
                    return coordinateZ;
                }
                else
                {
                    return coordinate;
                }
            }
            return new CoordinateZ(0, 0, 0);
        }
    

        /// <summary>
        /// Converts a String to LineString.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        public NetTopologySuite.Geometries.LineString ParseLineString(string value)
        {
            string[] points = value.Split(' ');
            if (points.Count() > 0)
            {
                var coordinates = points.Select(p => ParseCoordinate(p)).ToArray();

                return new NetTopologySuite.Geometries.LineString(coordinates);
            }
            return null;
        }

        public void CheckAttributes(List<tBaseObject> IMXModel1, List<tBaseObject> IMXModel2)
        {
            //assumption object with puic in one model is not in second model

            List<(tBaseObject, tBaseObject)> objectMatched = new List<(tBaseObject, tBaseObject)>();
            List<tBaseObject> Model1NotMatched = new List<tBaseObject>();
            foreach (tBaseObject baseObject in IMXModel1)
            {
                Type objectType = baseObject.GetType();

                PropertyInfo typeProperty = objectType.GetProperty($"{Char.ToLowerInvariant(objectType.Name[0]) + objectType.Name.Substring(1)}Type");
                List<tBaseObject> filteredObjectsByType = IMXModel2.Where(x=>x.GetType().Name == baseObject.GetType().Name).ToList();
                List<tBaseObject> filteredObjectsByName = filteredObjectsByType.Where(x=>x.name == baseObject.name).ToList();

                if (filteredObjectsByName.Count > 0)
                {
                    if (typeProperty != null)
                    {
                        List<tBaseObject> filteredByObjectSubType = filteredObjectsByName.Where(x => typeProperty.GetValue(x).ToString() == typeProperty.GetValue(baseObject).ToString()).ToList();

                        if (filteredByObjectSubType.Count > 0)
                        {
                            objectMatched.Add((baseObject, filteredByObjectSubType.First()));
                            continue;
                        }
                    }
                    else  
                    {
                        objectMatched.Add((baseObject, filteredObjectsByName.First()));
                        continue;
                    }
                }             
                
                Model1NotMatched.Add(baseObject);

            }

            

        }

        public void CheckLocation(Dictionary<string, tBaseObject> IMXModel1, Dictionary<string, tBaseObject> IMXModel2)
        {
            List<Tuple<tBaseObject, tBaseObject>> matches;
            foreach (KeyValuePair<string, tBaseObject> item in IMXModel1)
            {
                if (item.Value is tPointObject tPointObject)
                {
                    var location = tPointObject.Location.GeographicLocation;
                }
                else if (item.Value is tLineObject tLineObject)
                {
                    var location = tLineObject.Location.GeographicLocation.LineString;
                }
            }

        }
    }
}
