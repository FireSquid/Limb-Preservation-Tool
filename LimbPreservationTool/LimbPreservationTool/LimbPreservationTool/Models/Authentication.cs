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
            Uri requestUri = TClient.GenURI("/auth");

            var httpContent = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    username,
                    password
                }),
                System.Text.Encoding.UTF8,
                "application/json"
                );

            HttpResponseMessage response;
            try
            {
                response = await TClient.client.PostAsync(requestUri, httpContent);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return (await response.Content.ReadAsStringAsync()).Equals("LOGIN_VALID");
        }

    }
}
