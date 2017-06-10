using System;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SksChat.Elements;
using SksChat.Lib;
using SksChat.Lib.Database;
using SksChat.Lib.Log;
using SksChat.Lib.Messages;
using SksChat.Lib.Security.Asn1;
using SksChat.Lib.Security.Encryption;

namespace SksChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly byte[] KdcKey = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        private object ReceivedMessagesTextBlockLock = new object();

        private static SksServer server;
        private static SksClient client;

        public MainWindow()
        {
            InitializeComponent();

            // todo: tu skoncene..treba nejak poriesit init celej libky, alebo daco take 
            Logger.Initialize("logFile.txt");

            using (Aes myAes = Aes.Create())
            {
                var b = SksAes.EncryptStringToBytes_Aes("gabrielkerekesatakdalejloremipsumdolor", myAes.Key, myAes.IV);
                var t = SksAes.DecryptStringFromBytes_Aes(b, myAes.Key, myAes.IV);
            }

            //SksSqlite.Init();
            //SksSqlite.AddNewUser("gabrielkerekes", "127.0.0.1", "123", "123");

            //SksAsn1Parser.ParseTest();

            var u = new SksUser();
            u.Password = "gabrielkerekes";
            u.Key = KdcKey;

            var kid = u.KeyId;
            var pid = u.PasswordId;
        }

        #region client

        private void ClientToggleStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (client == null)
                // todo: try catch na parse
                client = new SksClient(UsernameTextBox.Text, ClientIpTextBox.Text, int.Parse(ClientPortTextBox.Text));

            if (!client.Connected)
            {
                if (!client.Connect())
                {
                    MessageBox.Show("Client couldn't connect");
                    return;
                }

                ClientToggleStatusButton.CurrentStatus = ConnectionStatusButton.Status.Connected;
            }
            else
            {
                client?.Disconnect();
                ClientToggleStatusButton.CurrentStatus = ConnectionStatusButton.Status.Disconnected;
            }
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            client?.SendMessage(MessageTextBox.Text);
            //var key = KdcKey;
            //client?.SendMessage(Convert.ToBase64String(SksAes.EncryptStringToBytes_Aes("Hi I am \"gabrielkerekes\"", key, key)));
        }

        #endregion
   
        #region server      

        private void ServerToggleStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (server == null)
                // todo: try catch na parse
                server = new SksServer(int.Parse(ServerPortTextBox.Text));

            if (!server.Connected)
            {
                server.Connect();
                server.MessageReceived -= Server_MessageReceived;
                server.MessageReceived += Server_MessageReceived;

                ServerToggleStatusButton.CurrentStatus = ConnectionStatusButton.Status.Connected;
            }
            else
            {
                if (server == null)
                    return;

                server.MessageReceived -= Server_MessageReceived;
                server.Disconnect();

                ServerToggleStatusButton.CurrentStatus = ConnectionStatusButton.Status.Disconnected;
            }
        }

        private void Server_MessageReceived(object source, SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            lock (ReceivedMessagesTextBlockLock)
            {
                Dispatcher.Invoke(() => { ReceivedMessagesTextBlock.Text += $"{sksMessageReceivedEventArgs.Message}\n"; });
            }
        }

        #endregion

        #region kdc

        private void KdcInitButton_OnClick(object sender, RoutedEventArgs e)
        {
            var name = "gabrielkerekes";
            var encryptedHiMessage = SksAes.EncryptStringToBytes_Aes($"Hi I am {name}", KdcKey, KdcKey);

            var message = new KdcHelloRequest(name, encryptedHiMessage);

            var encodedMessage = message.Encode();
        }

        #endregion
    }
}
