using FactoryFactory.AspNet.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SampleWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                /*
                 * This is how you swap out Microsoft.Extensions.DependencyInjection
                 * for FactoryFactory. Just one line of code.
                 */
                .UseFactoryFactory()
                /*
                 * This is what was put into the project by the template to
                 * start off with.
                 */
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
