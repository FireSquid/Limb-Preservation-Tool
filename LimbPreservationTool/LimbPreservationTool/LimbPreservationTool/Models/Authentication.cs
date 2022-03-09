using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LimbPreservationTool.Models
{
    public class Authentication
    {

        public static async Task<bool> AttemptAuthentication(string username, string password)
        {
            Uri requestUri = Client.GenURI("/auth");

            var httpContent = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    username,
                    password
                }),
                System.Text.Encoding.UTF8,
                "application/json"
                );

            HttpResponseMessage response = await Client.client.PostAsync(requestUri, httpContent);

            return (await response.Content.ReadAsStringAsync()).Equals("LOGIN_VALID");
        }

    }
}
