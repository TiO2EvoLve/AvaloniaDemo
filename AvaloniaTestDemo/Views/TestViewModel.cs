using System;

using CommunityToolkit.Mvvm.Input;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class TestViewModel() : DemoPageBase("Test", MaterialIconKind.TestTube,100)
{

    [RelayCommand]
    private void Test()
    {
        
    }
}