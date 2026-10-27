using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using Newtonsoft.Json;

namespace AvaloniaTestDemo.Views;

public partial class SupabaseViewModel() : DemoPageBase("Supabase数据库", MaterialIconKind.SqlQuery, 0)
{
    [ObservableProperty] private UserData selectData;

    [ObservableProperty] public ObservableCollection<UserData> userDatas = new();

    [RelayCommand]
    private async Task Select()
    {
        var client = new SupabaseClient();
        var result = await client.GetCards();
        Console.WriteLine(result);
        var list = JsonConvert.DeserializeObject<List<UserData>>(result);
        if (list is null) return;
        UserDatas.Clear();
        UserDatas = new ObservableCollection<UserData>(list);
    }

    [RelayCommand]
    private async Task Add()
    {
    }

    [RelayCommand]
    private async Task Delete(int id)
    {
    }

    [RelayCommand]
    private async Task Updata(int id)
    {
    }
}

public class SupabaseClient
{
    private readonly HttpClient _client;

    public SupabaseClient()
    {
        _client = new HttpClient();

        _client.DefaultRequestHeaders.Add(
            "apikey",
            "sb_publishable_iitF8TvQmK5SrlpEY9SCLw_UboHzgtd"
        );
        _client.DefaultRequestHeaders.Add(
            "Authorization",
            ""
        );
    }

    public async Task<string> GetCards()
    {
        var url = "https://bukdpuegydylbvzndqou.supabase.co/rest/v1/test?select=*";
        return await _client.GetStringAsync(url);
    }
}

public class UserData
{
    public int id { get; set; }
    public DateTime created_at { get; set; }
    public string name { get; set; }
    public byte age { get; set; }
}