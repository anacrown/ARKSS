using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ARKSS_Gui.Buisiness;

namespace ARKSS_Gui
{
    /// <summary>
    /// Interaction logic for ServerControlPanel.xaml
    /// </summary>
    public partial class ServerControlPanel : UserControl
    {
        public static readonly DependencyProperty ArkServerProperty = DependencyProperty.Register(
            "ArkServer", typeof(ArkServer), typeof(ServerControlPanel), new PropertyMetadata(default(ArkServer)));

        public ArkServer ArkServer
        {
            get => (ArkServer) GetValue(ArkServerProperty);
            set => SetValue(ArkServerProperty, value);
        }

        public ServerControlPanel()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnInstallServer(object sender, RoutedEventArgs e)
        {
            ArkServer.Update();
        }

        private void ButtonBase_OnDelete(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonBase_OnPlayerList(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonBase_OnRun(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonBase_OnStop(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
