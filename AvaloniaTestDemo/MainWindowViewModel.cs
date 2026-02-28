using System.Linq;
using Avalonia.Collections;
using AvaloniaTestDemo.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using DemoPageBase = AvaloniaTestDemo.Views.DemoPageBase;

namespace AvaloniaTestDemo;

public partial class MainWindowViewModel : ObservableObject
{
    public IAvaloniaReadOnlyList<DemoPageBase> DemoPages { get; }
    [ObservableProperty] private DemoPageBase? _activePage;
    public PageNavigationService PageNavigationService { get; }

    public MainWindowViewModel(IAvaloniaReadOnlyList<DemoPageBase> demoPages,PageNavigationService pageNavigationService)
    {
        DemoPages = new AvaloniaList<DemoPageBase>(demoPages.OrderBy(x => x.Index).ThenBy(x => x.DisplayName));
        PageNavigationService = pageNavigationService;
        pageNavigationService.NavigationRequested += pageType =>
        {
            var page = DemoPages.FirstOrDefault(x => x.GetType() == pageType);
            if (page is null || ActivePage?.GetType() == pageType) return;
            ActivePage = page;
        };
    }
}