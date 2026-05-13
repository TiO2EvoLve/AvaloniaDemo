using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using Stateless;

namespace AvaloniaTestDemo.Views;

public partial class StateMachineViewModel : DemoPageBase
{
    // 状态
    public enum State
    {
        Created,    // 已创建（未支付）
        Paid,       // 已支付
        Shipped,    // 已发货
        Completed,  // 已完成
        Cancelled   // 已取消
    }

    // 触发器
    public enum Trigger
    {
        Pay,
        Ship,
        Complete,
        Cancel,
        Reset
    }

    private readonly StateMachine<State, Trigger> _machine;

    [ObservableProperty]
    private string currentStateText;

    public StateMachineViewModel() : base("状态机", MaterialIconKind.StateMachine,1)
    {
        _machine = new StateMachine<State, Trigger>(State.Created);

        // ===== Created =====
        _machine.Configure(State.Created)
            .Permit(Trigger.Pay, State.Paid)
            .Permit(Trigger.Cancel, State.Cancelled)
            .OnEntry(() => CurrentStateText = "状态：已创建（待支付）");

        // ===== Paid =====
        _machine.Configure(State.Paid)
            .Permit(Trigger.Ship, State.Shipped)
            .Permit(Trigger.Cancel, State.Cancelled)
            .OnEntry(() => CurrentStateText = "状态：已支付");

        // ===== Shipped =====
        _machine.Configure(State.Shipped)
            .Permit(Trigger.Complete, State.Completed)
            // 注意：发货后不能取消（业务规则）
            .OnEntry(() => CurrentStateText = "状态：已发货");

        // ===== Completed =====
        _machine.Configure(State.Completed)
            .Permit(Trigger.Reset, State.Created)
            .OnEntry(() => CurrentStateText = "状态：已完成 ✅");

        // ===== Cancelled =====
        _machine.Configure(State.Cancelled)
            .Permit(Trigger.Reset, State.Created)
            .OnEntry(() => CurrentStateText = "状态：已取消 ❌");

        CurrentStateText = "状态：已创建（待支付）";
    }

    // ===== Commands =====

    [RelayCommand]
    private void Pay()
    {
        if (_machine.CanFire(Trigger.Pay))
            _machine.Fire(Trigger.Pay);
    }

    [RelayCommand]
    private void Ship()
    {
        if (_machine.CanFire(Trigger.Ship))
            _machine.Fire(Trigger.Ship);
    }

    [RelayCommand]
    private void Complete()
    {
        if (_machine.CanFire(Trigger.Complete))
            _machine.Fire(Trigger.Complete);
    }

    [RelayCommand]
    private void Cancel()
    {
        if (_machine.CanFire(Trigger.Cancel))
            _machine.Fire(Trigger.Cancel);
    }

    [RelayCommand]
    private void Reset()
    {
        if (_machine.CanFire(Trigger.Reset))
            _machine.Fire(Trigger.Reset);
    }
}
