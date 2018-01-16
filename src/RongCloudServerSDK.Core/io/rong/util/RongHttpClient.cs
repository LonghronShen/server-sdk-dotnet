using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using donet.io.rong.models;
using System.Net.Http;

#if NETCORE
using Mono.Web;
#else
using System.Web;
#endif

namespace donet.io.rong.util
{

    class RongHttpClient
    {

        public const int DefaultTimeOut = 30 * 1000;

        public static string ExecuteGet(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

#if NETCORE
            return ExecuteGetAsync(url).GetAwaiter().GetResult();
#else
            HttpWebRequest myRequest = WebRequest.Create(url) as HttpWebRequest;
            myRequest.Method = "GET";
            myRequest.ReadWriteTimeout = RongHttpClient.DefaultTimeOut;
            return ReturnResult(myRequest);
#endif
        }

        public static async Task<string> ExecuteGetAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            using (var client = new System.Net.Http.HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(RongHttpClient.DefaultTimeOut);
                return await client.GetStringAsync(url);
            }
        }

        public static string ExecutePost(string appkey, string appSecret, string methodUrl, string postStr, string contentType)
        {
#if NETCORE
            return ExecutePostAsync(appkey, appSecret, methodUrl,
                postStr, contentType).GetAwaiter().GetResult();
#else
            var signature = GenerateSignature(appSecret);

            if (contentType == null || contentType.Equals("") || contentType.Length < 10)
            {
                contentType = "application/x-www-form-urlencoded";
            }

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(methodUrl);
            myRequest.Method = "POST";
            myRequest.ContentType = contentType;

            myRequest.Headers.Add("App-Key", appkey);
            myRequest.Headers.Add("Nonce", signature.Item1);
            myRequest.Headers.Add("Timestamp", signature.Item2);
            myRequest.Headers.Add("Signature", signature.Item3);
            myRequest.ReadWriteTimeout = RongHttpClient.DefaultTimeOut;

            byte[] data = Encoding.UTF8.GetBytes(postStr);
            myRequest.ContentLength = data.Length;

            Stream newStream = myRequest.GetRequestStream();

            // Send the data.
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            return ReturnResult(myRequest);
#endif
        }

        public static async Task<string> ExecutePostAsync(string appkey, string appSecret, string methodUrl, string postStr, string contentType)
        {
            var signature = GenerateSignature(appSecret);

            if (contentType == null || contentType.Equals("") || contentType.Length < 10)
            {
                contentType = "application/x-www-form-urlencoded";
            }

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(RongHttpClient.DefaultTimeOut);
                client.DefaultRequestHeaders.TryAddWithoutValidation("App-Key", appkey);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Nonce", signature.Item1);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Timestamp", signature.Item2);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Signature", signature.Item3);

                var response = await client.PostAsync(methodUrl,
                    new StringContent(postStr, Encoding.UTF8, contentType));
                return await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式</returns>  
        public static int ConvertDateTimeInt(DateTime time)
        {
            var startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (int)(time - startTime).TotalSeconds;
        }

        public static string GetHash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

#if NETCORE
            using (var algorithm = SHA1.Create())
            {
                var size = algorithm.HashSize;
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hash).Replace("-", "");
            }
#else
            //建立SHA1对象
            SHA1 sha = new SHA1CryptoServiceProvider();

            //将mystr转换成byte[]
            UTF8Encoding enc = new UTF8Encoding();
            byte[] dataToHash = enc.GetBytes(input);

            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);

            //将运算结果转换成string
            string hash = BitConverter.ToString(dataHashed).Replace("-", "");

            return hash;
#endif
        }

        /// <summary>
        /// Certificate validation callback.
        /// </summary>
        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (error == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine("X509Certificate [{0}] Policy Error: '{1}'",
                cert.Subject,
                error.ToString());

            return false;
        }

        public static string ReturnResult(HttpWebRequest myRequest)
        {
            HttpWebResponse myResponse = null;
            int httpStatus = 200;
            string content;
            try
            {
#if NETCORE
                myResponse = (HttpWebResponse)myRequest.GetResponseAsync().GetAwaiter().GetResult();
#else
                myResponse = (HttpWebResponse)myRequest.GetResponse();
#endif
                httpStatus = (int)myResponse.StatusCode;
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);

                content = reader.ReadToEnd();
            }
            catch (WebException e)
            {
                //异常请求
                myResponse = (HttpWebResponse)e.Response;
                httpStatus = (int)myResponse.StatusCode;
                using (Stream errData = myResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(errData))
                    {
                        content = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
            return content;
        }

        public static Tuple<string, string, string> GenerateSignature(string appSecret)
        {
            var rd = new Random((int)DateTime.Now.Ticks);
            var rd_i = rd.Next();
            var nonce = Convert.ToString(rd_i);
            var timestamp = Convert.ToString(ConvertDateTimeInt(DateTime.UtcNow));
            var signature = GetHash(appSecret + nonce + timestamp);
            return Tuple.Create(nonce, timestamp, signature);
        }

    }

}