using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RetroPath.Gui.Views;

public partial class ErrorResultView : UserControl
{
    public ErrorResultView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}