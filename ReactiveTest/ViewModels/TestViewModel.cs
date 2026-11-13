using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Sextant;

namespace ReactiveTest.ViewModels;

public partial class TestViewModel : ViewModelBase, INavigable
{
    [Reactive]
    public partial string Name { get; set; }

    private IObservable<bool> canSave { get; set; }

    public TestViewModel()
    {
        // 当 Name 属性不为空时，canSave 为 true，否则为 false
        canSave = this.WhenAnyValue(
            x => x.Name, name => !string.IsNullOrWhiteSpace(name));
        
        // 订阅 Name 属性的变化，打印变化后的值
        this.WhenAnyValue(x => x.Name)
            .Subscribe(name =>
            {
                // 处理 Name 属性的变化
                Console.WriteLine($"Name changed to: {name}");
            });
    }
    
    //输入框不为空时才能点击
    [ReactiveCommand(CanExecute = nameof(canSave))]
    private void Save()
    {
        Console.WriteLine(Name);
    }

    /// <inheritdoc />
    public string? Id { get; } = nameof(TestViewModel);

    /// <inheritdoc />
    public IObservable<Unit> WhenNavigatedTo(INavigationParameter parameter) => Observable.Return(Unit.Default);

    /// <inheritdoc />
    public IObservable<Unit> WhenNavigatedFrom(INavigationParameter parameter) => Observable.Return(Unit.Default);

    /// <inheritdoc />
    public IObservable<Unit> WhenNavigatingTo(INavigationParameter parameter) => Observable.Return(Unit.Default);
}
