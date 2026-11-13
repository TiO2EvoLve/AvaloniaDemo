
using ReactiveUI.SourceGenerators;

namespace ReactiveTest.ViewModels;

public partial class MainViewModel(ReadCardDemoViewModel readCardDemoViewModel, TestViewModel testViewModel) : ViewModelBase
{
    [Reactive]
    private ViewModelBase _currentPage = readCardDemoViewModel;
    private ReadCardDemoViewModel _readCard { get; } = readCardDemoViewModel;
    private TestViewModel _test { get; } = testViewModel;
    
    [ReactiveCommand]
    private void Navigate(string page) => CurrentPage = page switch
    {
        "readcard" => _readCard,
        "test" => _test,
        _ => _readCard
    };
}
