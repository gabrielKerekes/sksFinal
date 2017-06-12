using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SksChat.Lib;

namespace SksChat.Elements
{
    public class UserRadioButton : RadioButton
    {
        public const string UserRadioButtonsGroupName = "UserRadioButtons";

        public User User { get; set; }

        public UserRadioButton(User user)
        {
            User = user;

            Content = user.Name;
            GroupName = UserRadioButtonsGroupName;
        }
    }
}
