using Microsoft.VisualStudio.TestTools.UnitTesting;
using SksChat.Lib.Security.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SksChat.Lib.Messages;

namespace SksChat.Lib.Security.Asn1.Tests
{
    [TestClass()]
    public class PemParserTests
    {
        [TestMethod()]
        public void GetMessageBytesTest()
        {
            // todo: refactor
            var expectedSequence = new byte[] { (byte)SksAsn1Type.Sequence, 0x0E, (byte)SksAsn1Type.Printablestring, 0x03, 65, 66, 67, (byte)SksAsn1Type.OctetString, 0x05, 0x00, 0x01, 0x02, 0x03, 0x04, };
            var expectedSequenceBase64 = Convert.ToBase64String(expectedSequence);
            var expected = $"-----BEGIN INITIAL MESSAGE 1-----{expectedSequenceBase64}-----END INITIAL MESSAGE 1";

            var messageBytes = PemParser.GetMessageBytes(expected);
            CollectionAssert.AreEqual(expectedSequence, messageBytes);
        }

        [TestMethod()]
        public void GetMessageTypeTest()
        {
            // todo: refactor
            var expectedSequence = new byte[] { (byte)SksAsn1Type.Sequence, 0x0E, (byte)SksAsn1Type.Printablestring, 0x03, 65, 66, 67, (byte)SksAsn1Type.OctetString, 0x05, 0x00, 0x01, 0x02, 0x03, 0x04, };
            var expectedSequenceBase64 = Convert.ToBase64String(expectedSequence);
            var expected = $"-----BEGIN INITIAL MESSAGE 1-----{expectedSequenceBase64}-----END INITIAL MESSAGE 1";

            var expectedType = PemMessageType.InitialMessage1;
            var messageType = PemParser.GetMessageType(expected);
            Assert.AreEqual(expectedType, messageType);
        }
    }
}