using ReactiveUI.Avalonia;
using ReactiveTest.ViewModels;

namespace ReactiveTest.Views.UserControl;

public partial class Test : ReactiveUserControl<TestViewModel>
{
    public Test()
    {
        InitializeComponent();
    }
}
