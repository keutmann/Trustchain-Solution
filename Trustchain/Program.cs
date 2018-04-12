using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Trustchain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Renaming BuildWebHost to InitWebHost avoids problems with add-migration command.
            // IDesignTimeDbContextFactory implemented for add-migration specifically.
            InitWebHost(args).Run();
        }

        public static IWebHost InitWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, 80);
                    options.Listen(IPAddress.Any, 443, listenOptions =>
                    {
                        var password = File.ReadAllText(@"C:\tmp\certpassword.txt");
                        listenOptions.UseHttps(@"C:\tmp\www_trust_dance.pfx", password);
                    });
                })
                .Build();
    }
}
