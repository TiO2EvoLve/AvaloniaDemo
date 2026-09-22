using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiteDB;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class LiteDBViewModel : DemoPageBase
{
    
    private readonly AppDatabase _database;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private int age = 20;

    [ObservableProperty]
    private User selectedUser;

    [ObservableProperty]
    private string message = "就绪";

    public ObservableCollection<User> Users { get; } = new ();

    public LiteDBViewModel() : base("LiteDB", MaterialIconKind.Database, int.MinValue) 
    {
        _database = new AppDatabase();

        LoadUsers();
    }

    /// <summary>
    /// 查询数据库
    /// </summary>
    private void LoadUsers()
    {
        Users.Clear();

        var users = _database.Users
            .FindAll()
            .OrderBy(x => x.Id);

        foreach (var user in users)
        {
            Users.Add(user);
        }

        Message = $"当前共有 {Users.Count} 条数据";
    }

    /// <summary>
    /// 新增
    /// </summary>
    [RelayCommand]
    private void AddUser()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            Message = "请输入姓名";
            return;
        }

        var user = new User
        {
            Name = Name,
            Age = Age
        };

        _database.Users.Insert(user);

        Users.Add(user);

        Message = $"新增成功：{user.Name}";

        ClearInput();
    }

    /// <summary>
    /// 修改
    /// </summary>
    [RelayCommand]
    private void UpdateUser()
    {
        if (SelectedUser == null)
        {
            Message = "请先选择一条数据";
            return;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            Message = "请输入姓名";
            return;
        }

        SelectedUser.Name = Name;
        SelectedUser.Age = Age;

        _database.Users.Update(SelectedUser);

        // ObservableCollection 中的对象本身已经修改
        // 为了简单演示，重新加载一次
        LoadUsers();

        Message = "修改成功";
    }

    /// <summary>
    /// 删除
    /// </summary>
    [RelayCommand]
    private void DeleteUser()
    {
        if (SelectedUser == null)
        {
            Message = "请先选择一条数据";
            return;
        }

        _database.Users.Delete(SelectedUser.Id);

        Users.Remove(SelectedUser);

        Message = $"删除成功：{SelectedUser.Name}";

        ClearInput();
    }

    /// <summary>
    /// 从数据库重新查询
    /// </summary>
    [RelayCommand]
    private void Refresh()
    {
        LoadUsers();

        Message = "刷新成功";
    }

    /// <summary>
    /// 清空输入框
    /// </summary>
    [RelayCommand]
    private void ClearInput()
    {
        Name = string.Empty;
        Age = 20;
        SelectedUser = null;
    }

    /// <summary>
    /// 点击表格中的用户
    /// </summary>
    partial void OnSelectedUserChanged(User value)
    {
        if (value == null)
        {
            return;
        }

        Name = value.Name;
        Age = value.Age;
    }
}

public class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }
}
public class AppDatabase : IDisposable
{
    private readonly LiteDatabase _database;

    public ILiteCollection<User> Users
    {
        get { return _database.GetCollection<User>("users"); }
    }

    public AppDatabase()
    {
        string appData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);

        string directory = Path.Combine(appData, "LiteDbDemo");

        Directory.CreateDirectory(directory);

        string databasePath = Path.Combine(directory, "data.db");

        _database = new LiteDatabase(databasePath);

        // Name 建立索引
        Users.EnsureIndex(x => x.Name);
    }

    public void Dispose()
    {
        _database.Dispose();
    }
}