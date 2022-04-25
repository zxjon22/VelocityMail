using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using VelocityMail.Service;

namespace Samples.NetCore
{
    /// <summary>
    /// Simplest use case using the VelocityMailService under .Net Core.
    /// <see cref="System.Net.Mail.SmtpClient"/> does not automatically load its default configuration
    /// from appsettings.json, unlike its .NET Framework counterpart with App.config.
    /// </summary>
    /// <remarks>Run Smtp4Dev (https://github.com/rnwood/smtp4dev) on port 2525 to inspect the
    /// e-mails sent</remarks>
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var services = new ServiceCollection();

            //----

            services.Configure<VelocityMailOptions>(opts =>
            {
                // System doesn't support external SmtpClient configuration in .Net Core.
                var clientOptions = new SmtpClientOptions();
                configuration.Bind("Smtp", clientOptions);

                configuration.Bind("VelocityMail", opts);
                opts.SmtpClientFactory = () => clientOptions.CreateSmtpClient();
            })
            .AddSingleton<VelocityMailService>(ctx =>
            {
                var opts = ctx.GetRequiredService<IOptions<VelocityMailOptions>>();
                return new VelocityMailService(opts.Value);
            });

            //----

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();

            var mailService = scope.ServiceProvider.GetRequiredService<VelocityMailService>();

            using var msg = mailService.CreateMailMessage("SimpleTemplate", "testfrom@test.com", "testto@test.com");
            msg.Subject = "Today's ($today.ToShortDateString()) message";
            msg.Put("name", "Jonathan");
            msg.Put("today", DateTime.Today);

            mailService.Send(msg);
        }
    }
}
