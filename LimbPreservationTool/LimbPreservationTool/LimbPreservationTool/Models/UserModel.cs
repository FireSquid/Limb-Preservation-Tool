using LimbPreservationTool.Models;
using LimbPreservationTool.Views;
using System;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
namespace LimbPreservationTool.Models
{
    public class Scan : SplittableHttpContent, IFetchableHttpContent
    {
        public Tuple<int, int> size { get; set; }
        public String patientID { get; set; }
        public String date { get; set; }
        public byte[] image { get; set; }

        public override List<HttpRequestMessage> ToHttpList()
        {

            IEnumerable<string> contentchunks = Split(Convert.ToBase64String(image), 100000);
            List<HttpRequestMessage> messages = new List<HttpRequestMessage>();

            foreach (string cc in contentchunks)
            {
                var content = new Dictionary<string, string> {
                                { "chunk",cc}

                            };
                HttpRequestMessage request = new HttpRequestMessage
                {

                    Method = HttpMethod.Post,

                    //RequestUri = new Uri("http://ec2-user@ec2-184-169-147-75.us-west-1.compute.amazonaws.com:5000/analyze?patientID="),
                    RequestUri = new Uri("http://miwpro.local:5000/analyze?patientID=" + patientID + "&date=" + date),
                    Content = new StringContent(JsonConvert.SerializeObject(content), System.Text.Encoding.UTF8, "application/json")
                };

                messages.Add(request);
            };
            return messages;


        }

        public HttpRequestMessage Fetch()
        {

            var content = new Dictionary<string, string> {
                    {"patientID",patientID },
                    {"date", date },
            };
            HttpRequestMessage request = new HttpRequestMessage
            {

                Method = HttpMethod.Get,
                RequestUri = new Uri("http://miwpro.local:5000/analyze?patientID=" + patientID + "&date=" + date),
                //Content = new StringContent(JsonConvert.SerializeObject(content), System.Text.Encoding.UTF8)
            };
            return request;
        }

    }




    public class Doctor
    {
        private static Doctor instance;
        readonly String ID;
        readonly String password;
        readonly HttpClient client;

        readonly HttpClientHandler h;
        List<Patient> PatientList;
        private Doctor(String user_id, String user_password)
        {
            ID = user_id;
            password = user_password;

        }


        public static async Task<Doctor> CreateInstance(String user_id, String user_password)
        {
            instance = new Doctor(user_id, user_password);
            Console.WriteLine($"user model created \n{user_id} \n{user_password}");
            //login check with database
            return instance;
        }
        public static Doctor GetInstance()
        {
            return instance;
        }

        public void LogOut()
        {
            instance = null;
        }

        public async Task<String> Examine(Stream imageStream)
        {
            var ms = new MemoryStream();
            imageStream.CopyTo(ms);
            byte[] imageBytes = ms.ToArray();

            //string image = Convert.ToBase64String(imageBytes);
            //demo id

            Scan scan = new Scan() { patientID = "123456789", date = "1970-01-01_10:00:00", image = imageBytes };

            string jsonScan = JsonConvert.SerializeObject(scan);

            //string jsonScan = new string('*', 100000);

            //Console.WriteLine("jsonScan:\n {0} ", image);
            Console.WriteLine("jsonScan size: {0} ", jsonScan.Length);

            try
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                await Client.GetInstance().SendRequestChunksAsync(scan);
                HttpResponseMessage scanResult = await Client.GetInstance().GetRequestAsync(scan);

                //TODO: extra steps for decoding ResponseMessage

                watch.Stop();

                Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds } ms");

                return "request finished";
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            return "request not finished";
        }

        //public void LookUpRecentScore(String patientID)
        //{

        //    //Scan scan = new Scan() { patientID = "12345678910", date = "1970-01-01 10:00:00", image = image };
        //    //string jsonScan = JsonConvert.SerializeObject(scan);
        //    //var client = new HttpClient();
        //    //HttpResponseMessage response = await client.PostAsync(new Uri("http://ec2-user@ec2-184-169-147-75.us-west-1.compute.amazonaws.com:5000/analyze"), new StringContent(jsonScan));
        //}

    }


}