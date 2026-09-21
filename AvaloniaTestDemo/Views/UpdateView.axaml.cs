using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaTestDemo.Views;

public partial class UpdateView : UserControl
{
    public UpdateView()
    {
        InitializeComponent();
    }
    
    private void LogEditor_OnTextChanged(object? sender, EventArgs e)
    {
        logEditor.CaretOffset = logEditor.Document.TextLength;
        logEditor.TextArea.Caret.BringCaretToView();
        
    }
}