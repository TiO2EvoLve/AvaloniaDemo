using System;
using Microsoft.EntityFrameworkCore.Storage;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace AvaloniaTestDemo.Views.WorkFlow;

public class ApprovalWorkflow : IWorkflow<ApprovalWorkflowData>
{
    public string Id => "ApprovalWorkflow";

    public int Version => 1;

    public void Build(IWorkflowBuilder<ApprovalWorkflowData> builder)
    {
        builder

            .StartWith(context =>
            {
                return ExecutionResult.Next();
            })

            .Then(context =>
            {
                return ExecutionResult.Next();
            })

            .WaitFor(
                "ManagerApprove",
                data => data.WorkflowId,
                data => DateTime.Now)

            .Then(context =>
            {
                return ExecutionResult.Next();
            })

            .WaitFor(
                "DirectorApprove",
                data => data.WorkflowId,
                data => DateTime.Now)

            .Then(context =>
            {
                return ExecutionResult.Next();
            })

            .WaitFor(
                "FinanceApprove",
                data => data.WorkflowId,
                data => DateTime.Now)

            .Then(context =>
            {
                return ExecutionResult.Next();
            });
    }
}