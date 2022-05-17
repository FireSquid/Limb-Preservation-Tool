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
        public String patientID { get; set; }
        public String date { get; set; }
        public Stream imageStream { get; set; }

        public override List<HttpRequestMessage> ToHttpList()
        {
            var ms = new MemoryStream();
            imageStream.CopyTo(ms);
            List<byte[]> contentchunks = Split(ms.ToArray(), 100000);
            List<HttpRequestMessage> messages = new List<HttpRequestMessage>();

            Console.WriteLine("chunk before conversion:{0}", ms.ToArray().Length);
            Console.WriteLine("chunk before conversion:{0}", Convert.ToBase64String(ms.ToArray()).Length);
            foreach (var cc in contentchunks)
            {
                var content = new Dictionary<string, string> {
                                { "chunk", Convert.ToBase64String(cc)}

                            };

                HttpRequestMessage request = new HttpRequestMessage
                {

                    Method = HttpMethod.Post,

                    RequestUri = new Uri("http://ec2-184-169-147-75.us-west-1.compute.amazonaws.com:5000/analyze?patientID=" + patientID + "&date=" + date),
                    //RequestUri = new Uri("http://miwpro.local:5000/analyze?patientID=" + patientID + "&date=" + date),
                    Content = new StringContent(JsonConvert.SerializeObject(content), System.Text.Encoding.UTF8, "application/json")
                    //Content = new ByteArrayContent(cc);
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
                RequestUri = new Uri("http://ec2-184-169-147-75.us-west-1.compute.amazonaws.com:5000/analyze?patientID=" + patientID + "&date=" + date),
                //RequestUri = new Uri("http://miwpro.local:5000/analyze?patientID=" + patientID + "&date=" + date),
                //Content = new StringContent(JsonConvert.SerializeObject(content), System.Text.Encoding.UTF8)
            };
            return request;
        }

        public static async Task<Scan> Decode(HttpResponseMessage m)
        {
            Scan s = new Scan();
            s.imageStream = await m.Content.ReadAsStreamAsync();
            return s;
        }

    }




    public class Doctor
    {
        private static Doctor instance;
        public String ID;
        readonly String password;
        readonly HttpClient client;

        readonly HttpClientHandler h;
        List<Patient> PatientList;
        private Doctor(String user_id, String user_password)
        {
            ID = user_id;
            password = user_password;

        }


        public static Doctor CreateInstance(String user_id, String user_password)
        {
            instance = new Doctor(user_id, user_password);
            Console.WriteLine($"user model created \n{user_id} \n{user_password}");
            //login check with database
            return instance;
        }
        public static Doctor GetInstance()
        {
            if (instance == null)
                CreateInstance("TestUser", "12345");
            return instance;
        }

        public void LogOut()
        {
            instance = null;
        }

        public async Task<Stream> Examine(Stream imageStream)
        {
            Console.WriteLine("-------------Examining");
            Console.WriteLine("-------------Created Stream");
            Scan scan = new Scan()
            {
                patientID = "123456789",
                date = DateTime.Now.ToString().Replace(' ', '_').Replace('/', '-'),
                imageStream = imageStream
            };
            Console.WriteLine("-------------Created Scan");

            try
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                Console.WriteLine("-------------Sending Scan");
                await Client.GetInstance().SendRequestChunksAsync(scan);
                HttpResponseMessage scanResult = await Client.GetInstance().GetRequestAsync(scan);
                Console.WriteLine("-------------Received Result");
                Scan result = await Scan.Decode(scanResult);
                Console.WriteLine("-------------Decoded Result");

                //Console.WriteLine(scanResult.Content.ToString());
                //TODO: extra steps for decoding ResponseMessage

                watch.Stop();

                Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds } ms");

                return result.imageStream;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            return Stream.Null;
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