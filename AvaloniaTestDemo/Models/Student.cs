using System.ComponentModel.DataAnnotations.Schema;

namespace AvaloniaTestDemo.Models;

[Table("student")]
public class Student
{
    public Student(string name, int age, string sex)
    {
        this.name = name;
        this.age = age;
        this.sex = sex;
    }

    public Student()
    {
    }

    public int id { get; set; }
    public string name { get; set; }
    public int age { get; set; }

    public string sex { get; set; }
}