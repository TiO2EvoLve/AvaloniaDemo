using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class DynamicDataViewModel : DemoPageBase
{
    private readonly SourceCache<Person, int> _people;

    private readonly ReadOnlyObservableCollection<Person> _filtered;
    public ReadOnlyObservableCollection<Person> Filtered => _filtered;

    [ObservableProperty]
    private string searchText = "";

    private int _idSeed = 1;

    public DynamicDataViewModel() : base("Dynamic Data", MaterialIconKind.MicrosoftDynamics365, int.MinValue)
    {
        _people = new SourceCache<Person, int>(p => p.Id);

        // ====== DynamicData 核心链路 ======
        var searchObservable = this
            .WhenPropertyChanged(x => x.SearchText)
            .Select(x => x.Value ?? "")
            .Throttle(TimeSpan.FromMilliseconds(200))
            .StartWith("");

        _people.Connect()
            // 🔍 实时过滤
            .Filter(searchObservable, (term, person) =>
                string.IsNullOrWhiteSpace(term) ||
                person.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            // 📊 自动排序
            .Sort(SortExpressionComparer<Person>
                .Ascending(p => p.Age))

            // 🔗 绑定到 UI
            .Bind(out _filtered)
            .Subscribe();

        // 初始化数据
        Seed();
    }

    private void Seed()
    {
        _people.AddOrUpdate(new[]
        {
            new Person { Id = _idSeed++, Name = "Alice", Age = 24 },
            new Person { Id = _idSeed++, Name = "Bob", Age = 30 },
            new Person { Id = _idSeed++, Name = "Charlie", Age = 18 },
            new Person { Id = _idSeed++, Name = "David", Age = 40 },
        });
    }

    // ➕ 添加数据
    [RelayCommand]
    private void Add()
    {
        _people.AddOrUpdate(new Person
        {
            Id = _idSeed++,
            Name = $"User {_idSeed}",
            Age = Random.Shared.Next(18, 50)
        });
    }

    // ❌ 删除数据
    [RelayCommand]
    private void Remove(Person person)
    {
        _people.RemoveKey(person.Id);
    }
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }
}