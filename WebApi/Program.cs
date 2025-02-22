using System;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using BusinessEntities;
using Data;

namespace WebApi
{
    public class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:9000/";

            try
            {
                using (WebApp.Start<Startup>(url: baseAddress))
                {
                    Console.WriteLine($"Server running at {baseAddress}");
                    Console.WriteLine("Press Ctrl+C to quit.");

                    // Keep the app running
                    var exitEvent = new System.Threading.ManualResetEvent(false);
                    Console.CancelKeyPress += (sender, e) =>
                    {
                        e.Cancel = true;
                        exitEvent.Set();
                    };
                    exitEvent.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
