using System.Xml.Serialization;
using IMX.V500;

namespace Hackathon2023
{
	public static class ImxSerializer
	{
		public static ImSpoor? ReadXml(string xmlFilePath)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ImSpoor));

			using (FileStream fileStream = new FileStream(xmlFilePath, FileMode.Open))
			{
				return serializer.Deserialize(fileStream) as ImSpoor;
			}
		}
	}
}
