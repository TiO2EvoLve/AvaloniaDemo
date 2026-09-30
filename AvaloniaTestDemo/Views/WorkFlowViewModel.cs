using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
using AvaloniaTestDemo.Views.WorkFlow;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using WorkflowCore.Interface;

namespace AvaloniaTestDemo.Views;

public partial class WorkFlowViewModel : DemoPageBase
{
    private readonly IWorkflowHost _host;

    private string? _workflowId;

    /// <summary>
    /// 流程显示
    /// </summary>
    [ObservableProperty]
    private string _currentStatus = "流程未启动";

    /// <summary>
    /// 当前状态
    /// </summary>
    [ObservableProperty]
    private string _approvalStatus = "等待开始";

    [ObservableProperty]
    private bool _isApprovalWaiting;

    [ObservableProperty]
    private int _currentApproveIndex;

    public ObservableCollection<WorkflowStepItem> Steps { get; } =
    [
        new("需求提交","待执行",Brushes.Gray),
        new("数据校验","待执行",Brushes.Gray),
        new("经理审批","待执行",Brushes.Gray),
        new("总监审批","待执行",Brushes.Gray),
        new("财务审批","待执行",Brushes.Gray),
        new("流程结束","待执行",Brushes.Gray)
    ];

    public WorkFlowViewModel(IWorkflowHost host) : base("工作流", MaterialIconKind.Work)
    {
        _host = host;

        ResetWorkflowState();
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        ResetWorkflowState();

        var data = new ApprovalWorkflowData();

        _workflowId =
            await _host.StartWorkflow(
                "ApprovalWorkflow",
                1,
                data);

        data.WorkflowId = _workflowId;

        SetStepState(
            0,
            "已完成",
            Brushes.LimeGreen,
            "需求已提交");

        SetStepState(
            1,
            "已完成",
            Brushes.LimeGreen,
            "数据校验通过");

        SetStepState(
            2,
            "等待审批",
            Brushes.Orange,
            "等待经理审批");

        CurrentApproveIndex = 2;

        CurrentStatus = "等待经理审批";

        ApprovalStatus = "经理审批";

        IsApprovalWaiting = true;
    }

    [RelayCommand]
    private async Task ApproveAsync()
    {
        if (_workflowId is null)
            return;

        switch (CurrentApproveIndex)
        {
            case 2:

                await _host.PublishEvent(
                    "ManagerApprove",
                    _workflowId,
                    true);

                SetStepState(
                    2,
                    "已完成",
                    Brushes.LimeGreen,
                    "经理审批通过");

                SetStepState(
                    3,
                    "等待审批",
                    Brushes.Orange,
                    "等待总监审批");

                CurrentApproveIndex = 3;

                ApprovalStatus = "总监审批";

                CurrentStatus = "等待总监审批";

                break;

            case 3:

                await _host.PublishEvent(
                    "DirectorApprove",
                    _workflowId,
                    true);

                SetStepState(
                    3,
                    "已完成",
                    Brushes.LimeGreen,
                    "总监审批通过");

                SetStepState(
                    4,
                    "等待审批",
                    Brushes.Orange,
                    "等待财务审批");

                CurrentApproveIndex = 4;

                ApprovalStatus = "财务审批";

                CurrentStatus = "等待财务审批";

                break;

            case 4:

                await _host.PublishEvent(
                    "FinanceApprove",
                    _workflowId,
                    true);

                SetStepState(
                    4,
                    "已完成",
                    Brushes.LimeGreen,
                    "财务审批通过");

                SetStepState(
                    5,
                    "已完成",
                    Brushes.LimeGreen,
                    "流程结束");

                ApprovalStatus = "已完成";

                CurrentStatus = "流程完成";

                IsApprovalWaiting = false;

                break;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        ResetWorkflowState();

        CurrentStatus = "流程取消";

        ApprovalStatus = "已取消";
    }

    private void SetStepState(
        int index,
        string state,
        IBrush brush,
        string description)
    {
        var step = Steps[index];

        step.Status = state;
        step.StatusBrush = brush;
        step.Description = description;
    }

    private void ResetWorkflowState()
    {
        CurrentApproveIndex = 0;

        IsApprovalWaiting = false;

        CurrentStatus = "流程未启动";

        ApprovalStatus = "等待开始";

        foreach (var step in Steps)
        {
            step.Status = "待执行";
            step.Description = "等待执行";
            step.StatusBrush = Brushes.Gray;
        }
    }
}

public partial class WorkflowStepItem(
    string title,
    string status,
    IBrush statusBrush) : ObservableObject
{
    [ObservableProperty]
    private string _title = title;

    [ObservableProperty]
    private string _status = status;

    [ObservableProperty]
    private string _description = "等待执行";

    [ObservableProperty]
    private IBrush _statusBrush = statusBrush;
}