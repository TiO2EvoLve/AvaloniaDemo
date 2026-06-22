
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Material.Icons;


namespace AvaloniaTestDemo.Views;

public partial class TestViewModel() : DemoPageBase("Test", MaterialIconKind.TestTube, 100)
{
    [RelayCommand]
    private void Test()
    {
        var array = new[] { 0, 2, 5, 6, 3 };
        
        for (int i = 0; i < array.Length - 1; i++)
        {
            if (array[i] < 4)
            {
                array.Remove([array[i]]);
            }
        }
    }
    
    
}
