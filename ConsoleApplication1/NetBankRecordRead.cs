using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    [DelimitedRecord("\t")]
    public class NetBankRecordRead
    {
		[FieldQuoted]
        [FieldConverter(ConverterKind.Date, "dd.MM.yyyy")]
        public DateTime Buchungstag;
        [FieldQuoted]
		[FieldConverter(ConverterKind.Date, "dd.MM.yyyy")]
		public DateTime Wertstellungstag;
        [FieldQuoted]
        public string Verwendungszweck;
        [FieldQuoted]
		[FieldConverter(ConverterKind.Decimal, ",")]
		public decimal Umsatz;
        [FieldQuoted]
        public string Währung;
        [FieldQuoted]
        public string KeineAhnung;
    }

	public class NetBankRecordWrite : NetBankRecordRead
	{
		[FieldQuoted]
		public string Empfänger;

		[FieldQuoted]
		public string Kategorie;

		public NetBankRecordWrite()
		{
			
		}

		public NetBankRecordWrite(string kategorie)
		{
			Kategorie = kategorie;
		}

		public NetBankRecordWrite(NetBankRecordRead recordRead, string kategorie, string empfänger)
		{
			Empfänger = empfänger;
			Buchungstag = recordRead.Buchungstag;
			Kategorie = kategorie;
			KeineAhnung = recordRead.KeineAhnung;
			Umsatz = recordRead.Umsatz;
			Verwendungszweck = recordRead.Verwendungszweck;
			Wertstellungstag = recordRead.Wertstellungstag;
			Währung = recordRead.Währung;
			
		}
	}
}
