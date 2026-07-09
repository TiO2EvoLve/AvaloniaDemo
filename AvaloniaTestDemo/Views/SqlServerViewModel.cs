using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaTestDemo.Models;
using AvaloniaTestDemo.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaTestDemo.Views;

public partial class SqlServerViewModel() : DemoPageBase("SqlServer 数据查询", MaterialIconKind.Database, int.MinValue)
{
    
    private List<Student> stu = [];
    
    [ObservableProperty]
    private ObservableCollection<Student> _sqlData = new();

    [RelayCommand]
    private async Task SqlSelect()
    {
        await using var context = new ApplicationDbContext();
        var students = await Task.Run(() => context.Students.ToList());
        SqlData = new ObservableCollection<Student>(students);
    }
    
    [RelayCommand]
    private async Task SqlUpdate()
    {
        await using var context = new ApplicationDbContext();
        context.Students.UpdateRange(SqlData);
        await context.SaveChangesAsync();
    }

    [RelayCommand]
    private async Task SqlEdit()
    {
        await using var context = new ApplicationDbContext();
    }
    
    [RelayCommand]
    private async Task SqlDelete()
    {
        await using var context = new ApplicationDbContext();
    }
}