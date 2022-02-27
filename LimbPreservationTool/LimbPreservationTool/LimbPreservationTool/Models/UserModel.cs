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
    public class Scan
    {

        public String patientID { get; set; }
        public String date { get; set; }
        public String image { get; set; }

    }
    public class Doctor
    {
        private static Doctor instance;
        readonly String ID;
        readonly String password;
        readonly HttpClient client;
        List<Patient> PatientList;
        private Doctor(String user_id, String user_password)
        {
            ID = user_id;
            password = user_password;
            client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(3)
            };

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
            String image = Convert.ToBase64String(imageBytes);
            //demo id
            Scan scan = new Scan() { patientID = "12345678910", date = "1970-01-01 10:00:00", image = image };
            string jsonScan = JsonConvert.SerializeObject(scan);
            try
            {

                // HttpRequestMessage request = new HttpRequestMessage
                // {
                //     Method = HttpMethod.Get,
                //     RequestUri = new Uri("http://ec2-user@ec2-184-169-147-75.us-west-1.compute.amazonaws.com:5000/analyze"),
                //     Content = new StringContent(jsonScan)
                // };

                // HttpResponseMessage response = await client.SendAsync(request);
                var RequestUri = new Uri("http://ec2-user@ec2-184-169-147-75.us-west-1.compute.amazonaws.com:5000/analyze");

                var Content = new StringContent(jsonScan);
                Console.WriteLine("Sending request");
                HttpResponseMessage response = await client.PostAsync(RequestUri, Content);

                Console.WriteLine("Received request!");
                string score = await response.Content.ReadAsStringAsync();
                Console.WriteLine(score);
                return score;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                Console.WriteLine("Examine timeout");
            }
            return "request not finished";
        }

        public void LookUpRecentScore(String patientID)
        {

            //Scan scan = new Scan() { patientID = "12345678910", date = "1970-01-01 10:00:00", image = image };
            //string jsonScan = JsonConvert.SerializeObject(scan);
            //var client = new HttpClient();
            //HttpResponseMessage response = await client.PostAsync(new Uri("http://ec2-user@ec2-184-169-147-75.us-west-1.compute.amazonaws.com:5000/analyze"), new StringContent(jsonScan));
        }

    }


    //https://stackoverflow.com/questions/44370046/how-do-i-serialize-object-to-json-using-json-net-which-contains-an-image-propert
    public class ImageConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var base64 = (string)reader.Value;
            // convert base64 to byte array, put that into memory stream and feed to image
            return ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(base64)));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var imageStream = (Stream)value;
            // save to memory stream in original format
            var ms = new MemoryStream();
            imageStream.CopyTo(ms);
            byte[] imageBytes = ms.ToArray();
            // write byte array, will be converted to base64 by JSON.NET
            writer.WriteValue(imageBytes);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Stream);
        }
    }
}