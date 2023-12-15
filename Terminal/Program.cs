using IMX.V500;
using Hackathon2023;
using System.Reflection;

namespace Terminal {
	internal class Program {
		static void Main(string[] _) {
			string file1 = @"C:\Users\924890\source\repos\Hackathon2023\Ontwerp IMX.V500.xml";
			string file2 = @"C:\Users\924890\source\repos\Hackathon2023\Uitlever IMX B.V500.xml";

			ShowDuplicateRefObjects(file1, file2);
		}

		static void ShowDuplicateRefObjects(string firstFile, string secondFile) {
			object[] firstObjects = IMXObjects.GetNonIMX(ImxSerializer.ReadXml(firstFile)!);
			object[] secondObjects = IMXObjects.GetNonIMX(ImxSerializer.ReadXml(secondFile)!);

			Console.WriteLine($"Objects in first file: {firstObjects.Length}");

			foreach (IGrouping<Type, object> groups in firstObjects.GroupBy(x => x.GetType())) {
				Console.WriteLine($"\t{groups.Key.Name}: {groups.Count()}");
			}

			Console.WriteLine($"Objects in second file: {secondObjects.Length}");

			foreach (IGrouping<Type, object> groups in secondObjects.GroupBy(x => x.GetType())) {
				Console.WriteLine($"\t{groups.Key.Name}: {groups.Count()}");
			}

			object[] duplicates = firstObjects.Where(firstObject => secondObjects.Any(secondObject => Compare.IsEqual(firstObject, secondObject))).ToArray();

			Console.WriteLine($"Duplicate objects: {duplicates.Length}");

			foreach (IGrouping<Type, object> groups in duplicates.GroupBy(x => x.GetType())) {
				Console.WriteLine($"\t{groups.Key.Name}: {groups.Count()}");
			}

			Console.ReadKey();
		}

		static void ShowDuplicatePuics(string firstFile, string secondFile) {
			string[] duplicatePuics = GetDuplicatePuics(firstFile, secondFile);

			foreach (string puic in duplicatePuics) {
				Console.WriteLine($"\t{puic}");
			}

			Console.WriteLine($"Duplicate puics: {duplicatePuics.Length}");
			Console.ReadKey();
		}

		static string[] GetDuplicatePuics(string firstFile, string secondFile) {
			ImSpoor firstModel = ImxSerializer.ReadXml(firstFile)!;
			ImSpoor secondModel = ImxSerializer.ReadXml(secondFile)!;

			IEnumerable<string> all = IMXObjects.GetPuics(firstModel).Concat(IMXObjects.GetPuics(secondModel));

			return all.GroupBy(x => x)
				.Where(x => x.Count() > 1)
				.Select(x => x.Key)
				.ToArray();
		}
	}
}
