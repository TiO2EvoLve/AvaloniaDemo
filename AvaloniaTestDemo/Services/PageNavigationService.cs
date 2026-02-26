using System;
using AvaloniaTestDemo.Models;

namespace AvaloniaTestDemo.Services;

public class PageNavigationService
{
    public Action<Type>? NavigationRequested { get; set; }

    public void RequestNavigation<T>() where T : DemoPageBase
    {
        NavigationRequested?.Invoke(typeof(T));
    }
}