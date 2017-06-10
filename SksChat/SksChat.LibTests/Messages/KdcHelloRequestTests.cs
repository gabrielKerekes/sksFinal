using Microsoft.VisualStudio.TestTools.UnitTesting;
using SksChat.Lib.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SksChat.Lib.Security.Asn1;

namespace SksChat.Lib.Messages.Tests
{
    [TestClass()]
    public class KdcHelloRequestTests
    {
        [TestMethod()]
        public void EncodeTest()
        {
            var kdcHelloRequest = new KdcHelloRequest("ABC", new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, });

            var encodedRequest = kdcHelloRequest.Encode();

            var expectedSequence = new byte[] {(byte) SksAsn1Type.Sequence, 0x0C, (byte) SksAsn1Type.Printablestring, 0x03, 65, 66, 67, (byte) SksAsn1Type.OctetString, 0x05, 0x00, 0x01, 0x02, 0x03, 0x04,};
            var expectedSequenceBase64 = Convert.ToBase64String(expectedSequence);
            var expected = $"-----BEGIN INITIAL MESSAGE 1-----\n{expectedSequenceBase64}\n-----END INITIAL MESSAGE 1";

            Assert.AreEqual(expected, encodedRequest);
        }
    }
}