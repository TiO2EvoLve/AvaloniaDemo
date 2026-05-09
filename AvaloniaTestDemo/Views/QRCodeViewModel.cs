using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using QRCoder;
using SukiUI.Controls;
using SukiUI.MessageBox;
using SukiUI.Toasts;
using Tmds.DBus.Protocol;

namespace AvaloniaTestDemo.Views;

// 使用常规构造函数并调用基类构造以避免语法错误
public partial class QRCodeViewModel(ISukiToastManager toastManager) : DemoPageBase("QRCode", MaterialIconKind.Qrcode, int.MinValue)
{
    // 将要传递给基类的显示名与图标，通过构造函数调用基类构造器

    // 使用 source-generator 自动生成属性并触发通知
    [ObservableProperty]
    private Bitmap? image;

    [ObservableProperty]
    private string text = "www.baidu.com";

    // 保存最近生成的 PNG 字节，这样保存文件时可以直接写入字节流
    private byte[]? _lastPngBytes;

    // 生成二维码并设置 Image 属性
    [RelayCommand]
    private void SpawnQRCode()
    {
        try
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(Text, QRCodeGenerator.ECCLevel.Q);
            using var pngQr = new PngByteQRCode(qrCodeData);

            // 获取 PNG 格式的字节数组（可直接写入文件）
            var pngBytes = pngQr.GetGraphic(20);
            _lastPngBytes = pngBytes;

            // 从字节数组创建 Avalonia Bitmap
            using var ms = new MemoryStream(pngBytes);
            ms.Position = 0;
            Image = new Bitmap(ms);
        }
        catch (Exception ex)
        {
            // 如果需要，可以记录或显示错误；这里只是吞掉异常以防止 UI 崩溃
            System.Diagnostics.Debug.WriteLine($"SpawnQRCode error: {ex}");
        }
    }

    // 将最近生成的二维码保存为文件（保存到用户的图片目录下的 qrcode.png）
    [RelayCommand]
    private async Task Save()
    {
        try
        {
            if (_lastPngBytes == null || _lastPngBytes.Length == 0)
                return; // 没有可保存的数据
            var pictures = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = Path.Combine(pictures, "qrcode.png");
            await File.WriteAllBytesAsync(path, _lastPngBytes);
            ShowTypeDemoToast(NotificationType.Success);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Save QRCode error: {ex}");
        }
    }
    private void ShowTypeDemoToast(NotificationType toastType)
    {
        toastManager.CreateToast()
            .WithTitle("保存成功")
            .WithContent(
                $"图片已保存到桌面.")
            .OfType(toastType)
            .Dismiss().After(TimeSpan.FromSeconds(1))
            .Dismiss().ByClicking()
            .Queue();
    }
}

