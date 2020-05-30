using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ARKSS_Gui.Annotations;

namespace ARKSS_Gui
{
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public string SteamCmdDir
        {
            get => Properties.Settings.Default.SteamCmdDir;
            set
            {
                if (value == SteamCmdDir) return;
                Properties.Settings.Default.SteamCmdDir = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public string ServersDir
        {
            get => Properties.Settings.Default.ServersDir;
            set
            {
                if (value == ServersDir) return;
                Properties.Settings.Default.ServersDir = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
