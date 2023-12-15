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

    public class locationObjectPair
    {
        private tBaseObject _baseObject;
        private Geometry _location;

        public tBaseObject BaseObject { get => _baseObject;}
        public Geometry Location { get => _location; }
    
        public locationObjectPair(tBaseObject value, Geometry location)
        {
            this._baseObject = value;
            this._location = location;
        }
    }

    [TestClass]
    public class Opdracht1Tests
    {
        [TestMethod]
        public void CompareObjectsInIMX()
        {

            (Dictionary<string, tBaseObject>? baseObjectsProject1, Dictionary<string, tBaseObject>? baseObjectsProject2) = LoadFiles();

            //filter for puics
            var intersect = baseObjectsProject1.Keys.Intersect(baseObjectsProject2.Keys).ToList(); // Correctly matched puics
            var except1 = baseObjectsProject1.Keys.Except(baseObjectsProject2.Keys).ToList();      // Puics found in Project 1 but not in 2
            var except2 = baseObjectsProject2.Keys.Except(baseObjectsProject1.Keys).ToList();      // Puics found in Project 2 but not in 1

            
            List<string> typeNamesProject1 = new List<string>();
            foreach (string puic in intersect)
            {
                tBaseObject actualObject = baseObjectsProject1[puic];
                typeNamesProject1.Add(actualObject.GetType().Name);
            }

            HashSet<string> exceptTypes1 = except1.Select(x => baseObjectsProject1[x].GetType().Name).ToHashSet();
            HashSet<string> exceptTypes2 = except2.Select(x => baseObjectsProject2[x].GetType().Name).ToHashSet();
            int intersectCount = intersect.Count;
            HashSet<string> typesInXML = baseObjectsProject1.Select(x => x.Value.GetType().Name).ToHashSet();
            HashSet<string> typesInXML2 = baseObjectsProject2.Select(x => x.Value.GetType().Name).ToHashSet();
            HashSet<string> uniqueTypes = typeNamesProject1.ToHashSet();


            Dictionary<string, int> countPerTypeDict1 = new Dictionary<string, int>();
            Dictionary<string, int> countPerTypeDict2 = new Dictionary<string, int>();
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

            //get filtered model from puics
            //Step 1
            List<tBaseObject> filteredModel1 = baseObjectsProject1.Where(x => except1.Contains(x.Key)).Select(x => x.Value).ToList();
            List<tBaseObject> filteredModel2 = baseObjectsProject2.Where(x => except2.Contains(x.Key)).Select(x => x.Value).ToList();

            //Step 2
            //filter by envelope and objects having location
            (List<locationObjectPair> model1LocationPairs, List<locationObjectPair> model2LocationPairs, 
                List<tBaseObject> model1WithoutLocation, List<tBaseObject> model2WithoutLocation) = CheckLocation(filteredModel1, filteredModel2);

            //step3
            //filter by name + type incl subtype
            List<tBaseObject> filteredModel1Attributes = CheckAttributes(model1WithoutLocation, model2WithoutLocation, out var matchedObjectsModel1to2);
            List<tBaseObject> filteredModel1AttributesLocation = CheckAttributes(model1LocationPairs.Select(x=>x.BaseObject).ToList(), 
                model2LocationPairs.Select(x => x.BaseObject).ToList(), out var matchedLocationObjectsModel1to2);

            //step 4
            List<locationObjectPair> filteredmodel1Step4WithLocation = model1LocationPairs.Where(y => filteredModel1AttributesLocation.Exists(x => x.puic == y.BaseObject.puic)).ToList();
            var notMatchedObjects =  ObjectsNotCloseToOtherObjects(filteredmodel1Step4WithLocation, model2LocationPairs, out List<locationObjectPair> matchedObjectsLocation);

        }

        public List<locationObjectPair> ObjectsNotCloseToOtherObjects(List<locationObjectPair> model1, List<locationObjectPair> model2, out List<locationObjectPair> matchedObjects)
        {
            matchedObjects = new List<locationObjectPair>();
            var notMatchedLocations = new List<locationObjectPair>();

            foreach(locationObjectPair locationObjectPair in model1)
            {

                if (locationObjectPair.Location is NetTopologySuite.Geometries.LineString lineString)
                {
                    List<locationObjectPair> model2OnlyLineStrings = model2.Where(x => x.Location is NetTopologySuite.Geometries.LineString).ToList();
                    if (model2OnlyLineStrings.Find(x => lineString.Contains(x.Location)) is locationObjectPair matchedObject)
                    {
                        matchedObjects.Add(matchedObject);
                    }
                    else
                    {
                        notMatchedLocations.Add(locationObjectPair);
                    }
                }
                else                 
                {
                    List<locationObjectPair> model2OnlyPointObjects = model2.Where(x => x.Location is NetTopologySuite.Geometries.Point).ToList();
                    if (model2OnlyPointObjects.Find(x => x.Location.IsWithinDistance(locationObjectPair.Location, 2)) is locationObjectPair matchedObject2)
                    {
                        matchedObjects.Add(matchedObject2);
                    }
                    else
                    {
                        notMatchedLocations.Add(locationObjectPair);
                    }
                }

            }
            return notMatchedLocations;
        }

        void ExploreIMXModel(object toExplore, ref Dictionary<string, tBaseObject> tBaseObjects)
        {
            PropertyInfo[] props = toExplore.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in props)
            {
                object objectValue = propertyInfo.GetValue(toExplore);
                if (objectValue is IEnumerable<tBaseObject> baseObjects)
                {
                    foreach (var item in baseObjects)
                    {
                        tBaseObjects.Add(item.puic, item);
                    }
                }
                else if (objectValue is IEnumerable<object> objects)
                {
                    //do nothing
                }
                else if (objectValue is tBaseObject baseObject)
                {

                }
                else if (objectValue != null && !(objectValue is ValueType))
                {
                    ExploreIMXModel(objectValue, ref tBaseObjects);
                }
            }
        }

        private (Dictionary<string, tBaseObject> baseObjectsProject1, Dictionary<string, tBaseObject> baseObjectsProject2)
            LoadFiles()
        {
            tSituation? project, project2;
            List<string> firstProjectPuics = new();
            List<string> secondProjectPuics = new();
            Dictionary<string, tBaseObject> baseObjectsProject1, baseObjectsProject2;
            //Arrange
            string baseDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testFile = System.IO.Path.Combine(baseDir, "TestFiles", "Ontwerp IMX.v500.xml");
            string testFileCompare = System.IO.Path.Combine(baseDir, "TestFiles", "Uitlever IMX B.v500.xml");
            //Assert: Check if the created file exists in the deployment directory
            Assert.IsTrue(File.Exists(testFile), "deployment failed: " + testFile + " did not get deployed");
            //Act
            IMX.V500.ImSpoor? imSpoorDesign = ImxSerializer.ReadXml(testFile);
            IMX.V500.ImSpoor? imSpoorCompare = ImxSerializer.ReadXml(testFileCompare);

            project = imSpoorDesign.Item as tSituation;
            project2 = imSpoorCompare.Item as tSituation;
            baseObjectsProject1 = new Dictionary<string, tBaseObject>();
            baseObjectsProject2 = new Dictionary<string, tBaseObject>();

            ExploreIMXModel(project, ref baseObjectsProject1);
            ExploreIMXModel(project2, ref baseObjectsProject2);

            return (baseObjectsProject1, baseObjectsProject2);
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

        public List<tBaseObject> CheckAttributes(List<tBaseObject> IMXModel1, List<tBaseObject> IMXModel2, out List<(tBaseObject, tBaseObject)> objectMatched)
        {
            //assumption object with puic in one model is not in second model

            objectMatched = new List<(tBaseObject, tBaseObject)>();
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

            return Model1NotMatched;
            

        }


        public List<locationObjectPair> getLocationObjectPairList(List<tBaseObject> IMXModel, out List<tBaseObject> objectsWithoutLocation)
        {
            List<locationObjectPair> locationPairedObjectsList = new();
            objectsWithoutLocation = new();

            foreach (tBaseObject item in IMXModel)
            {
                if (item is tPointObject tPointObject)
                {
                    var location = new NetTopologySuite.Geometries.Point(ParseCoordinate(tPointObject.Location.GeographicLocation.Point.coordinates));
                    locationPairedObjectsList.Add(new locationObjectPair(item, location));
                }
                else if (item is tLineObject tLineObject)
                {
                    var lineString = ParseLineString(tLineObject.Location.GeographicLocation.LineString.coordinates.ToString());
                    locationPairedObjectsList.Add(new locationObjectPair(item, lineString));

                }
                else
                {
                    objectsWithoutLocation.Add(item); 
                }
            }

            return locationPairedObjectsList;
        }

        public (List<locationObjectPair>, List<locationObjectPair>, List<tBaseObject>, List<tBaseObject>) CheckLocation(List<tBaseObject> IMXModel1, List<tBaseObject> IMXModel2)
        {

            List<Tuple<tBaseObject, tBaseObject>> matches;
            List<locationObjectPair> locationPairedObjects1 = getLocationObjectPairList(IMXModel1, out List<tBaseObject> objectsWithoutLocationModel1);
            List<locationObjectPair> locationPairedObjects2 = getLocationObjectPairList(IMXModel2, out List<tBaseObject> objectsWithoutLocationModel2);

            List<Coordinate> coordinates1 = new();
            locationPairedObjects1.ForEach(x => coordinates1.AddRange(x.Location.Coordinates));
            Envelope envelope1 = new Envelope(coordinates1);

            List<Coordinate> coordinates2 = new();
            locationPairedObjects2.ForEach(x => coordinates2.AddRange(x.Location.Coordinates));
            Envelope envelope2 = new Envelope(coordinates2);

            Envelope intersection = envelope1.Intersection(envelope2);
            
            List<locationObjectPair> filteredList1 = locationPairedObjects1.Where(x => intersection.Contains(x.Location.EnvelopeInternal)).ToList();
            List<locationObjectPair> filteredList2 = locationPairedObjects2.Where(x => intersection.Contains(x.Location.EnvelopeInternal)).ToList();

            return (filteredList1, filteredList2, objectsWithoutLocationModel1, objectsWithoutLocationModel2);
        }
    }
}
