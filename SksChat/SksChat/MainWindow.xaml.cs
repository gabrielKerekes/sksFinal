using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SksChat.Elements;
using SksChat.Lib;
using SksChat.Lib.Database;
using SksChat.Lib.Log;
using SksChat.Lib.Messages;
using SksChat.Lib.Messages.Kdc;
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
        public List<UserRadioButton> UserRadioButtons { get; set; }

        private User selectedUser;

        public MainWindow()
        {
            InitializeComponent();

            var logFileName = $"logFile{DateTime.Now.Ticks}.txt";
            Lib.Lib.Init("gabrielkerekes", logFileName);
            LogFileNameLabel.Content = logFileName;

            //using (Aes myAes = Aes.Create())
            //{
            //    var b = SksAes.EncryptBytes_Aes(KdcKey.Concat(KdcKey).Concat(KdcKey).ToArray(), myAes.Key, myAes.IV);
            //    var t = SksAes.DecryptBytesFromBytes_Aes(b, myAes.Key, myAes.IV);
            //}

            //SksSqlite.Init();
            //SksSqlite.AddNewUser("gabrielkerekes", "127.0.0.1", "123", "123");

            //SksAsn1Parser.ParseTest();

            //var u = new User();
            //u.Password = "gabrielkerekes";
            //u.Key = KdcKey;

            //var kid = u.KeyId;
            //var pid = u.PasswordId;

            //SksClient.NewClientCreated += (source, args) =>
            //{
            //    ((SksClient) source).MessageReceived += Server_MessageReceived;
            //};

            InitKdc();
        }

        #region client

        private void ClientToggleStatusButton_Click(object sender, RoutedEventArgs e)
        {
            var user = selectedUser;
            if (user == null)
                // todo: some error - user doesn't exist
                return;
            
            if (user.Client == null)
                user.Client = new SksClient(user);

            if (!user.Client.Connected)
            {
                if (!user.Client.Connect())
                {
                    MessageBox.Show("Client couldn't connect");
                    return;
                }

                user.Client.ChatMessageReceived += Server_MessageReceived;  

                ClientToggleStatusButton.CurrentStatus = ConnectionStatusButton.Status.Connected;
            }
            else
            {
                user.Client?.Disconnect();

                ClientToggleStatusButton.CurrentStatus = ConnectionStatusButton.Status.Disconnected;
            }
        }

        // todo: aj odoslane spravy davat do textboxu ale s nejakym prepend....prepend pridat
        // aj na prisle spravy...nech pise ze kto to poslal ...
        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            //selectedUser.Client?.SendMessage(MessageTextBox.Text);
            selectedUser.SendChatMessage(MessageTextBox.Text);
            ReceivedMessagesTextBlock.Text += $"{selectedUser.Name} <- {MessageTextBox.Text}\n";
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
                server.Connect(Server_MessageReceived);
                //server.MessageReceived -= Server_MessageReceived;
                //server.MessageReceived += Server_MessageReceived;

                ServerToggleStatusButton.CurrentStatus = ConnectionStatusButton.Status.Connected;
            }
            else
            {
                if (server == null)
                    return;

                //server.MessageReceived -= Server_MessageReceived;
                server.Disconnect();

                ServerToggleStatusButton.CurrentStatus = ConnectionStatusButton.Status.Disconnected;
            }
        }

        private void Server_MessageReceived(object source, SksMessageReceivedEventArgs sksMessageReceivedEventArgs)
        {
            lock (ReceivedMessagesTextBlockLock)
            {
                if (sksMessageReceivedEventArgs.FromIp == "147.175.127.10" && sksMessageReceivedEventArgs.FromPort == 54321)
                {
                    var users = KdcHelloResponse.FromString(sksMessageReceivedEventArgs.Message, Lib.Lib.KdcKey).Users;

                    AddUsersRadioButtons(users);
                    Lib.Lib.AddUsers(users);

                    return;
                }

                var user = Lib.Lib.GetUserByIpAndPort(sksMessageReceivedEventArgs.FromIp, sksMessageReceivedEventArgs.FromPort);
                Dispatcher.Invoke(() => { ReceivedMessagesTextBlock.Text += $"{user.Name} -> {sksMessageReceivedEventArgs.Message}\n"; });
            }
        }

        #endregion

        #region kdc

        private void KdcInitButton_OnClick(object sender, RoutedEventArgs e)
        {
            InitKdc();
        }

        private void InitKdc()
        {
            try
            {
                var name = "gabrielkerekes";

                var message = new KdcHelloRequest(name, Lib.Lib.KdcKey);

                var encodedMessage = message.ToString();
                var kdcClient = new SksClient(new User {IpAddress = "147.175.127.10", Port = "54321"});
                kdcClient.MessageReceived += Server_MessageReceived;
                kdcClient.Connect();
                kdcClient.SendMessage(encodedMessage);
            }
            catch (Exception e)
            {
                Logger.Log("ERROR", $"Some exception {e.Message}");
                var user1 = new User { Name = "gabo1", IpAddress = "127.0.0.1", Port = "56789", Key = KdcKey, Password = "asdasdasdasdasd", };
                var user2 = new User { Name = "gabo2" };
                var user3 = new User { Name = "gabo3" };
                var user4 = new User { Name = "gabo4" };
                var users = new List<User> { user1, user2, user3, user4, };

                AddUsersRadioButtons(users);
                Lib.Lib.AddUsers(users);
            }
        }

        private void AddUsersRadioButtons(List<User> users)
        {
            UserRadioButtons = new List<UserRadioButton>();

            foreach (var user in users)
            {
                var radioButton = new UserRadioButton(user);
                radioButton.Checked += RadioButton_Checked;

                UserRadioButtons.Add(radioButton);
                RadioButtonsStackPanel.Children.Add(radioButton);
            }

            UserRadioButtons[0].IsChecked = true;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var radioButton in UserRadioButtons)
            {
                if (!radioButton.IsChecked.HasValue || !radioButton.IsChecked.Value)
                {
                    continue;
                }

                selectedUser = radioButton.User;
                break;
            }
        }

        #endregion
    }
}
