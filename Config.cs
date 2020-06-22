using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Base
{
    class Config
    {
        private static Config _instance = null;
        public static Config getInstance()
        {
            if (_instance == null)
            {
                _instance = new Config();
            }
            return _instance;
        }


        Config()
        {
            LoadGlobalConfig();
        }

        // Global Config
        private string _serverUrl = "";
        private int _updateInterval = 300;
        private string _htmlTemplate = "";
        private string _cardTemplate = "";
        private void LoadGlobalConfig()
        {
            var xml = XDocument.Load(@"Config.xml");
            var query = from c in xml.Root.Descendants("Item")
                        select c;
            foreach (var item in query)
            {
                switch ((string)item.Attribute("Key"))
                {
                    case "ServerUrl":
                        _serverUrl = (string)item.Attribute("Value");
                        break;
                    case "UpdateInterval":
                        _updateInterval = (int)item.Attribute("Value");
                        break;
                    case "HtmlTemplate":
                        _htmlTemplate = item.Value.Replace("  ", "").Trim();
                        break;
                    case "CardTemplate":
                        _cardTemplate = item.Value.Replace("  ", "").Trim();
                        break;
                }
            }
        }
        public string getServerUrl()
        {
            return _serverUrl;
        }
        public int getUpdateInterval()
        {
            return _updateInterval;
        }
        public string getHtmlTemplate()
        {
            return _htmlTemplate;
        }
        public string getCardTemplate()
        {
            return _cardTemplate;
        }

    }
}
