using System;
using VelocityMail.Service;

namespace Samples.Attachments
{
    /// <summary>
    /// Demonstration of using attachments.
    /// </summary>
    /// <remarks>Run Smtp4Dev (https://github.com/rnwood/smtp4dev) on port 2525 to inspect the
    /// e-mails sent</remarks>
    class Program
    {
        static void Main(string[] args)
        {
            var mailService = new VelocityMailService(new VelocityMailOptions
            {
                TemplatesAssembly = "Samples.Attachments.EmailTemplates, Samples.Attachments"
            });

            using (var msg = mailService.CreateMailMessage("SimpleTemplate", "testfrom@test.com", "testto@test.com"))
            {
                msg.Subject = "Today's ($today.ToShortDateString()) message";
                msg.Put("name", "Jonathan");
                msg.Put("today", DateTime.Today);

                msg.AttachFile("Test.pdf", "application/pdf");
                mailService.Send(msg);
            }
        }
    }
}
