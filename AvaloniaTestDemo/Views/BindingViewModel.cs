using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using ReactiveUI;

namespace AvaloniaTestDemo.Views;

public partial class BindingViewModel :  DemoPageBase
{
    public Dictionary<string, bool> States { get; } = new();
    
    public BindingViewModel() : base("Binding", MaterialIconKind.LinkVariant)
    {
        States.Add("IsCheck", false);
        States.Add("IsSelect", false);
    }
    [RelayCommand]
    private void ClickButton()
    {
        States["IsCheck"] = true;
    }
}
