using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ARKSS_Gui.Annotations;
using ARKSS_Gui.Buisiness;
using Salaros.Configuration;

namespace ARKSS_Gui
{
    /// <summary>
    /// Interaction logic for ServerConfigManager.xaml
    /// </summary>
    public partial class ServerConfigManager : INotifyPropertyChanged
    {
        public static readonly DependencyProperty ArkServerProperty = DependencyProperty.Register(
            "ArkServer", typeof(ArkServer), typeof(ServerConfigManager), new PropertyMetadata(default(ArkServer)));

        private ObservableCollection<SettingsFileView> _settings;
        private SettingsItemView _selectedItemView;
        private string _selectedItemJData;

        public ArkServer ArkServer
        {
            get => (ArkServer) GetValue(ArkServerProperty);
            set => SetValue(ArkServerProperty, value);
        }

        public ObservableCollection<SettingsFileView> Settings
        {
            get => _settings;
            set
            {
                if (Equals(value, _settings)) return;
                _settings = value;
                OnPropertyChanged();
            }
        }

        public SettingsItemView SelectedItemView
        {
            get => _selectedItemView;
            set
            {
                if (Equals(value, _selectedItemView)) return;
                _selectedItemView = value;
                OnPropertyChanged();
            }
        }

        public string SelectedItemJData
        {
            get => _selectedItemJData;
            set
            {
                if (value == _selectedItemJData) return;
                _selectedItemJData = value;
                OnPropertyChanged();
            }
        }

        public ServerConfigManager()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ArkServerProperty && e.NewValue != null)
            {
                Settings = new ObservableCollection<SettingsFileView>(ArkServer.Settings.Select(setting => new SettingsFileView(setting.Key, setting.Value)));
            }

            base.OnPropertyChanged(e);
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItemView = e.NewValue as SettingsItemView;
            if (SelectedItemView != null)
            {
                SelectedItemJData = ArkServerFactory.Instance.JData[SelectedItemView.Section.File.Name]?["arguments"].Children().FirstOrDefault(c => c["argument"].ToString().ToLower().Contains(SelectedItemView.Name.ToLower()))?["effect"]?.ToString();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class SettingsFileView : INotifyPropertyChanged
    {
        private string _dir;
        private string _name;
        private ConfigParser _config;
        private ObservableCollection<SettingsSectionView> _sections;

        public string Dir
        {
            get => _dir;
            set
            {
                if (value == _dir) return;
                _dir = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SettingsSectionView> Sections
        {
            get => _sections;
            set
            {
                if (Equals(value, _sections)) return;
                _sections = value;
                OnPropertyChanged();
            }
        }

        public ConfigParser Config
        {
            get => _config;
            set
            {
                if (Equals(value, _config)) return;
                _config = value;
                OnPropertyChanged();
            }
        }

        public SettingsFileView(string dir, ConfigParser config)
        {
            Dir = dir;
            Config = config;
            Name = Path.GetFileName(dir);

            Sections = new ObservableCollection<SettingsSectionView>(Config.Sections.Select(section => new SettingsSectionView(section, this)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class SettingsSectionView : INotifyPropertyChanged
    {
        public ConfigSection Section { get; }
        public SettingsFileView File { get; }

        private string _name;
        private ObservableCollection<SettingsItemView> _items;

        public SettingsSectionView(ConfigSection section, SettingsFileView file)
        {
            Section = section;
            File = file;
            Name = Section.SectionName;
            Items = new ObservableCollection<SettingsItemView>(Section.Keys.Select(key => new SettingsItemView(key, this)));
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SettingsItemView> Items
        {
            get => _items;
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class SettingsItemView : INotifyPropertyChanged
    {
        public IConfigKeyValue Key { get; }
        public SettingsSectionView Section { get; }

        public SettingsItemView(IConfigKeyValue key, SettingsSectionView section)
        {
            Key = key;
            Section = section;
            Name = key.Name;
        }

        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
