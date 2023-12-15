using Hackathon2023;
using IMX.V500;

namespace Terminal {
	public static class Solutions {
		public static void ShowAll() {
			string firstFile = @"C:\Users\924890\source\repos\Hackathon2023\Ontwerp IMX.V500.xml";
			string secondFile = @"C:\Users\924890\source\repos\Hackathon2023\Uitlever IMX B.V500.xml";

			ShowDuplicatePuics(firstFile, secondFile);
			ShowDuplicates(firstFile, secondFile, IMXObjects.GetNonIMX, Compare.IsNonBaseObjectWithRefEqual);
			ShowDuplicates(firstFile, secondFile, IMXObjects.Get, Compare.IsNonBaseWithoutRefEqual);
		}

		public static void ShowDuplicates(string firstFile, string secondFile, Func<ImSpoor, object[]> objectGetter, Func<object, object, bool> comparer) {
			object[] firstObjects = objectGetter(ImxSerializer.ReadXml(firstFile)!);
			object[] secondObjects = objectGetter(ImxSerializer.ReadXml(secondFile)!);

			Console.WriteLine($"Objects in first file: {firstObjects.Length}");

			foreach (IGrouping<Type, object> groups in firstObjects.GroupBy(x => x.GetType())) {
				Console.WriteLine($"\t{groups.Key.Name}: {groups.Count()}");
			}

			Console.WriteLine($"Objects in second file: {secondObjects.Length}");

			foreach (IGrouping<Type, object> groups in secondObjects.GroupBy(x => x.GetType())) {
				Console.WriteLine($"\t{groups.Key.Name}: {groups.Count()}");
			}

			object[] duplicates = firstObjects.Where(firstObject => secondObjects.Any(secondObject => comparer(firstObject, secondObject))).ToArray();

			Console.WriteLine($"Duplicate objects: {duplicates.Length}");

			foreach (IGrouping<Type, object> groups in duplicates.GroupBy(x => x.GetType())) {
				Console.WriteLine($"\t{groups.Key.Name}: {groups.Count()}");
			}

			Console.WriteLine();
			Console.ReadKey();
		}

		public static void ShowDuplicatePuics(string firstFile, string secondFile) {
			string[] duplicatePuics = GetDuplicatePuics(firstFile, secondFile);

			foreach (string puic in duplicatePuics) {
				Console.WriteLine($"\t{puic}");
			}

			Console.WriteLine($"Duplicate puics: {duplicatePuics.Length}");
			Console.ReadKey();
		}

		public static string[] GetDuplicatePuics(string firstFile, string secondFile) {
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
