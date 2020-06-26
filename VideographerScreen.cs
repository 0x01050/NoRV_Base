using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Base
{
    public partial class VideographerScreen : Form
    {
        private string videographer = "";
        private string commission = "";
        public VideographerScreen(string videographer, string commission)
        {
            this.videographer = videographer;
            this.commission = commission;
            InitializeComponent();
        }

        private void VideographerScreen_Load(object sender, EventArgs e)
        {
            txtVideographer.Text = videographer;
            txtCommission.Text = commission;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Application.UseWaitCursor = true;
                videographer = txtVideographer.Text;
                commission = txtCommission.Text;
                HttpClient client = new HttpClient();
                MultipartFormDataContent httpContent = new MultipartFormDataContent();
                httpContent.Add(new StringContent(videographer), "Videographer");
                httpContent.Add(new StringContent(commission), "Commission");
                HttpResponseMessage response = client.PostAsync(Config.getInstance().getServerUrl() + "/info", httpContent).Result;
                Application.UseWaitCursor = false;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(response.StatusCode.ToString());
                }
            }
            catch(Exception)
            {
                Application.UseWaitCursor = false;
            }
        }
    }
}
