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
    //滑动条绑定数字
    [ObservableProperty]
    public partial int Number { get; set; } = 0;

    // 下拉框当前选择的项
    [ObservableProperty]
    public partial Option SelectedSeason { get; set; }

    // 枚举列表，下拉框绑定
    public IEnumerable<Option> Season { get; }
        = Enum.GetValues<Option>();
    //枚举
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
