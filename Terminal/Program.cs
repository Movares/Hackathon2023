using Hackathon2023;
using IMX.V500;
using System.Reflection;

namespace Terminal
{
    public class Program
    {
        static void Main(string[] _)
        {
            string file1 = @"C:\Users\924890\source\repos\Hackathon2023\Ontwerp IMX.V500.xml";
            string file2 = @"C:\Users\924890\source\repos\Hackathon2023\Uitlever IMX B.V500.xml";

            object[] firstObjects = IMXObjects.GetNonIMX(ImxSerializer.ReadXml(file1)!);
            object[] secondObjects = IMXObjects.GetNonIMX(ImxSerializer.ReadXml(file2)!);

            Console.WriteLine($"Objects in first file: {firstObjects.Length}");
            Console.WriteLine($"Objects in second file: {secondObjects.Length}");

            List<object> duplicates = new();

            foreach (object obj in firstObjects)
            {
                if (secondObjects.Any(o => CompareIMXObject(obj, o)))
                {
                    duplicates.Add(obj);
                }
            }

            Console.WriteLine($"Duplicate objects: {duplicates.Count}");
            Console.ReadKey();
        }

        static void ShowDuplicatePuics(string firstFile, string secondFile)
        {
            firstFile ??= @"C:\Users\924890\source\repos\Hackathon2023\Ontwerp IMX.V500.xml";
            secondFile ??= @"C:\Users\924890\source\repos\Hackathon2023\Uitlever IMX B.V500.xml";

            string[] duplicatePuics = GetDuplicatePuics(firstFile, secondFile);

            foreach (string puic in duplicatePuics)
            {
                Console.WriteLine($"\t{puic}");
            }

            Console.WriteLine($"Duplicate puics: {duplicatePuics.Length}");
            Console.ReadKey();
        }

        static string[] GetDuplicatePuics(string firstFile, string secondFile)
        {
            ImSpoor firstModel = ImxSerializer.ReadXml(firstFile)!;
            ImSpoor secondModel = ImxSerializer.ReadXml(secondFile)!;

            IEnumerable<string> all = IMXObjects.GetPuics(firstModel).Concat(IMXObjects.GetPuics(secondModel));

            return all.GroupBy(x => x)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToArray();
        }

        static bool CompareIMXObject(object first, object second)
        {
            Dictionary<Type, List<PropertyInfo>> keys = new Dictionary<Type, List<PropertyInfo>>
            {
                [typeof(MicroNode)] = new List<PropertyInfo> { typeof(MicroNode).GetProperty("junctionRef")! },
                [typeof(MicroLink)] = new List<PropertyInfo> { typeof(MicroLink).GetProperty("implementationObjectRef")! },
                [typeof(FlankProtectionConfiguration)] = new List<PropertyInfo> {
                    typeof(FlankProtectionConfiguration).GetProperty("switchMechanismRef")!,
                    typeof(FlankProtectionConfiguration).GetProperty("position")!,
                },
                [typeof(ErtmsBaliseGroup)] = new List<PropertyInfo> { typeof(MicroNode).GetProperty("baliseGroupRef")! },
                [typeof(ErtmsLevelCrossing)] = new List<PropertyInfo> { typeof(MicroNode).GetProperty("levelCrossingRef")! },
                [typeof(ErtmsRoute)] = new List<PropertyInfo> { typeof(MicroNode).GetProperty("routeRef")! },
                [typeof(ErtmsSignal)] = new List<PropertyInfo> { typeof(MicroNode).GetProperty("signalRef")! },
                [typeof(EuroBalise)] = new List<PropertyInfo> { typeof(MicroNode).GetProperty("baliseRef")! },
            };

            Type type = first.GetType();

            if (type != second.GetType())
            {
                return false;
            }

            if (first is tBaseObject firstAsBase && second is tBaseObject secondAsBase)
            {
                return firstAsBase.puic == secondAsBase.puic;
            }

            return keys[type].All(prop => prop.GetValue(first)!.Equals(prop.GetValue(second)!));
        }
    }
}
