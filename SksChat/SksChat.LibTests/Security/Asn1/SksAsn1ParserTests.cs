using Microsoft.VisualStudio.TestTools.UnitTesting;
using SksChat.Lib.Security.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SksChat.Lib.Encodings.Asn1;

namespace SksChat.Lib.Security.Asn1.Tests
{
    [TestClass()]
    public class SksAsn1ParserTests
    {
        [TestMethod()]
        public void ParseTest5AnybodyThere()
        {
            // 30 13 02 01 05 16 0e 41 6e 79 62 6f 64 79 20 74 68 65 72 65 3f - 5 anybody there?
            var message = new byte[] { 0x10, 0x13, 0x02, 0x01, 0x05, 0x13, 0x0e, 0x41, 0x6e, 0x79, 0x62, 0x6f, 0x64, 0x79, 0x20, 0x74, 0x68, 0x65, 0x72, 0x65, 0x3f, };

            var obj = SksAsn1Parser.Parse(message);
            var objList = (List<object>) obj;
            var number = (int) objList[0];
            var str = (string) objList[1];

            Assert.AreEqual(5, number);
            Assert.AreEqual("Anybody there?", str);
        }

        [TestMethod()]
        public void ParseTestKdcSample()
        {
            // kdc sample message
            var message = new byte[] { 0x17, 0x2B, 0x10, 0x29, 0x13, 0x0F, 0x31, 0x31, 0x31, 0x2E, 0x31, 0x31, 0x31, 0x2E, 0x31, 0x31, 0x31, 0x2E, 0x31, 0x31, 0x31, 0x02, 0x01, 0x05, 0x13, 0x0e, 0x41, 0x6e, 0x79, 0x62, 0x6f, 0x64, 0x79, 0x20, 0x74, 0x68, 0x65, 0x72, 0x65, 0x3f, 0x04, 0x00, 0x13, 0x01, 65, };

            var obj = SksAsn1Parser.Parse(message);
            var set = (List<object>)obj;
            var sequence = (List<object>) set[0];
            var ip = (string) sequence[0];
            var number = (int) sequence[1];
            var str = (string) sequence[2];
            var b = (byte[]) sequence[3];
            var str2 = (string)sequence[4];

            Assert.AreEqual("111.111.111.111", ip);
            Assert.AreEqual(5, number);
            Assert.AreEqual("Anybody there?", str);
            CollectionAssert.AreEqual(new byte[] {}, b);
            Assert.AreEqual("A", str2);
        }
    }
}