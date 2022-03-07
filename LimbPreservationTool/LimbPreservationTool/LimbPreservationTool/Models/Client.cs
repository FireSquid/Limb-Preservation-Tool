using System;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace LimbPreservationTool.Models
{
    public abstract class SplittableHttpContent
    {

        abstract public List<HttpRequestMessage> ToHttpList();

        static protected IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize + 1)
                .Select(i => str.Substring(i * chunkSize, Math.Min(chunkSize, str.Length - i * chunkSize)));
        }
    }
    public interface IFetchableHttpContent
    {
        HttpRequestMessage Fetch();
    }
    public class Client
    {
        private static HttpClient hc = null;

        private static Client c = null;
        private Client()
        {
            hc = new HttpClient();
            hc.Timeout = TimeSpan.FromSeconds(120);
        }
        public static Client GetInstance()
        {
            if (c == null)
            {
                c = new Client();
            }
            return c;
        }
        async public Task SendRequestChunksAsync(SplittableHttpContent s)
        {
            if (hc == null)
            {
                Console.WriteLine("hc is null");
            }
            List<HttpRequestMessage> chunks = s.ToHttpList();

            Console.WriteLine(chunks.Count);
            foreach (HttpRequestMessage c in chunks)
            {

                //request.Content.
                //request.Headers.
                try
                {
                    HttpResponseMessage r = await hc.SendAsync(c);

                    //string score = await r.Content.ReadAsStringAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }

            }

        }
        async public Task<HttpResponseMessage> GetRequestAsync(IFetchableHttpContent f)
        {
            return await hc.SendAsync(f.Fetch());

        }
        //async public HttpResponseMessage Retrieve

    }
}
