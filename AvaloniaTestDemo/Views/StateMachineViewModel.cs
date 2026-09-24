using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using Stateless;

namespace AvaloniaTestDemo.Views;

public partial class StateMachineViewModel : DemoPageBase
{
    public enum LightState
    {
        Red,
        Yellow,
        Green
    }

    public enum Trigger
    {
        ToRed,
        ToYellow,
        ToGreen
    }

    private readonly StateMachine<LightState, Trigger> _machine;

    private CancellationTokenSource? _cts;

    [ObservableProperty]
    private string currentStateText = "";

    [ObservableProperty]
    private string lastTransition = "";

    [ObservableProperty]
    private string permittedTriggersText = "";

    [ObservableProperty]
    private string autoRunButtonText = "停止自动运行";

    [ObservableProperty]
    private IBrush redBrush = Brushes.DarkRed;

    [ObservableProperty]
    private IBrush yellowBrush = Brushes.DarkGoldenrod;

    [ObservableProperty]
    private IBrush greenBrush = Brushes.DarkGreen;

    private bool _autoRunning = true;

    public StateMachineViewModel() : base("状态机", MaterialIconKind.StateMachine)
    {
        _machine = new StateMachine<LightState, Trigger>(LightState.Red);

        ConfigureStateMachine();

        UpdateUI();

        StartAutoRun();
    }

    private void ConfigureStateMachine()
    {
        _machine.Configure(LightState.Red)
            .Permit(Trigger.ToYellow, LightState.Yellow)
            .Permit(Trigger.ToGreen, LightState.Green);

        _machine.Configure(LightState.Yellow)
            .Permit(Trigger.ToRed, LightState.Red)
            .Permit(Trigger.ToGreen, LightState.Green);

        _machine.Configure(LightState.Green)
            .Permit(Trigger.ToRed, LightState.Red)
            .Permit(Trigger.ToYellow, LightState.Yellow);

        _machine.OnTransitioned(t =>
        {
            LastTransition =
                $"最近变化：{t.Source} → {t.Destination}";
        });
    }

    private void UpdateUI()
    {
        RedBrush = Brushes.DarkRed;
        YellowBrush = Brushes.DarkGoldenrod;
        GreenBrush = Brushes.DarkGreen;

        switch (_machine.State)
        {
            case LightState.Red:
                RedBrush = Brushes.OrangeRed;
                break;

            case LightState.Yellow:
                YellowBrush = Brushes.Yellow;
                break;

            case LightState.Green:
                GreenBrush = Brushes.LimeGreen;
                break;
        }

        CurrentStateText =
            $"当前状态：{_machine.State}";

        var triggers = _machine.GetPermittedTriggersAsync().Result.Select(x => x.ToString());
        
        PermittedTriggersText = $"允许触发器：{string.Join(", ", triggers)}";
    }

    private void StartAutoRun()
    {
        _cts = new CancellationTokenSource();

        _ = Task.Run(async () =>
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), _cts.Token);

                    if (_cts.IsCancellationRequested) break;

                    switch (_machine.State)
                    {
                        case LightState.Red:
                            await _machine.FireAsync(Trigger.ToGreen);
                            break;

                        case LightState.Green:
                            await _machine.FireAsync(Trigger.ToYellow);
                            break;

                        case LightState.Yellow:
                            await _machine.FireAsync(Trigger.ToRed);
                            break;
                    }

                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(UpdateUI);
                }
                catch
                {
                    break;
                }
            }
        });
    }

    private void StopAutoRun()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    [RelayCommand]
    private void ToRed()
    {
        if (_machine.State == LightState.Red)
            return;

        _machine.Fire(Trigger.ToRed);

        UpdateUI();
    }

    [RelayCommand]
    private void ToYellow()
    {
        if (_machine.State == LightState.Yellow)
            return;

        _machine.Fire(Trigger.ToYellow);

        UpdateUI();
    }

    [RelayCommand]
    private void ToGreen()
    {
        if (_machine.State == LightState.Green)
            return;

        _machine.Fire(Trigger.ToGreen);

        UpdateUI();
    }

    [RelayCommand]
    private void ToggleAutoRun()
    {
        if (_autoRunning)
        {
            StopAutoRun();

            _autoRunning = false;

            AutoRunButtonText = "启动自动运行";
        }
        else
        {
            StartAutoRun();

            _autoRunning = true;

            AutoRunButtonText = "停止自动运行";
        }
    }
}
