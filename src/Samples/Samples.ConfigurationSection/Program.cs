using System;
using VelocityMail.Service;

namespace Samples.ConfigurationSection
{
    /// <summary>
    /// Sample using a configuration section in App.config. This example also demonstrates address
    /// re-rewriting.
    /// </summary>
    /// <remarks>Run Smtp4Dev (https://github.com/rnwood/smtp4dev) on port 2525 to inspect the
    /// e-mails sent</remarks>
    class Program
    {
        static void Main(string[] args)
        {
            // Create with configuration from App.config
            var mailService = new VelocityMailService();

            using (var msg = mailService.CreateMailMessage("SimpleTemplate"))
            {
                msg.To.Add("jon@bar.com");
                msg.To.Add("max@foo.com");
                msg.Subject = "Today's ($today.ToShortDateString()) message";
                msg.Put("name", "Jonathan");
                msg.Put("today", DateTime.Today);

                mailService.Send(msg);
            }
        }
    }
}
