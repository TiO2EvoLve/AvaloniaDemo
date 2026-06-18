using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarmonyLib;
using Material.Icons;

namespace AvaloniaTestDemo.Views;

public partial class HarmonyModViewModel() : DemoPageBase("Harmony Mod", MaterialIconKind.Code, 100)
{
    [ObservableProperty] private string _name = "World";
    [ObservableProperty] private string _result = string.Empty;
    [ObservableProperty] private bool _isPatched = false;

    private readonly Harmony _harmony = new("com.example.avalonia.harmonydemo");

    [RelayCommand]
    private void ShowOriginal()
    {
        var t = new HarmonyDemoTarget();
        Result = t.GetGreeting(Name);
    }
    

    [RelayCommand]
    private void ApplyPatch()
    {
        if (IsPatched)
        {
            Result = "Already patched.";
            return;
        }

        try
        {
            var original = typeof(HarmonyDemoTarget).GetMethod("GetGreeting");
            var postfix = typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.Postfix));

            if (original == null || postfix == null)
            {
                Result = "Failed to locate methods for patching.";
                return;
            }

            _harmony.Patch(original, postfix: new HarmonyMethod(postfix));
            IsPatched = true;
            Result = "Patch applied. 再次调用将看到修改后的结果。";
        }
        catch (Exception ex)
        {
            Result = $"Patch failed: {ex.Message}";
        }
    }

    // 补丁方法必须是公共静态的
    public static class HarmonyPatches
    {
        public static void Postfix(ref string __result)
        {
            __result += " (modded by Harmony)";
        }
    }
}
public class HarmonyDemoTarget
{
    // A simple virtual method we will patch at runtime with Harmony
    public virtual string GetGreeting(string name)
    {
        return $"Hello, {name}!";
    }
}

