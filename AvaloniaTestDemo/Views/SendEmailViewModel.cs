using System;
using System.Net;
using System.Net.Mail;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using SukiUI.Toasts;

namespace AvaloniaTestDemo.Views;

public partial class SendEmailViewModel (ISukiToastManager toastManager) : DemoPageBase("Email", MaterialIconKind.Email, int.MinValue)
{

    public string fromEmail { get; set; } = "3290158038@qq.com";
    public string password { get; set; } = "uyfvzrlrtxdqcjhh";
    public string toEmail { get; set; } = "3290158038@qq.com";
    public string subject { get; set; } = "你好";
    public string body { get;set; } = "你好";
    

    private void SendEmail(string fromEmail, string password, string toEmail, string subject, string body)
    {
        try
        {
            // 创建一个MailMessage对象
            var mail = new MailMessage();

            // 设置发件人地址
            mail.From = new MailAddress(fromEmail);

            // 设置收件人地址
            mail.To.Add(toEmail);

            // 设置邮件主题
            mail.Subject = subject;

            // 设置邮件正文
            mail.Body = body;

            // 创建一个SmtpClient对象，用于发送邮件
            var smtpClient = new SmtpClient("smtp.qq.com", 587); // 替换为实际的SMTP服务器和端口

            // 设置SMTP客户端的认证信息
            smtpClient.Credentials = new NetworkCredential(fromEmail, password);

            // 启用SSL加密
            smtpClient.EnableSsl = true;

            // 发送邮件
            smtpClient.Send(mail);

            // 邮件发送成功
            ShowTypeDemoToast("邮件发送成功：",NotificationType.Success);
        }
        catch (Exception ex)
        {
            // 处理发送邮件时的异常
            ShowTypeDemoToast("邮件发送失败：",NotificationType.Error);
        }
    }
    
    [RelayCommand]
    private void SendEmail_Click()
    {
        SendEmail(fromEmail, password, toEmail, subject, body);
    }
    
    private void ShowTypeDemoToast(string msg,NotificationType toastType)
    {
        toastManager.CreateToast()
            .WithTitle("执行完毕！")
            .WithContent(msg)
            .OfType(toastType)
            .Dismiss().After(TimeSpan.FromSeconds(1))
            .Dismiss().ByClicking()
            .Queue();
    }
}