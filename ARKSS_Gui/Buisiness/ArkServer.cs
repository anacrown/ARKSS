using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ARKSS_Gui.Annotations;
using Serilog;

namespace ARKSS_Gui.Buisiness
{
    public class ArkServer : INotifyPropertyChanged
    {
        private string _name;
        private string _dir;
        private string _mods;

        private readonly ILogger _logger;

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

        public string Mods
        {
            get => _mods;
            set
            {
                if (value == _mods) return;
                _mods = value;
                OnPropertyChanged();
            }
        }

        public ArkServer(string name, string dir, string mods)
        {
            Name = name;
            Dir = dir;
            Mods = mods;

            _logger = Log.Logger
                .ForContext("ClassName", GetType())
                .ForContext("ServerName", Name)
                .ForContext("Directory", Dir);
        }

        public void Update()
        {
            _logger.Information(@"Запуск установки\обновления");

            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);

            var mods = Mods.Split(',').Select(id => id.Trim()).Select(modId => (workshopId: Steam.ArkWorkshopId, modId: modId)).ToList();

            Steam.Update(Dir, Steam.ArkServerId, mods.ToArray());

            var modsDir = Path.Combine(Dir, "ShooterGame", "Content", "Mods");

            foreach (var mod in mods)
            {
                var downloadDir = Path.Combine(Dir, "steamapps", "workshop", "content", mod.workshopId);

                var downloadModDir = Path.Combine(downloadDir, mod.modId);
                if (Directory.Exists(downloadModDir))
                {
                    var modDir = Path.Combine(modsDir, mod.modId);
                    if (Directory.Exists(modDir))
                        Directory.Delete(modDir, true);

                    Directory.Move(downloadModDir, modDir);
                }
            }

            _logger.Information(@"установка\обновление завершено");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}