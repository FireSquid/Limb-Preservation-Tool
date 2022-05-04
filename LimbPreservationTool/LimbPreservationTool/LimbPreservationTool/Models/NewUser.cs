using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LimbPreservationTool.Models
{
    public class NewUser
    {
        public static async Task<bool> AttemptCreation(string username, string password)
        {
            Uri requestUri = TClient.GenURI("/new_user");

            var httpContent = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    username,
                    password
                }),
                System.Text.Encoding.UTF8,
                "application/json"
                );

            HttpResponseMessage response = await TClient.client.PostAsync(requestUri, httpContent);

            return (await response.Content.ReadAsStringAsync()).Equals("USER_CREATED");
        }
    }
}
