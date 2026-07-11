using System.ComponentModel.DataAnnotations.Schema;
namespace AvaloniaTestDemo.Models;
using CommunityToolkit.Mvvm.ComponentModel;

[Table("student")]
// 使用 ObservableProperty 自动实现通知
public partial class Student : ObservableObject
{
    public int Id  { get; set; }
    
    public string? Name { get; set; }
    
    public int Age { get; set; }
    
    public string? Sex { get; set; }

    public void clear()
    {
        Id = 0;
        Name = null;
        Age = 0;
        Sex = null;
    }
}