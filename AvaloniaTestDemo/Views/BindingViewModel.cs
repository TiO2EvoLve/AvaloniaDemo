using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class BindingViewModel() :  DemoPageBase("绑定", MaterialIconKind.LinkVariant)
{
    ////////////////////////////////////变量绑定////////////////////////////////////
    //滑动条绑定数字
    [ObservableProperty] private int number;

    // 下拉框当前选择的项
    [ObservableProperty]
    public partial Option SelectedSeason { get; set; }

    // 枚举列表，下拉框绑定
    public IEnumerable<Option> Season { get; } = Enum.GetValues<Option>();
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
    //checkbox绑定
    [ObservableProperty]
    private bool isCheck;
    
    ////////////////////////////////////转换器绑定////////////////////////////////////
    [ObservableProperty] public bool boolValue;
    [ObservableProperty] public bool boolValue2  = true;
    [ObservableProperty] public string stringValue;
    
}
