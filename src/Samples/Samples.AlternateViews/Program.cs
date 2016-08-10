using System;
using VelocityMail.Service;

namespace Samples.AlternateViews
{
    /// <summary>
    /// Demonstrates using alternate views (i.e. both a text body and an HTML body in the same e-mail). Mail
    /// clients display one or the other depending on their capabilities.
    /// </summary>
    /// <remarks>Run Smtp4Dev (https://github.com/rnwood/smtp4dev) on port 2525 to inspect the
    /// e-mails sent</remarks>
    class Program
    {
        static void Main(string[] args)
        {
            var mailService = new VelocityMailService(new VelocityMailOptions
            {
                TemplatesAssembly = "Samples.AlternateViews.EmailTemplates, Samples.AlternateViews"
            });

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
