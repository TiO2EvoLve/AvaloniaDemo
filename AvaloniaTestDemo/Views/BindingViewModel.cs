using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class BindingViewModel() :  DemoPageBase("绑定", MaterialIconKind.LinkVariant)
{
    //数字绑定
    [ObservableProperty]
    public partial int Number { get; set; } = 0;

    // 当前选中项
    [ObservableProperty]
    public partial Option SelectedSeason { get; set; }

    // 枚举列表
    public IEnumerable<Option> Season { get; }
        = Enum.GetValues<Option>();
    
    public enum Option
    {
        [Description("春天")]
        Spring,
    
        [Description("夏天")]
        Summer,
    
        [Description("秋天")]
        Autumn,
    
        [Description("冬天")]
        Winter
    }
    
}
