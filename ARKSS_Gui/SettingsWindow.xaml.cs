using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ARKSS_Gui.Annotations;
using ARKSS_Gui.Buisiness;
using Ookii.Dialogs.Wpf;

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

        private void SelectSteamCmdDir()
        {
            var folderBrowserDialog = new VistaFolderBrowserDialog { ShowNewFolderButton = true };

            if (folderBrowserDialog.ShowDialog(this) == true)
                SteamCmdDir = folderBrowserDialog.SelectedPath;
        }

        private void SelectServersDir()
        {
            var folderBrowserDialog = new VistaFolderBrowserDialog { ShowNewFolderButton = true };

            if (folderBrowserDialog.ShowDialog(this) == true)
                ServersDir = folderBrowserDialog.SelectedPath;
        }

        private void ButtonBase_OnSelectSteamCmdDir(object sender, RoutedEventArgs e) => SelectSteamCmdDir();

        private void ButtonBase_OnSelectServersDir(object sender, RoutedEventArgs e) => SelectServersDir();

        private void ButtonBase_OnSteamCmdInstall(object sender, RoutedEventArgs e)
        {
            while (string.IsNullOrEmpty(SteamCmdDir))
                SelectSteamCmdDir();

            SteamCmd.Instance.Install();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
