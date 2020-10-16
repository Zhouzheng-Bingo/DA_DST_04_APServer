using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpListener
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建HTTP监听
            using (var httpListener = new HttpListener())
            {
                //监听的路径
                httpListener.Prefixes.Add("http://localhost:8820/");
                //设置匿名访问
                httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                //开始监听
                httpListener.Start();
                Console.WriteLine("监听端口:8820...");
                while (true)
                {
                    //等待传入的请求接受到请求时返回，它将阻塞线程，直到请求到达
                    var context = httpListener.GetContext();
                    //取得请求的对象
                    HttpListenerRequest request = context.Request;
                    Console.WriteLine("{0} {1} HTTP/1.1", request.HttpMethod, request.RawUrl);
                    var reader = new StreamReader(request.InputStream);
                    var msg = reader.ReadToEnd();//读取传过来的信息

                    //Console.WriteLine("Accept: {0}", string.Join(",", request.AcceptTypes));
                    //Console.WriteLine("Accept-Language: {0}",
                    //    string.Join(",", request.UserLanguages));
                    Console.WriteLine("User-Agent: {0}", request.UserAgent);
                    Console.WriteLine("Accept-Encoding: {0}", request.Headers["Accept-Encoding"]);
                    Console.WriteLine("Connection: {0}",
                        request.KeepAlive ? "Keep-Alive" : "close");
                    Console.WriteLine("Host: {0}", request.UserHostName);
                    Console.WriteLine("Pragma: {0}", request.Headers["Pragma"]);

                    // 取得回应对象
                    HttpListenerResponse response = context.Response;

                    // 设置回应头部内容，长度，编码
                    response.ContentEncoding = Encoding.UTF8;
                    //response.ContentType = "text/plain;charset=utf-8";

                    //var path = @"C:\Users\wyl\Desktop\cese\";
                    ////访问的文件名
                    //var fileName = request.Url.LocalPath;

                    //读取文件内容
                    //var buff = File.ReadAllBytes(path + fileName);
                    //response.ContentLength64 = buff.Length;

                    //-------------------------
                    //byte[] data = new byte[1] { 1 };
                    //StringWriter sw = new StringWriter();
                    //XmlSerializer xm = new XmlSerializer(data.GetType());
                    //xm.Serialize(sw, data);
                    //var buff = Encoding.UTF8.GetBytes(sw.ToString());
                    //------------------------

                    //-------------------------------
                    //构造soap请求信息
                    //response.ContentType = "text/xml; charset=utf-8";
                    //StringBuilder soap = new StringBuilder();
                    //soap.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    //soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
                    //soap.Append("<soap:Body>");
                    //soap.Append("<GetIPCountryAndLocal xmlns=\"http://tempuri.org/\">");
                    //soap.Append("<RequestIP>183.39.119.90</RequestIP>");
                    //soap.Append("</GetIPCountryAndLocal>");
                    //soap.Append("</soap:Body>");
                    //soap.Append("</soap:Envelope>");
                    //byte[] buff = Encoding.UTF8.GetBytes(soap.ToString());
                    //-----------------------------------

                    response.ContentType = "text/xml; charset=utf-8";
                    string responseString = string.Format("<HTML><BODY> {0}</BODY></HTML>", DateTime.Now);
                    byte[] buff = Encoding.UTF8.GetBytes(responseString);

                    // 输出回应内容
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buff, 0, buff.Length);
                    // 必须关闭输出流
                    output.Close();
                }
            }

        }
    }
}
