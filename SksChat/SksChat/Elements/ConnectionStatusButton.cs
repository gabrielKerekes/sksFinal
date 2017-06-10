using System.Windows.Controls;
using System.Windows.Media;

namespace SksChat.Elements
{
    public class ConnectionStatusButton : Button
    {
        public enum Status
        {
            Connected, Disconnected,
        }

        private Status currentStatus;
        public Status CurrentStatus
        {
            get => currentStatus;
            set
            {
                currentStatus = value;
                if (currentStatus == Status.Connected)
                {
                    Content = "Disconnect";
                    Background = Brushes.Crimson;
                }
                else
                {
                    Content = "Connect";
                    Background = Brushes.DeepSkyBlue;
                }
            }
        }
    }
}
