using System;
using System.Collections.Generic;

namespace SksChat.Lib.Encodings.Pem
{
    public static class PemMessageTypes
    {
        public const string LongTermKeyTitle = "LONG TERM KEY";
        public const string InitialMessage1Title = "INITIAL MESSAGE 1";
        public const string InitialMessage2Title = "INITIAL MESSAGE 2";
        public const string ErrorMessageTitle = "ERROR MESSAGE";
        public const string ChatMessageTitle = "CHAT MESSAGE";
        public const string Akep2Message1Title = "AKEP2 MESSAGE 1";
        public const string Akep2Message2Title = "AKEP2 MESSAGE 2";
        public const string Akep2Message3Title = "AKEP2 MESSAGE 3";
        public const string OtwayReesMessage1Title = "OTWAY REES MESSAGE 1";
        public const string OtwayReesMessage2Title = "OTWAY REES MESSAGE 2";
        public const string OtwayReesMessage3Title = "OTWAY REES MESSAGE 3";
        public const string OtwayReesMessage4Title = "OTWAY REES MESSAGE 4";
        public const string HandshakeMessage1Title = "HANDSHAKE MESSAGE 1";
        public const string HandshakeMessage2Title = "HANDSHAKE MESSAGE 2";
        public const string HandshakeMessage3Title = "HANDSHAKE MESSAGE 3";
        public const string HandshakeMessage4Title = "HANDSHAKE MESSAGE 4";
        public const string SshTlsMessage1Title = "SSH TLS MESSAGE 1";
        public const string SshTlsMessage2Title = "SSH TLS MESSAGE 2";
        public const string SshTlsMessage3Title = "SSH TLS MESSAGE 3";
    }

    public enum PemMessageType
    {
        LongTermKey,
        InitialMessage1,
        InitialMessage2,
        ErrorMessage,
        ChatMessage,
        Akep2Message1,
        Akep2Message2,
        Akep2Message3,
        OtwayReesMessage1,
        OtwayReesMessage2,
        OtwayReesMessage3,
        OtwayReesMessage4,
        HandshakeMessage1,
        HandshakeMessage2,
        HandshakeMessage3,
        HandshakeMessage4,
        SshTlsMessage1,
        SshTlsMessage2,
        SshTlsMessage3,
    }

    public static class PemParser
    {
        public static readonly Dictionary<string, PemMessageType> PemMessageTypeMap = new Dictionary<string, PemMessageType>
        {
            { PemMessageTypes.LongTermKeyTitle, PemMessageType.LongTermKey },
            { PemMessageTypes.InitialMessage1Title, PemMessageType.InitialMessage1 },
            { PemMessageTypes.InitialMessage2Title, PemMessageType.InitialMessage2 },
            { PemMessageTypes.ErrorMessageTitle, PemMessageType.ErrorMessage },
            { PemMessageTypes.ChatMessageTitle, PemMessageType.ChatMessage },
            { PemMessageTypes.Akep2Message1Title, PemMessageType.Akep2Message1 },
            { PemMessageTypes.Akep2Message2Title, PemMessageType.Akep2Message2 },
            { PemMessageTypes.Akep2Message3Title, PemMessageType.Akep2Message3 },
            { PemMessageTypes.OtwayReesMessage1Title, PemMessageType.OtwayReesMessage1 },
            { PemMessageTypes.OtwayReesMessage2Title, PemMessageType.OtwayReesMessage2 },
            { PemMessageTypes.OtwayReesMessage3Title, PemMessageType.OtwayReesMessage3 },
            { PemMessageTypes.OtwayReesMessage4Title, PemMessageType.OtwayReesMessage4 },
            { PemMessageTypes.HandshakeMessage1Title, PemMessageType.HandshakeMessage1 },
            { PemMessageTypes.HandshakeMessage2Title, PemMessageType.HandshakeMessage2 },
            { PemMessageTypes.HandshakeMessage3Title, PemMessageType.HandshakeMessage3 },
            { PemMessageTypes.HandshakeMessage4Title, PemMessageType.HandshakeMessage4 },
            { PemMessageTypes.SshTlsMessage1Title, PemMessageType.SshTlsMessage1 },
            { PemMessageTypes.SshTlsMessage2Title, PemMessageType.SshTlsMessage2 },
            { PemMessageTypes.SshTlsMessage3Title, PemMessageType.SshTlsMessage3 },
        };

        public static PemMessageType GetMessageType(string message)
        {
            if (!message.StartsWith("-----BEGIN"))
                return PemMessageType.ChatMessage;

            message = RemoveNewLinesFromMessage(message);

            var messageSplit = SplitMessage(message);

            return PemMessageTypeMap[messageSplit[0].Substring(6)];
        }

        private static string[] SplitMessage(string message)
        {
            return message.Split(new[] { "-----" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string RemoveNewLinesFromMessage(string message)
        {
            return message.Replace("\n", "");
        }

        public static byte[] GetMessageContentBytes(string message)
        {
            message = RemoveNewLinesFromMessage(message);

            var messageSplit = message.Split(new[] {"-----"}, StringSplitOptions.RemoveEmptyEntries);

            return Utils.FromBase64(messageSplit[1]);
        }
    }
}
