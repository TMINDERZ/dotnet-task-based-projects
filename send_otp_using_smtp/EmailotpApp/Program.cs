using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace GmailOtpApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Enter your email address: ");
            string toEmail = Console.ReadLine();

            // Step 1: Generate random OTP
            string otp = GenerateRandomOtp();
            Console.WriteLine($"Generated OTP: {otp}");

            // Step 2: Send via Gmail
            await SendOtpEmailAsync(toEmail, otp);

            // Step 3: Verify OTP (Optional)
            Console.Write("\nEnter the OTP you received: ");
            string enteredOtp = Console.ReadLine();

            if (enteredOtp == otp)
                Console.WriteLine("✅ OTP Verified Successfully!");
            else
                Console.WriteLine("❌ Invalid OTP. Please try again.");
        }

        // 🔸 Random secure OTP generator (6 digits)
        public static string GenerateRandomOtp(int length = 6)
        {
            byte[] bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);
            string otp = "";

            foreach (byte b in bytes)
                otp += (b % 10).ToString();

            return otp;
        }

        // 🔸 Send email using Gmail SMTP
        public static async Task SendOtpEmailAsync(string toEmail, string otp)
        {
            string fromEmail = "miniduheptagon@gmail.com";
            string fromPassword = "ifcidgnltnrkajxs"; // from Gmail App Password setup

            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, "MyApp Verification"),
                Subject = "Your One-Time Password (OTP)",
                Body = $"Your OTP is: {otp}\nIt will expire in 5 minutes.",
                IsBodyHtml = false
            };
            mail.To.Add(toEmail);

            await smtp.SendMailAsync(mail);
            Console.WriteLine("\n📧 OTP sent successfully using Gmail!");
        }
    }
}
