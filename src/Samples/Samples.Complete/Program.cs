using System;
using System.Globalization;
using System.Threading;
using Samples.Complete.Models;
using VelocityMail.Service;

namespace Samples.Complete
{
    /// <summary>
    /// Complete example demonstrating configuration of the service from App.config, localisation, alternative
    /// views along with some more real-world use of Velocity templates.
    /// </summary>
    /// <remarks>Run Smtp4Dev (https://github.com/rnwood/smtp4dev) on port 2525 to inspect the
    /// e-mails sent</remarks>
    class Program
    {
        static void Main(string[] args)
        {
            var mailService = new VelocityMailService();
            var order = CreateSampleOrder();

            using (var msg = mailService.CreateMailMessage("OrderReceipt", "testfrom@test.com", "testto@test.com"))
            {
                msg.Subject = "Order Confirmation: $lang";
                msg.Put("name", "Jonathan");
                msg.Put("order", order);
                msg.Put("currencyFormatter", new CultureInfo("fr-FR")); // Euros
                msg.Put("st", new StringTools());

                // Send both the German and English versions. In normal use you would just
                // send the e-mail and the correct version would be sent based on the
                // current culture settings.
                RunAsUiCulture("de-DE", () =>
                {
                    msg.Put("lang", "de-DE");
                    mailService.Send(msg);
                });

                RunAsUiCulture("en-GB", () =>
                {
                    msg.Put("lang", "en-GB");
                    mailService.Send(msg);
                });
            }
        }

        /// <summary>
        /// Runs the action method with the specified UICulture.
        /// </summary>
        /// <param name="langCode">Desired language</param>
        /// <param name="action">Action to run with the specified language</param>
        static void RunAsUiCulture(string langCode, Action action)
        {
            var oldUiCulture = Thread.CurrentThread.CurrentUICulture;
            var oldCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                var newUiCulture = new CultureInfo(langCode);
                Thread.CurrentThread.CurrentUICulture = newUiCulture;
                Thread.CurrentThread.CurrentCulture = newUiCulture;
                action();
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = oldUiCulture;
                Thread.CurrentThread.CurrentCulture = oldCulture;
            }
        }

        static Order CreateSampleOrder()
        {
            var address = new Address
            {
                Name = "Home",
                Line1 = "2205 Meadow Road",
                Line2 = "Little Vale",
                Town = "Oxford",
                County = "Oxfordshire",
                PostalCode = "OX99 12Z"
            };

            var order = new Order
            {
                Id = "SO12345678",
                OrderDate = DateTime.Now,
                Address = address,
                AmountPaidByCard = 7.95m,
                TransactionCode = "TC12345678"
            };

            order.OrderItems.Add(new OrderItem
            {
                ProductCode = "NZJ12345",
                Description = "Dishwasher upper arm",
                Quantity = 1,
                Price = 3.75m,
                Delivery = "3 Days",
                PaymentType = PaymentType.Normal
            });

            order.OrderItems.Add(new OrderItem
            {
                ProductCode = "RMJ12345",
                Description = "Renault Scenic MK2 boot strut",
                Quantity = 2,
                Price = 18.55m,
                Delivery = "3 Days",
                PaymentType = PaymentType.Normal
            });

            order.OrderItems.Add(new OrderItem
            {
                ProductCode = "ZZJ12345",
                Description = "Replacement battery pack",
                Quantity = 1,
                Price = 7.95m,
                Delivery = "5 Days",
                PaymentType = PaymentType.PersonalCard
            });

            order.OrderItems.Add(new OrderItem
            {
                ProductCode = "YZW992375",
                Description = "Screem cleaner & cloth",
                Quantity = 5,
                Price = 2.95m,
                Delivery = "3 Days",
                PaymentType = PaymentType.PersonalCard
            });

            return order;
        }
    }
}
