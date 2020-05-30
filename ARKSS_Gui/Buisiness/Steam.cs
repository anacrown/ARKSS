using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Context;

namespace ARKSS_Gui.Buisiness
{
    public static class Steam
    {
        public static string ArkServerId = "376030";
        public static string ArkWorkshopId = "346110";

        private static string Dir => Properties.Settings.Default.SteamCmdDir;

        public static void Update(string installDir, string appId, (string workshopId, string modId)[] mods)
        {
            var args = $"+login anonymous +force_install_dir {installDir} +app_update {appId} ";
            args = mods.Aggregate(args, (current, mod) => current + $"+workshop_download_item {mod.workshopId} {mod.modId} ");
            args += "+quit";

            Run(args);
        }

        public static void Run(string args)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = Path.Combine(Dir, "steamcmd.exe");
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

        private static void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            using (LogContext.PushProperty("ClassName", typeof(Steam)))
            {
                Log.Logger.Information(e.Data);
            }
        }
    }
}