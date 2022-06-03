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

        static protected List<byte[]> Split(byte[] str, int chunkSize)
        {

            List<byte[]> l = new List<byte[]>();
            int max = str.Length;
            for (int i = 0; i < max; i += chunkSize)
            {
                l.Add(new ArraySegment<byte>(str, i, Math.Min(chunkSize, max - i)).ToArray<byte>());
                // l.Add(str.Skip<byte>(i).Take<byte>(Math.Min(chunkSize, max - i)).ToArray());
            }
            //return Enumerable.Range(0, str.Length / chunkSize + 1)
            //    .Select(i => Take<byte>(str, new Range(i * chunkSize, )));
            return l;
        }
        //public static T[] CopySlice<T>(this T[] source, int index, int length, bool padToLength = false)
        //{
        //    int n = length;
        //    T[] slice = null;

        //    if (source.Length < index + length)
        //    {
        //        n = source.Length - index;
        //        if (padToLength)
        //        {
        //            slice = new T[length];
        //        }
        //    }

        //    if (slice == null) slice = new T[n];
        //    Array.Copy(source, index, slice, 0, n);
        //    return slice;
        //}

        //public static IEnumerable<T[]> Slices<T>(this T[] source, int count, bool padToLength = false)
        //{
        //    for (var i = 0; i < source.Length; i += count)
        //        yield return source.CopySlice(i, count, padToLength);
        //}
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
            //hc = new HttpClient();
            //hc.Timeout = TimeSpan.FromSeconds(120);
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
            if (TClient.client == null)
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
                    HttpResponseMessage r = await TClient.client.SendAsync(c);

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
            return await TClient.client.SendAsync(f.Fetch());

        }
        //async public HttpResponseMessage Retrieve
    }

    internal class TClient
    {
        private static readonly string SERVER_URL = "http://ec2-184-169-147-75.us-west-1.compute.amazonaws.com:5000";

        //private static readonly string SERVER_URL = "http://miwpro.local:5000";

        // private static readonly string SERVER_URL = 

        internal static Uri GenURI(string urlExtension)
        {
            return new Uri(SERVER_URL + urlExtension);
        }

        internal static readonly HttpClient client = new HttpClient();
    }
}
