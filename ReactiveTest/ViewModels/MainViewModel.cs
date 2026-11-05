
using ReactiveUI.SourceGenerators;

namespace ReactiveTest.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [Reactive]
    private string _greeting = "Welcome to Avalonia!";

    public MainViewModel()
    {
        
    }

    [ReactiveCommand]
    public void Test()
    {
        
    }
    
}