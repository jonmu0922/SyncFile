using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace SyncFile.Infrastructure.Utility
{
    public class HttpHelper
    {
        /// <summary>
        /// http get
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                byte[] bResponse = client.DownloadData(url);

                return Encoding.UTF8.GetString(bResponse);
            }
        }

        /// <summary>
        /// http get (return byte)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] HttpGetByte(string url)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                return client.DownloadData(url);                
            }
        }

        /// <summary>
        /// http post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public static string HttpPost(string url, Dictionary<string,string> para)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                NameValueCollection nc = new NameValueCollection();

                foreach(var p in para)
                    nc[p.Key] = p.Value;

                byte[] bResponse = client.UploadValues(url, nc);
                
                return Encoding.UTF8.GetString(bResponse);
            }
        }

        public static string HttpPost(string url, Dictionary<string, string> para, string name, byte[] file)
        {
            // source from
            // https://stackoverflow.com/questions/18535799/send-fileparameters-in-post-request

            string result = "";
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);

            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            Stream rs = wr.GetRequestStream();
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

            foreach (var p in para)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, p.Key, para[p.Key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }           

            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "file", name, "text/plain");
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            MemoryStream msstream = new MemoryStream(file);

            byte[] buffer = new byte[4096];
            int bytesRead = 0;

            while ((bytesRead = msstream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);              
            }

            msstream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();
            WebResponse wresp = null;

            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                result = reader2.ReadToEnd();
            }
            catch (Exception)
            {                
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

            return result;
        }    

        public static string HttpDelete(string url, Dictionary<string, string> para)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                NameValueCollection nc = new NameValueCollection();

                foreach (var p in para)
                    nc[p.Key] = p.Value;

                byte[] bResponse = client.UploadValues(url, "DELETE", nc);

                return Encoding.UTF8.GetString(bResponse);
            }
        }
    }
}
