using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

public class EmailService
{
    public async Task SendPasswordEmailAsync(string toEmail, string password)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("後台管理系統", "your@email.com"));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "InfinityCinema忘記密碼通知";
        message.Body = new TextPart("plain")
        {
            Text = $"您好，您的密碼是：{password}，建議您登入後盡速修改密碼"
        };

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync("a0985324280@gmail.com", "jrxyvloqcwczzsla");
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
