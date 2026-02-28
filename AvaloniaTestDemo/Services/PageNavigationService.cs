using System;
using DemoPageBase = AvaloniaTestDemo.Views.DemoPageBase;

namespace AvaloniaTestDemo.Services;

public class PageNavigationService
{
    public Action<Type>? NavigationRequested { get; set; }

    public void RequestNavigation<T>() where T : DemoPageBase
    {
        NavigationRequested?.Invoke(typeof(T));
    }
}