using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Ionic.Zip;
using Serilog;
using Serilog.Context;

namespace ARKSS_Gui.Buisiness
{
    public class SteamCmd
    {
        private static SteamCmd _instance;
        public static SteamCmd Instance => _instance ??= new SteamCmd();

        public static string ArkServerId = "376030";
        public static string ArkWorkshopId = "346110";

        private ILogger _logger;

        private SteamCmd()
        {
            _logger = Log.Logger.ForContext("ClassType", GetType());
        }

        public void Install()
        {
            if (File.Exists(Path.Combine(Properties.Settings.Default.SteamCmdDir, "steamcmd.exe")))
                return;

            _logger.Information("download steamCMD {SteamCmdDir}", Properties.Settings.Default.SteamCmdDir);

            var zipFile = Path.Combine(Properties.Settings.Default.SteamCmdDir, Path.GetFileName(Properties.Settings.Default.SteamCmdDownloadLink));

            using (var web = new WebClient())
                web.DownloadFile(Properties.Settings.Default.SteamCmdDownloadLink, zipFile);

            _logger.Information("extract {ZipFile} to {SteamCmdDir}", zipFile, Properties.Settings.Default.SteamCmdDir);

            using (var zip = ZipFile.Read(zipFile))
                zip.ExtractAll(Properties.Settings.Default.SteamCmdDir, ExtractExistingFileAction.OverwriteSilently);

            _logger.Information("delete downloaded zip file {ZipFile}", zipFile);

            File.Delete(zipFile);

            Run("+quit");
        }

        public void UpdateApp(string installDir, string appId, (string workshopId, string modId)[] mods)
        {
            var args = $"+login anonymous +force_install_dir {installDir} +app_update {appId} ";
            args = mods.Aggregate(args, (current, mod) => current + $"+workshop_download_item {mod.workshopId} {mod.modId} ");
            args += "+quit";

            Run(args);
        }

        public void Run(string args)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = Path.Combine(Properties.Settings.Default.SteamCmdDir, "steamcmd.exe");
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += ProcessOnOutputDataReceived;

                process.Start();
                process.BeginOutputReadLine();

                //process.WaitForExit();
            }
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            using (LogContext.PushProperty("ClassName", typeof(SteamCmd)))
            {
                Log.Logger.Information(e.Data);
            }
        }
    }
}