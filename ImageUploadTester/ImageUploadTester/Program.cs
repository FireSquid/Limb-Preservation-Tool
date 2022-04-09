using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageUploadTester
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            loadFile();
        }

        private static async Task loadFile()
        {
            OpenFileDialog fbd = new OpenFileDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(fbd.FileName); // full path
            }

            Console.WriteLine("-------------Examining");
            Console.WriteLine("-------------Created Stream");

            Scan scan = new Scan()
            {
                patientID = "123456789",
                date = DateTime.Now.ToString().Replace(' ', '_').Replace('/', '-'),
                imageStream = fbd.OpenFile()
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

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                Stream aStream;

                saveFileDialog.Filter = "png files (*.png)|*.png|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if ((aStream = saveFileDialog.OpenFile()) != null)
                    {
                        result.imageStream.CopyTo(aStream);

                        aStream.Close();
                    }
                }

                // OUTPUT AREA
                //result.imageStream;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

            Console.ReadLine();
        }
    }


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
                    Console.WriteLine(c.ToString());

                    HttpResponseMessage r = await TClient.client.SendAsync(c);

                    //string score = await r.Content.ReadAsStringAsync();
                    ;
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

    internal class TClient
    {
        private static readonly string SERVER_URL = "http://ec2-184-169-147-75.us-west-1.compute.amazonaws.com:5000";

        internal static Uri GenURI(string urlExtension)
        {
            return new Uri(SERVER_URL + urlExtension);
        }

        internal static readonly HttpClient client = new HttpClient();
    }
}
