using System;
using ReactiveUI.SourceGenerators;

namespace ReactiveTest.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [Reactive] private string _greeting = "Welcome to Avalonia!";

    [ReactiveCommand]
    private void Run()
    {
        Console.WriteLine("ok");
    }
}