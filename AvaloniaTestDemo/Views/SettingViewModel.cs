using System;
using Avalonia.Collections;
using Avalonia.Styling;
using AvaloniaTestDemo.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using SukiUI;
using SukiUI.Enums;
using SukiUI.Models;

namespace AvaloniaTestDemo.Views;

public partial class SettingViewModel : DemoPageBase
{
    public Action<SukiBackgroundStyle>? BackgroundStyleChanged { get; set; }
    public Action<bool>? BackgroundAnimationsChanged { get; set; }
    public Action<bool>? BackgroundTransitionsChanged { get; set; }
    
    public IAvaloniaReadOnlyList<SukiColorTheme> AvailableColors { get; }
    public IAvaloniaReadOnlyList<SukiBackgroundStyle> AvailableBackgroundStyles { get; }

    private readonly SukiTheme _theme = SukiTheme.GetInstance();

    [ObservableProperty] private bool _isLightTheme;
    [ObservableProperty] private SukiBackgroundStyle _backgroundStyle ;
    [ObservableProperty] private bool _backgroundAnimations;
    [ObservableProperty] private bool _backgroundTransitions;
    
    public SettingViewModel() : base("Theming", MaterialIconKind.PaletteOutline, -200)
    {
        AvailableBackgroundStyles = new AvaloniaList<SukiBackgroundStyle>(Enum.GetValues<SukiBackgroundStyle>());
        AvailableColors = _theme.ColorThemes;
        IsLightTheme = _theme.ActiveBaseTheme == ThemeVariant.Light;
        _theme.OnBaseThemeChanged += variant =>
            IsLightTheme = variant == ThemeVariant.Light;
    }

    partial void OnIsLightThemeChanged(bool value) =>
        _theme.ChangeBaseTheme(value ? ThemeVariant.Light : ThemeVariant.Dark);
    
    [RelayCommand]
    private void SwitchToColorTheme(SukiColorTheme colorTheme) =>
        _theme.ChangeColorTheme(colorTheme);

    partial void OnBackgroundStyleChanged(SukiBackgroundStyle value) => 
        BackgroundStyleChanged?.Invoke(value);

    partial void OnBackgroundAnimationsChanged(bool value) => 
        BackgroundAnimationsChanged?.Invoke(value);

    partial void OnBackgroundTransitionsChanged(bool value) =>
        BackgroundTransitionsChanged?.Invoke(value);
    
}