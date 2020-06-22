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
    public partial class AddScreen : Form
    {
        public AddScreen()
        {
            InitializeComponent();
        }

        private void AddScreen_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.UseWaitCursor = true;

            string id= txtID.Text;
            string master= txtMaster.Text;
            string slave = txtSlave.Text;

            HttpClient client = new HttpClient();
            MultipartFormDataContent httpContent = new MultipartFormDataContent();
            httpContent.Add(new StringContent(id), "id");
            httpContent.Add(new StringContent(master), "master");
            httpContent.Add(new StringContent(slave), "slave");
            HttpResponseMessage response = client.PostAsync(Config.getInstance().getServerUrl() + "/machine", httpContent).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                if (result == "Successfully Added")
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(result);
                }
            }
            else
            {
                MessageBox.Show(response.StatusCode.ToString());
            }
            Application.UseWaitCursor = false;
        }
    }
}
