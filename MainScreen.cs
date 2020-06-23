using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Base
{
    public partial class MainScreen : Form
    {
        private string videographer = "";
        private string commission = "";

        public MainScreen()
        {
            InitializeComponent();
        }

        private void txtVideographer_MouseDown(object sender, MouseEventArgs e)
        {
            var scene = new VideographerScreen(videographer, commission);
            if (scene.ShowDialog() == DialogResult.OK)
            {
                FetchSettings();
            }
        }

        private async void FetchSettings()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Config.getInstance().getServerUrl() + "/info");
            if(response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();
                var respObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                foreach (var item in respObj)
                {
                    switch(item.Key)
                    {
                        case "Videographer":
                            videographer = item.Value;
                            break;
                        case "Commission":
                            commission = item.Value;
                            break;
                    }
                }
            }
            Invoke(new Action(() =>
            {
                txtVideographer.Text = videographer;
            }));
        }

        Thread fetchThread = null;
        private void MainScreen_Load(object sender, EventArgs e)
        {
            FetchSettings();
            fetchThread = new Thread(new ThreadStart(fetchProc));
            fetchThread.Start();
        }

        private void MainScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            fetchThread.Abort();
            fetchThread = null;
        }

        private void fetchProc()
        {
            while(true)
            {
                fetchWork();
                Thread.Sleep(Config.getInstance().getUpdateInterval() * 1000);
            }
        }

        private void fetchWork()
        {
            string cards = "";
            HttpClient client = new HttpClient();
            MultipartFormDataContent httpContent = new MultipartFormDataContent();
            HttpResponseMessage response = client.PostAsync(Config.getInstance().getServerUrl() + "/status", httpContent).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = response.Content.ReadAsStringAsync().Result;
                var respObj = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(content);
                foreach (var item in respObj)
                {
                    string card = Config.getInstance().getCardTemplate();
                    foreach(var info in item)
                    {
                        string key = info.Key;
                        string value = info.Value;
                        if(key.Contains("update"))
                        {
                            if(value == "0000-00-00 00:00:00")
                            {
                                value = "";
                            }
                            else
                            {
                                DateTime convertedDate = DateTime.SpecifyKind(DateTime.Parse(value), DateTimeKind.Utc);
                                convertedDate = convertedDate.AddHours(5);
                                convertedDate = convertedDate.ToLocalTime();
                                value = convertedDate.ToString("yyyy-MM-dd h:mm:sstt");
                            }
                        }
                        card = card.Replace("{$" + info.Key + "}", value);
                    }
                    card = card.Replace("<img src=\"data:image/png;base64, \" style=\"height: 100px\" alt=\"\" />", "<div style=\"height:100px\"></div>");
                    cards = cards + "\n" + card + "\n";
                }
            }
            string html = Config.getInstance().getHtmlTemplate();
            html = html.Replace("{$CARDS}", cards);
            Invoke(new Action(() =>
            {
                cardsBrowser.Navigate("about:blank");
                HtmlDocument doc = cardsBrowser.Document;
                doc.Write(String.Empty);
                cardsBrowser.DocumentText = html;
            }));
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Application.UseWaitCursor = true;
            fetchWork();
            Application.UseWaitCursor = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var scene = new AddScreen();
            if(scene.ShowDialog() == DialogResult.OK)
                fetchWork();
        }
    }
}
