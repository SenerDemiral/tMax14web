using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FB2SCfull
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FbLibrary.SendWithWebSocket.FrtSend("F");
            FbLibrary.SendWithWebSocket.OpmSend("F");
            FbLibrary.SendWithWebSocket.OphSend("F");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FbLibrary.SendWithWebSocket.AfbSend("F");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string data = "SENER";
            using (var client = new HttpClient())
            {
                var response = client.PostAsync("http://rest.tMax.online/DenemePut", new StringContent(data)).Result;
                if (response.IsSuccessStatusCode)
                {
                    Console.Write("Success");
                }
                else
                    Console.Write("Error");
            }
        }
    }
}
