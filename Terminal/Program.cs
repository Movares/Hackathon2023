namespace Terminal
{
    public class Program {
		public static void Main(string[] _) {
			string firstFile = @"C:\Users\924890\source\repos\Hackathon2023\Ontwerp IMX.V500.xml";
			string secondFile = @"C:\Users\924890\source\repos\Hackathon2023\Uitlever IMX B.V500.xml";

			Solutions.ShowDuplicatePuics(firstFile, secondFile);
			Solutions.ShowDuplicates(firstFile, secondFile, IMXObjects.GetNonIMX, Compare.IsNonBaseObjectWithRefEqual);
			Solutions.ShowDuplicates(firstFile, secondFile, IMXObjects.Get, Compare.IsNonBaseWithoutRefEqual);
		}
	}
}
