using FileHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			var pathRead = @"C:\Users\Milly\Downloads\umsatzliste-2014-01-04-15-00-08.xls";
			var pathWrite = Path.Combine(Path.GetDirectoryName(pathRead), Path.GetFileNameWithoutExtension(pathRead)) + ".updated.csv";
			if (File.Exists(pathWrite))
				File.Delete(pathWrite);

			var records = readRecordsFromFile(pathRead);
			var config = readConfig();
			var recordWrites = generateRecordWrites(records, config);
			writeRecords(recordWrites, pathWrite);


		}

		private static void writeRecords(IEnumerable<NetBankRecordWrite> recordWrites, string pathWrite)
		{
			var engine = new FileHelperEngine<NetBankRecordWrite>();
			engine.WriteFile(pathWrite, recordWrites);
		}

		private static IEnumerable<NetBankRecordWrite> generateRecordWrites(IEnumerable<NetBankRecordRead> records, Configuration config)
		{
			return records
				.Select(netBankRecord => new {netBankRecord, kategory = GetCategoryAndEmpfänger(config, netBankRecord)})
				.Select(@t => new NetBankRecordWrite(@t.netBankRecord, @t.kategory.Item1, @t.kategory.Item2));
		}

		private static Tuple<string,string> GetCategoryAndEmpfänger(Configuration config, NetBankRecordRead netBankRecordRead)
		{
			foreach (var category in config.Categories
				.Where(category => category.Keywords
					.Any(keyword => netBankRecordRead.Verwendungszweck.ToLower().Contains(keyword.Name.ToLower()))))
			{
				return new Tuple<string, string>(category.Name,
					category.Keywords.Where(keyword => netBankRecordRead.Verwendungszweck.ToLower().Contains(keyword.Name.ToLower()))
						.Select(keyword => keyword.Empfänger)
						.Single());
			}

			return new Tuple<string, string>("", "");
		}

		private static Configuration readConfig()
		{
			var serializer = new XmlSerializer(typeof(Configuration));
			var xdoc = new XmlDocument();
			xdoc.Load("Configuration.xml");
			var configuration = (Configuration)serializer.Deserialize(File.Open("Configuration.xml", FileMode.Open));

			//var test = new Configuration { Categories = new List<Category> { new Category { Name = "c1", Keywords = new List<string> { "a", "b" } } } };
			//var xmldoc = new XmlDocument();
			//using (var stream = new MemoryStream())
			//{
			//	serializer.Serialize(stream, test);
			//	stream.Position = 0;
			//	xmldoc.Load(stream);
			//}

			return configuration;
		}

		private static IEnumerable<NetBankRecordRead> readRecordsFromFile(string fileName)
		{
			var engine = new FileHelperEngine<NetBankRecordRead>();
			engine.Options.IgnoreFirstLines = 10;
			engine.Options.IgnoreLastLines = 1;
			var records = engine.ReadFile(fileName);

			return records;
		}
	}
}
