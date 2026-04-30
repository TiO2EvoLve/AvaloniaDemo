using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using ReactiveUI;

namespace AvaloniaTestDemo.Views;

public partial class BindingViewModel() :  DemoPageBase("Binding", MaterialIconKind.LinkVariant)
{
    [ObservableProperty] private bool isChecked;
    

}
