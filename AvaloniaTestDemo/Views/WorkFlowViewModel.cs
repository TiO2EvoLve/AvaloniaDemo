using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class WorkFlowViewModel : DemoPageBase
{
    private CancellationTokenSource? _cts;
    private TaskCompletionSource<bool>? _approvalTcs;

    [ObservableProperty]
    private string _currentStatus = "流程未启动";

    [ObservableProperty]
    private string _approvalStatus = "等待开始";

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private bool _isApprovalWaiting;

    public ObservableCollection<WorkflowStepItem> Steps { get; } =
    [
        new ("需求提交", "待执行", Brushes.Gray),
        new ("数据校验", "待执行", Brushes.Gray),
        new ("等待审批", "待执行", Brushes.Gray),
        new ("执行审批后任务", "待执行", Brushes.Gray),
        new ("流程结束", "待执行", Brushes.Gray)
    ];

    public WorkFlowViewModel() : base("工作流", MaterialIconKind.Work, 100)
    {
        ResetWorkflowState();
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        if (IsRunning)
            return;

        ResetWorkflowState();
        _cts = new CancellationTokenSource();
        IsRunning = true;
        CurrentStatus = "流程已开始";
        ApprovalStatus = "审批未开始";

        try
        {
            await ExecuteWorkflowAsync(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            CurrentStatus = "流程已取消";
            ApprovalStatus = "已取消";
            IsApprovalWaiting = false;
        }
        finally
        {
            IsRunning = false;
            if (_cts is not null && !_cts.IsCancellationRequested)
            {
                _cts.Dispose();
                _cts = null;
            }
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        if (!IsRunning || _cts is null)
            return;

        _cts.Cancel();
        _approvalTcs?.TrySetCanceled();
        IsApprovalWaiting = false;
        CurrentStatus = "流程已取消";
        ApprovalStatus = "已取消";
    }

    [RelayCommand]
    private void Approve()
    {
        if (!IsApprovalWaiting || _approvalTcs is null)
            return;

        IsApprovalWaiting = false;
        ApprovalStatus = "已审批";
        CurrentStatus = "审批已通过，流程继续执行";
        _approvalTcs.TrySetResult(true);
    }

    private async Task ExecuteWorkflowAsync(CancellationToken cancellationToken)
    {
        await RunStepAsync(0, "已完成", Brushes.LimeGreen, "需求已提交", cancellationToken);
        await RunStepAsync(1, "已完成", Brushes.LimeGreen, "数据校验通过", cancellationToken);

        SetStepState(2, "等待审批", Brushes.Orange, "审批节点已到达，等待审批");
        IsApprovalWaiting = true;
        ApprovalStatus = "等待审批";
        CurrentStatus = "流程暂停，等待审批";
        await WaitForApprovalAsync(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException(cancellationToken);

        SetStepState(2, "已完成", Brushes.LimeGreen, "审批已通过");
        await RunStepAsync(3, "已完成", Brushes.LimeGreen, "审批后任务正在执行", cancellationToken);
        await RunStepAsync(4, "已完成", Brushes.LimeGreen, "流程结束", cancellationToken);

        IsApprovalWaiting = false;
        ApprovalStatus = "已审批";
        CurrentStatus = "流程已完成";
    }

    private async Task WaitForApprovalAsync(CancellationToken cancellationToken)
    {
        _approvalTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            await _approvalTcs.Task.WaitAsync(cancellationToken);
        }
        finally
        {
            _approvalTcs = null;
        }
    }

    private async Task RunStepAsync(int index, string state, IBrush brush, string description, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SetStepState(index, state, brush, description);
        await Task.Delay(350, cancellationToken);
    }

    private void SetStepState(int index, string state, IBrush brush, string description)
    {
        if (index < 0 || index >= Steps.Count)
            return;

        var step = Steps[index];
        step.Status = state;
        step.StatusBrush = brush;
        step.Description = description;
    }

    private void ResetWorkflowState()
    {
        IsRunning = false;
        IsApprovalWaiting = false;
        ApprovalStatus = "等待开始";
        CurrentStatus = "流程未启动";

        for (var i = 0; i < Steps.Count; i++)
        {
            var step = Steps[i];
            step.Status = "待执行";
            step.Description = "等待执行";
            step.StatusBrush = Brushes.Gray;
        }
    }
}

public partial class WorkflowStepItem : ObservableObject
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _status;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private IBrush _statusBrush;

    public WorkflowStepItem(string title, string status, IBrush statusBrush)
    {
        _title = title;
        _status = status;
        _description = "等待执行";
        _statusBrush = statusBrush;
    }
}