using FileHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestFixture]
    public class CsvReadTests
    {
        [Test]
        public void TestRead01()
        {
            var engine = new FileHelperEngine<NetBankRecord>();
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "netbank1.xls");
            var records = engine.ReadFile(path);


        }
    }
}
