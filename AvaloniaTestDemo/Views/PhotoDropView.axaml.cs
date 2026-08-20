using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Media.Imaging;

namespace AvaloniaTestDemo.Views;

public partial class PhotoDropView : UserControl
{
    public PhotoDropView()
    {
        InitializeComponent();
    }

    private void DropZone_DragOver(object? sender, DragEventArgs e)
    {
        var files = e.DataTransfer.TryGetFiles();
        e.DragEffects = (files != null && files.Length > 0) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private async void DropZone_Drop(object? sender, DragEventArgs e)
    {
        var files = e.DataTransfer.TryGetFiles();
        if (files == null || files.Length == 0)
            return;

        // TryGetFiles 返回的是 IStorageItem
        // 这里只接受文件，不接受文件夹
        var file = files.OfType<IStorageFile>().FirstOrDefault();

        if (file == null)
            return;

        string ext = Path.GetExtension(file.Name).ToLowerInvariant();

        string[] imageExts = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".webp" };

        if (!imageExts.Contains(ext))
            return;

        await using var stream = await file.OpenReadAsync();

        var bitmap = new Bitmap(stream);

        if (DataContext is PhotoDropViewModel vm)
        {
            vm.PreviewImage = bitmap;
        }
    }
}