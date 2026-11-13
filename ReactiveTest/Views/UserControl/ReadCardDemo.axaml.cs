using ReactiveUI.Avalonia;
using ReactiveTest.ViewModels;

namespace ReactiveTest.Views.UserControl;

public partial class ReadCardDemo : ReactiveUserControl<ReadCardDemoViewModel>
{
    public ReadCardDemo()
    {
        InitializeComponent();
    }
}
