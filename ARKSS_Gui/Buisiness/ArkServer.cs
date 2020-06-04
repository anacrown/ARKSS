using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ARKSS_Gui.Annotations;
using Salaros.Configuration;
using Serilog;

namespace ARKSS_Gui.Buisiness
{
    public class ArkServer
    {
        public string Name;

        public string Dir;

        public string Mods;

        public Dictionary<string, ConfigParser> Settings;

        private ILogger _logger;

        public ArkServer(string dir = null)
        {
            Dir = dir;

            _logger = Log.Logger.ForContext("ClassName", GetType());

            if (!string.IsNullOrEmpty(dir))
                Load(dir);
        }

        public void Load(string dir)
        {
            Dir = dir;

            var configFiles = new[] {
                Path.Combine(Dir, "ShooterGame", "Saved", "Config", "WindowsServer", "Game.ini"),
                Path.Combine(Dir, "ShooterGame", "Saved", "Config", "WindowsServer", "GameUserSettings.ini")
            };

            Settings = configFiles.ToDictionary(file => file, file => new ConfigParser(file));

            Name = Settings[configFiles[1]].GetValue("SessionSettings", "SessionName");
            Mods = Settings[configFiles[1]].GetValue("ServerSettings", "ActiveMods");
            
            _logger = _logger.ForContext("ServerName", Name)
                             .ForContext("Directory", Dir);
        }

        public void Update()
        {
            _logger.Information(@"Запуск установки\обновления");

            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);

            var mods = Mods.Split(',').Select(id => id.Trim()).Select(modId => (workshopId: SteamCmd.ArkWorkshopId, modId: modId)).ToList();

            SteamCmd.Instance.UpdateApp(Dir, SteamCmd.ArkServerId, mods.ToArray());

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