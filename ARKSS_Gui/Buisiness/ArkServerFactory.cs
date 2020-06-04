using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ARKSS_Gui.Buisiness
{
    public class ArkServerFactory
    {
        private static ArkServerFactory _instance;
        public static ArkServerFactory Instance => _instance ?? (_instance = new ArkServerFactory());

        private readonly List<ArkServer> _servers = new List<ArkServer>();
        private JObject _jData;

        private ArkServerFactory() { }

        public IReadOnlyCollection<ArkServer> ServersSettings => _servers.AsReadOnly();

        public JObject JData => _jData ?? (_jData = JObject.Load(new JsonTextReader(File.OpenText("data.json"))));

        public bool AddServer(ArkServer arkServer, bool installed = false)
        {
            _servers.Add(arkServer);
            OnNewServer(arkServer);

            return true;
        }

        private bool CanInstall(ArkServer arkServer)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ArkServer> NewServer;
        protected virtual void OnNewServer(ArkServer e) => NewServer?.Invoke(this, e);
    }
}