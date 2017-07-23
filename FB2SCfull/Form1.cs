using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            FbLibrary.SendWithWebSocket.FrcSend("F");
            FbLibrary.SendWithWebSocket.OpmSend("F");
            FbLibrary.SendWithWebSocket.OphSend("F");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FbLibrary.SendWithWebSocket.AfbSend("F");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FbLibrary.SendWithWebSocket.FrcSend("F");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*
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
            }*/

            /*
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://rest.tMax.online/tMax14rest/DenemePut");

            Person person = new Person() { FirstName = "Iron", LastName = "Man", Age = 30 };
            Console.WriteLine("Before calling the PUT method");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("FirstName of the Person is : {0}", person.FirstName);
            Console.WriteLine("LastName of the Person is  : {0}", person.LastName);
            Console.WriteLine("Fullname of the Person is  : {0}", person.FullName);
            Console.WriteLine("Age of the Person is       : {0}", person.Age);
            Console.WriteLine();
            string serilized = JsonConvert.SerializeObject(person);
            var inputMessage = new HttpRequestMessage
            {
                Content = new StringContent(serilized, Encoding.UTF8, "application/json")
            };
            inputMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage message = client.PutAsync("api/trial", inputMessage.Content).Result;
            if (message.IsSuccessStatusCode)
            {
                var inter = message.Content.ReadAsStringAsync();
                
                Person reslutPerson = JsonConvert.DeserializeObject<Person>(inter.Result);
                Console.WriteLine("Person returned from PUT method:");
                Console.WriteLine("--------------------------------");
                Console.WriteLine("FirstName of the Person is : {0}", reslutPerson.FirstName);
                Console.WriteLine("LastName of the Person is  : {0}", reslutPerson.LastName);
                Console.WriteLine("Fullname of the Person is  : {0}", reslutPerson.FullName);
                Console.WriteLine("Age of the Person is       : {0}", reslutPerson.Age);
            }*/
            //text/html; charset=UTF-8

            dynamic jsn = new JObject();
            jsn.AD = "Dilara";
            jsn.YAS = 47;

            var cli = new WebClient();
            //cli.Headers[HttpRequestHeader.ContentType] = "application/json";
            cli.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            cli.UploadFile("http://rest.tmax.online/tmax14rest/denemeput", "PUT", @"C:\Starcounter\kemper.icde11.memory.pdf");
            //string response = cli.UploadString("http://rest.tmax.online/tmax14rest/denemeput", "PUT", JsonConvert.SerializeObject(jsn));//"{'Tbl': 'ZZZ'}");
    }

        private void button4_Click(object sender, EventArgs e)
        {
            FbLibrary.SendWithWebSocket.FrtSendWebClient("F");

        }

    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
    }
}
