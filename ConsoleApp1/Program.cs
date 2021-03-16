using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleApp1
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static System.Timers.Timer aTimer;
        static void Main(string[] args)
        {
            SetTimer(((60-DateTime.UtcNow.Minute)*60000)-(DateTime.UtcNow.Second*1000));

            Console.WriteLine("\nWrite quit and press Enter to exit the application...\n");
            Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
            while(true)
            {
                if(Console.ReadLine()=="quit")
                {
                    aTimer.Stop();
                    aTimer.Dispose();
                    break;
                }
            }

            Console.WriteLine("Terminating the application...");
        }
        private static async Task Caller()
        {
            try
            {
                string json = JsonConvert.SerializeObject("Start");
                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("http://192.168.1.198:44375/api/Server", httpContent);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
        private static void SetTimer(int interval)
        {
            aTimer = new System.Timers.Timer(interval); 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Caller();
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}", e.SignalTime);
            if (e.SignalTime.Minute == 0)
            {
                aTimer.Interval = 60 * 60000;
            }
            else
            {
                aTimer.Interval = ((60 - DateTime.UtcNow.Minute) * 60000) - (DateTime.UtcNow.Second * 1000);
            }
        }
    }
}
