﻿using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net;
using Serilog;
using Microsoft.Extensions.Logging;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Display;

namespace Trustchain
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static int Main(string[] args)
        {
            var formatter = (false) ? (ITextFormatter)new RenderedCompactJsonFormatter() : new MessageTemplateTextFormatter("{Timestamp:o} {RequestId,13} [{Level:u3}] {Message} ({EventId:x8}){NewLine}{Exception}", null);

            var pathFormat = "Logs/log-{Date}.txt";
            const long DefaultFileSizeLimitBytes = 1024 * 1024 * 1024;
            const int DefaultRetainedFileCountLimit = 31;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Async(w => w.RollingFile(
                    formatter,
                    Environment.ExpandEnvironmentVariables(pathFormat),
                    fileSizeLimitBytes: DefaultFileSizeLimitBytes,
                    retainedFileCountLimit: DefaultRetainedFileCountLimit,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(2)))
                .CreateLogger();

            try
            {
                Log.Information("Getting the motors running...");

                // Renaming BuildWebHost to InitWebHost avoids problems with add-migration command.
                // IDesignTimeDbContextFactory implemented for add-migration specifically.
                InitWebHost(args).Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IWebHost InitWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseConfiguration(Configuration)
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
                .UseSerilog()
                .Build();
    }
}
