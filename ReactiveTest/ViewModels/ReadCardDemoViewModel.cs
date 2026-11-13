using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Sextant;

namespace ReactiveTest.ViewModels;

public partial class ReadCardDemoViewModel : ViewModelBase,INavigable
{
    #region 响应式属性

    [Reactive] private bool _isConnected;

    [Reactive] private bool _isAuthenticated;

    [Reactive] private bool _isBusy;

    [Reactive] private string _searchText = string.Empty;

    [Reactive] private string _result = "等待操作...";

    #endregion

    #region 派生属性

    // ReactiveUI 22 没有 [ObservableAsProperty] 源生成器（该特性来自 ReactiveUI.Binding），
    // 因此这里显式使用 ObservableAsPropertyHelper。
    private readonly ObservableAsPropertyHelper<string> _status;

    public string Status => _status.Value;

    private readonly ObservableAsPropertyHelper<ObservableCollection<string>> _filteredDevices;

    public ObservableCollection<string> FilteredDevices => _filteredDevices.Value;

    #endregion
    
    #region 命令执行条件

    private readonly IObservable<bool> _canRead;

    #endregion

    #region 设备数据

    private readonly string[] _devices =
    {
        "USB 智能卡读卡器",
        "接触式 CPU 卡读卡器",
        "非接触式 IC 卡读卡器",
        "SAM 安全模块"
    };

    #endregion

    public ReadCardDemoViewModel()
    {
        // 1. 设备状态：组合三个属性
        _status = this.WhenAnyValue(x => x.IsConnected)
            .CombineLatest(this.WhenAnyValue(x => x.IsAuthenticated),
                this.WhenAnyValue(x => x.IsBusy),
                (connected, authenticated, busy) =>
                {
                    if (busy)
                        return "设备忙碌";

                    if (!connected)
                        return "设备未连接";

                    if (!authenticated)
                        return "等待身份认证";

                    return "设备就绪";
                })
            .ToProperty(this, x => x.Status);

        // 2. 实时搜索设备
        _filteredDevices = this.WhenAnyValue(x => x.SearchText)
            .Select(keyword =>
            {
                var result = _devices.Where(d =>
                    d.Contains(keyword ?? "", StringComparison.OrdinalIgnoreCase)).ToList();
                return new ObservableCollection<string>(result);
            })
            .ToProperty(this, x => x.FilteredDevices);

        // 3. 读卡条件
        _canRead = this.WhenAnyValue(x => x.IsConnected).CombineLatest(this.WhenAnyValue(x => x.IsAuthenticated),
            this.WhenAnyValue(x => x.IsBusy),
            (connected, authenticated, busy) =>
                connected && authenticated && !busy);

        // 4. 订阅读卡命令返回结果
        ReadCardCommand.Subscribe(result => { Result = result; });
    }

    #region ReactiveCommand

    [ReactiveCommand(CanExecute = nameof(_canRead))]
    private async Task<string> ReadCardAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        Result = "正在读取 CPU 卡...";

        try
        {
            // 模拟读卡操作
            await Task.Delay(2000, cancellationToken);

            return "CPU 卡读取成功！";
        }
        catch (OperationCanceledException)
        {
            return "读卡操作已取消";
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    /// <inheritdoc />
    public string? Id { get; } = nameof(ReadCardDemoViewModel);

    /// <inheritdoc />
    public IObservable<Unit> WhenNavigatedTo(INavigationParameter parameter) => Observable.Return(Unit.Default);

    /// <inheritdoc />
    public IObservable<Unit> WhenNavigatedFrom(INavigationParameter parameter) => Observable.Return(Unit.Default);

    /// <inheritdoc />
    public IObservable<Unit> WhenNavigatingTo(INavigationParameter parameter) => Observable.Return(Unit.Default);
}