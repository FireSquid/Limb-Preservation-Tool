using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LimbPreservationTool.Models
{
    internal class Client
    {
        private static readonly string SERVER_URL = "http://ec2-184-169-147-75.us-west-1.compute.amazonaws.com:5000";

        internal static Uri GenURI(string urlExtension)
        {
            return new Uri(SERVER_URL + urlExtension);
        }

        internal static readonly HttpClient client = new HttpClient();
    }
}
