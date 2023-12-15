using IMX.V500;

namespace Terminal {
	internal class Program {
		static void Main(string[] _) {
			string file1 = @"C:\Users\924890\source\repos\Hackathon2023\Ontwerp IMX.V500.xml";
			string file2 = @"C:\Users\924890\source\repos\Hackathon2023\Uitlever IMX B.V500.xml";

			string[] duplicatePuics = GetDuplicatePuics(file1, file2);
			s
			Console.WriteLine($"Duplicate puics: {duplicatePuics.Length}");

			foreach (string puic in duplicatePuics) {
				Console.WriteLine($"\t{puic}");
			}

			Console.ReadKey();
		}

		static string[] GetDuplicatePuics(string firstFile, string secondFile) {
			ImSpoor firstModel = Hackathon2023.ImxSerializer.ReadXml(firstFile)!;
			ImSpoor secondModel = Hackathon2023.ImxSerializer.ReadXml(secondFile)!;

			IEnumerable<string> all = IMXObjects.GetPuics(firstModel).Concat(IMXObjects.GetPuics(secondModel));

			return all.GroupBy(x => x)
				.Where(x => x.Count() > 1)
				.Select(x => x.Key)
				.ToArray();
		}
	}
}