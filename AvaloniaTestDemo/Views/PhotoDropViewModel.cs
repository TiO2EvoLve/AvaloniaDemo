using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class PhotoDropViewModel : DemoPageBase
{
    public PhotoDropViewModel() : base("图片拖拽", MaterialIconKind.Fingerprint, int.MinValue) { }

    [ObservableProperty]
    private Bitmap? previewImage;

    public bool HasImage => PreviewImage != null;

    partial void OnPreviewImageChanged(Bitmap? value)
    {
        OnPropertyChanged(nameof(HasImage));
    }
}