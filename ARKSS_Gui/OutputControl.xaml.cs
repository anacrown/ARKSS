using System;
using System.Collections.Generic;
using System.Linq;
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
using Serilog.Events;

namespace ARKSS_Gui
{
    public partial class OutputControl : UserControl
    {
        public static readonly DependencyProperty LogsSinkProperty = DependencyProperty.Register(
            "LogsSink", typeof(LogsSink), typeof(OutputControl), new PropertyMetadata(default(LogsSink)));

        public LogsSink LogsSink
        {
            get => (LogsSink) GetValue(LogsSinkProperty);
            set => SetValue(LogsSinkProperty, value);
        }

        public OutputControl()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == LogsSinkProperty)
            {
                if (e.OldValue is LogsSink oldValue)
                    oldValue.EmitLog -= OnEmitLog;

                if (e.NewValue is LogsSink newValue)
                    newValue.EmitLog += OnEmitLog;
            }

            base.OnPropertyChanged(e);
        }

        private void OnEmitLog(object sender, LogEvent e)
        {
            var message = $"[{e.Timestamp}] {e.Level}: {e.RenderMessage()}\r\n";

            if (e.Exception != null)
                message += $"{e.Exception.Message}\r\n";

            MessagesBox.AppendText(message);
            MessagesBox.ScrollToEnd();
        }
    }
}
