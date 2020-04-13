using FactoryFactory.AspNet.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SampleWebApp.NetCore22
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                /*
                 * This is how you swap out Microsoft.Extensions.DependencyInjection
                 * for FactoryFactory. Just one line of code.
                 */
                .UseFactoryFactory()
                /*
                 * This is what was put into the project by the template to
                 * start off with.
                 */
                .UseStartup<Startup>();
    }
}
