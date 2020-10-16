using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        static Object o = new object();
        static void Main(string[] args)
        {
            HttpListener listerner = new HttpListener();
            Address addr = new Address();
            while (true)
            {
                try
                {
                    listerner.AuthenticationSchemes = AuthenticationSchemes.Anonymous;//指定身份验证 Anonymous匿名访问
                    string url = "http://192.168.0.100:8820/";
                    listerner.Prefixes.Add(url);
                    addr.AddAddress("http://192.168.0.100:8820/");
                    listerner.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("服务启动失败...");
                    break;
                }
                Console.WriteLine("服务器启动成功.......");

                //线程池
                int minThreadNum;
                int portThreadNum;
                int maxThreadNum;
                ThreadPool.GetMaxThreads(out maxThreadNum, out portThreadNum);
                ThreadPool.GetMinThreads(out minThreadNum, out portThreadNum);
                Console.WriteLine("最大线程数：{0}", maxThreadNum);
                Console.WriteLine("最小空闲线程数：{0}", minThreadNum);
                //ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc1), x);

                Console.WriteLine("\n\n等待客户连接中。。。。");
                while (true)
                {
                    //等待请求连接
                    //没有请求则GetContext处于阻塞状态
                    HttpListenerContext ctx = listerner.GetContext();

                    ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc), ctx);
                }
                //listerner.Stop();
            }
            Console.ReadKey();
        }

        static void TaskProc(object o)
        {
            HttpListenerContext ctx = (HttpListenerContext)o;

            ctx.Response.StatusCode = 200;//设置返回给客服端http状态代码

            ////接收Get参数
            //string type = ctx.Request.QueryString["type"];
            //string userId = ctx.Request.QueryString["userId"];
            //string password = ctx.Request.QueryString["password"];
            //string filename = Path.GetFileName(ctx.Request.RawUrl);
            //string userName = HttpUtility.ParseQueryString(filename).Get("userName");//避免中文乱码
            ////进行处理
            //Console.WriteLine("收到数据:" + userName);

            //接收POST参数
            Stream stream = ctx.Request.InputStream;
            System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
            String body = reader.ReadToEnd();
            Console.WriteLine("收到POST数据:" + HttpUtility.UrlDecode(body));
            Console.WriteLine("解析Data:" + HttpUtility.ParseQueryString(body).Get("data"));
            string data = HttpUtility.ParseQueryString(body).Get("data");
            JArray jo = (JArray)JsonConvert.DeserializeObject(data);

            string equip_ip = jo[0]["equip_ip"].ToString();
            string nc_type = jo[0]["nc_type"].ToString();
            string cnc_software = jo[0]["cnc_software"].ToString();
            string machineToolProbe = jo[0]["machineToolProbe"].ToString();
            string run_mode = jo[0]["run_mode"].ToString();
            string nc_state = jo[0]["nc_state"].ToString();
            string spl_speed = jo[0]["spl_speed"].ToString();
            string spl_override = jo[0]["spl_override"].ToString();
            string sql_programmed = jo[0]["sql_programmed"].ToString();


            //使用Writer输出http响应代码,UTF8格式
            using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream, Encoding.UTF8))
            {
                writer.Write("处理结果,Hello world<br/>");
                //writer.Write("数据是userId={0},userName={1}", userId, userName);
                writer.Close();
                ctx.Response.Close();
            }
        }

    }
}
