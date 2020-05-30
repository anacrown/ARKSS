using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ARKSS_Gui.Annotations;
using ARKSS_Gui.Buisiness;
using Microsoft.VisualStudio.PlatformUI;
using Salaros.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Path = System.IO.Path;

namespace ARKSS_Gui
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ILogger _logger;
        private MainViewModel _model;

        public MainViewModel Model
        {
            get => _model;
            set
            {
                if (Equals(value, _model)) return;
                _model = value;
                OnPropertyChanged();
            }
        }

        public LogsSink LogsSink { get; set; } = new LogsSink();

        public MainWindow()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Sink(LogsSink)
                .WriteTo.Seq("http://127.0.0.1:5341")
                .Enrich.WithProperty("ProgramName", "ARKSS_Gui")
                .CreateLogger();

            _logger = Log.Logger.ForContext("ClassType", GetType());
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _logger.Information("MainWindow loaded");

            if (string.IsNullOrEmpty(Properties.Settings.Default.SteamCmdDir) ||
                string.IsNullOrEmpty(Properties.Settings.Default.ServersDir))
                new SettingsWindow().ShowDialog();

            Model = new MainViewModel();

            foreach (var server in LoadServersSettings()) Model.ServerFactory.AddServer(server, true);

            Model.SelectedServerSettingsIndex = 0;
        }

        private IEnumerable<ArkServer> LoadServersSettings()
        {
            foreach (var dir in Directory.GetDirectories(Properties.Settings.Default.ServersDir))
            {
                var shooterGame = Path.Combine(dir, "ShooterGame", "Binaries", "Win64", "ShooterGameServer.exe");
                if (!File.Exists(shooterGame))
                    continue;

                var gameUserSettings = new ConfigParser(Path.Combine(dir, "ShooterGame", "Saved", "Config", "WindowsServer", "GameUserSettings.ini"));

                var tab = new ArkServer(gameUserSettings.GetValue("SessionSettings", "SessionName"), dir,
                    gameUserSettings.GetValue("ServerSettings", "ActiveMods"));

                yield return tab;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public ArkServerFactory ServerFactory => ArkServerFactory.Instance;

        private DelegateCommand<object> _newCommand;
        private int _selectedServerSettingsIndex;

        private ObservableCollection<ArkServer> _servers;

        public ObservableCollection<ArkServer> Servers
        {
            get
            {
                if (_servers == null)
                {
                    _servers = new ObservableCollection<ArkServer>();
                    var itemsView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(_servers);
                    itemsView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
                }

                return _servers;
            }
        }

        public int SelectedServerSettingsIndex
        {
            get => _selectedServerSettingsIndex;
            set
            {
                if (value == _selectedServerSettingsIndex) return;
                _selectedServerSettingsIndex = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand<object> NewCommand
        {
            get
            {
                if (_newCommand == null)
                {
                    _newCommand = new DelegateCommand<object>(New_Execute);
                }

                return _newCommand;
            }
        }

        public MainViewModel()
        {
            ServerFactory.NewServer += ServerFactoryOnNewServer;
        }

        private void ServerFactoryOnNewServer(object sender, ArkServer e)
        {
            Servers.Add(e);
        }

        private void New_Execute(object parameter)
        {
            ServerFactory.AddServer(new ArkServer("New server", "", ""));
            SelectedServerSettingsIndex = ServerFactory.ServersSettings.Count - 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate NewButtonTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) => item == CollectionView.NewItemPlaceholder ? NewButtonTemplate : ItemTemplate;
    }

    public class LogsSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent) => EmitLog?.Invoke(null, logEvent);

        public event EventHandler<LogEvent> EmitLog;
    }
}
