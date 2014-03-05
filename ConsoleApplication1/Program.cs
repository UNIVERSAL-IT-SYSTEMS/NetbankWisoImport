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
		private static string _configurationPath = "Configuration.xml";

		static void Main(string[] args)
		{
			var pathRead = args[0];
			var pathComplete = Path.Combine(Path.GetDirectoryName(pathRead), Path.GetFileNameWithoutExtension(pathRead)) + ".complete.csv";
			var pathIncomplete = Path.Combine(Path.GetDirectoryName(pathRead), Path.GetFileNameWithoutExtension(pathRead)) + ".incomplete.csv";
			if (File.Exists(pathComplete))
				File.Delete(pathComplete);
			if (File.Exists(pathIncomplete))
				File.Delete(pathIncomplete);
			
			var records = readRecordsFromFile(pathRead);
			var config = readConfig();
			var completeRecords = new List<NetBankRecordWrite>();
			var minAndMaxRecordTime = getMinAndMaxRecordTime(records, config);

			writeMaxRecordTimeToConfig(config, minAndMaxRecordTime.Item2);

			records = filterRecords(records, minAndMaxRecordTime);
			var incompleteRecords = new List<NetBankRecordWrite>();
			generateRecordWrites(records, config, completeRecords.Add, incompleteRecords.Add);
			writeRecords(completeRecords, pathComplete);
			writeRecords(incompleteRecords, pathIncomplete);
		}

		private static IEnumerable<NetBankRecordRead> filterRecords(IEnumerable<NetBankRecordRead> records, Tuple<DateTime, DateTime> minAndMaxRecordTime)
		{
			return records.Where(read => read.Buchungstag > minAndMaxRecordTime.Item1 && read.Buchungstag <= minAndMaxRecordTime.Item2);
		}

		private static Tuple<DateTime, DateTime> getMinAndMaxRecordTime(IEnumerable<NetBankRecordRead> records, Configuration config)
		{
			var maxRecordTime = records.Select(read => read.Buchungstag).Where(time => time < DateTime.Today).Max();
			var minRecordTime = config.LastWrite ?? DateTime.MinValue;

			return new Tuple<DateTime, DateTime>(minRecordTime, maxRecordTime);
		}

		private static void writeMaxRecordTimeToConfig(Configuration config, DateTime maxRecordTime)
		{
			config.LastWrite = maxRecordTime;
			var serializer = new XmlSerializer(typeof(Configuration));
			using (var writer = new XmlTextWriter(_configurationPath, Encoding.UTF8))
			{
				serializer.Serialize(writer, config);
			}
		}

		private static void writeRecords(IEnumerable<NetBankRecordWrite> recordWrites, string pathWrite)
		{
			var engine = new FileHelperEngine<NetBankRecordWrite>();
			engine.WriteFile(pathWrite, recordWrites);
		}

		private static void generateRecordWrites(IEnumerable<NetBankRecordRead> records, Configuration config, 
			Action<NetBankRecordWrite> continueForCompleteItems, Action<NetBankRecordWrite> continueForIncompleteItems)
		{
			foreach (var record in records
				.Select(netBankRecord => new { netBankRecord, kategory = GetCategoryAndEmpfänger(config, netBankRecord) })
				.Select(@t => new NetBankRecordWrite(@t.netBankRecord, @t.kategory.Item1, @t.kategory.Item2)))
			{
				if (String.IsNullOrEmpty(record.Empfänger))
					continueForIncompleteItems(record);
				else
					continueForCompleteItems(record);
			}
			
		}

		private static Tuple<string,string> GetCategoryAndEmpfänger(Configuration config, NetBankRecordRead netBankRecordRead)
		{
			foreach (var category in config.Categories
				.Where(category => category.Keywords
					.Any(keyword => netBankRecordRead.Verwendungszweck.ToLower().Contains(keyword.Name.ToLower()))))
			{
				return new Tuple<string, string>(category.Name,
					category.Keywords.Where(keyword => netBankRecordRead.Verwendungszweck.ToLower().Contains(keyword.Name.ToLower()))
						.Select(keyword => keyword.Empfänger ?? keyword.Name)
						.Single());
			}

			return new Tuple<string, string>("", "");
		}

		private static Configuration readConfig()
		{
			var serializer = new XmlSerializer(typeof(Configuration));
			var xdoc = new XmlDocument();
			xdoc.Load(_configurationPath);
			Configuration configuration;
			using (var fileStream = File.Open(_configurationPath, FileMode.Open))
			{
				configuration = (Configuration)serializer.Deserialize(fileStream);
			}

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
