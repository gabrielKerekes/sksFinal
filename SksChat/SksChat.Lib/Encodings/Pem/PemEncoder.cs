using System.Collections.Generic;
using System.Linq;
using SksChat.Lib.Log;

namespace SksChat.Lib.Encodings.Pem
{
    public static class PemEncoder
    {
        private const string LogTag = "PEM_ENCODE";

        public static string CreatePemMessage(PemMessageType messageType, byte[] asn1Content)
        {
            var messageTitleKvp = PemParser.PemMessageTypeMap.FirstOrDefault(kvp => kvp.Value == messageType);
            if (messageTitleKvp.Equals(default(KeyValuePair<string, PemMessageType>)))
            {
                Logger.Log(LogTag, "ERROR: Unknown message");
                return "";
            }

            var messageTitle = messageTitleKvp.Key;

            return $"{BuildFirstLine(messageTitle)}\n{Utils.ToBase64(asn1Content)}\n{BuildLastLine(messageTitle)}\n";
        }

        private static string BuildFirstLine(string messageTitle)
        {
            return $"-----BEGIN {messageTitle}-----";
        }

        private static string BuildLastLine(string messageTitle)
        {
            return $"-----END {messageTitle}------";
        }
    }
}
