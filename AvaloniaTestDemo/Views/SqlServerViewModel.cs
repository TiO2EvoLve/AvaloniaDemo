
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaTestDemo.Models;
using AvaloniaTestDemo.Services;
using AvaloniaTestDemo.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaTestDemo.Views;

public partial class SqlServerViewModel : DemoPageBase
{
    [ObservableProperty] private ObservableCollection<Student> _sqlData = new(); //表格数据绑定
    [ObservableProperty] private Student selectStudent = new(); //回显数据绑定
    [ObservableProperty] private Student queryStudent = new(); //查询条件绑定
    public List<string> SexOptions { get; } = ["男", "女"]; // 性别选项绑定

    public SqlServerViewModel() : base("SqlServer", MaterialIconKind.Database, 0)
    {
        SqlSelect().ConfigureAwait(true); //初始化查询
    }
    
    //查询方法绑定
    [RelayCommand]
    private async Task SqlSelect()
    {
        await using var context = new ApplicationDbContext();
        var query = context.Students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(QueryStudent?.Name))
        {
            // 使用 Like 进行模糊查询，支持自定义通配符
            query = query.Where(s => EF.Functions.Like(s.Name, $"%{QueryStudent.Name}%"));
        }

        if (QueryStudent?.Age > 0)
        {
            query = query.Where(s => s.Age == QueryStudent.Age);
        }

        if (!string.IsNullOrWhiteSpace(QueryStudent?.Sex))
        {
            query = query.Where(s => EF.Functions.Like(s.Sex, $"%{QueryStudent.Sex}%"));
        }

        var students = await Task.Run(() => query.ToList());
        SqlData = new ObservableCollection<Student>(students);
    }

    //插入方法绑定
    [RelayCommand]
    private async Task SqlInsert()
    {
        var addWindow = new Add
        {
            DataContext = this
        };

        await addWindow.ShowDialog(App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null);
        
        if (string.IsNullOrWhiteSpace(SelectStudent.Name) || SelectStudent.Age == 0 || string.IsNullOrWhiteSpace(SelectStudent.Sex)) //检查是否有空属性
        {
            return;
        }

        await using var context = new ApplicationDbContext();
        await context.Students.AddAsync(SelectStudent);
        await context.SaveChangesAsync();
        await SqlSelect(); // 刷新列表
    }

    //编辑方法绑定
    [RelayCommand]
    private async Task SqlEdit(Student student)
    {
        await using var context = new ApplicationDbContext();
        //回显数据
        SelectStudent = context.Students.Single(s => s.Id == student.Id);
        var addWindow = new Add
        {
            DataContext = this
        };
        await addWindow.ShowDialog(App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null);
        //保存修改后的数据
        context.Students.Update(SelectStudent);
        await context.SaveChangesAsync();
        await SqlSelect(); // 刷新列表
        SelectStudent.clear();
    }

    //删除方法绑定
    [RelayCommand]
    private async Task SqlDelete(Student student)
    {
        await using var context = new ApplicationDbContext();
        context.Students.Remove(student);
        await context.SaveChangesAsync();
        await SqlSelect();
    }

    //重置方法绑定
    [RelayCommand]
    private async Task SqlReset(Student student)
    {
        QueryStudent.clear();
        await SqlSelect();
    }
}