using System;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using LanguageExt;
using static LanguageExt.Prelude;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class TestViewModel() : DemoPageBase("Test", MaterialIconKind.TestTube,100)
{

    [RelayCommand]
    private void Test()
    {
        var pr = from desktop in Try(() => Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
            where desktop.Contains("d") 
            select desktop;

        pr.Match(
            Succ: Console.WriteLine, 
            Fail: _ => { }                     
        );
    }
}