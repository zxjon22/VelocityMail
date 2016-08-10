using System;
using VelocityMail.Service;

namespace Samples.SimpleService
{
    /// <summary>
    /// Simplest use case using the VelocityMailService.
    /// Configuration is via the configuration section in App.Config
    /// </summary>
    /// <remarks>Run Smtp4Dev (https://github.com/rnwood/smtp4dev) on port 2525 to inspect the
    /// e-mails sent</remarks>
    class Program
    {
        static void Main(string[] args)
        {
            // Create with configuration from App.config
            var mailService = new VelocityMailService();

            using (var msg = mailService.CreateMailMessage("SimpleTemplate", "testfrom@test.com", "testto@test.com"))
            {
                msg.Subject = "Today's ($today.ToShortDateString()) message";
                msg.Put("name", "Jonathan");
                msg.Put("today", DateTime.Today);

                mailService.Send(msg);
            }
        }
    }
}
