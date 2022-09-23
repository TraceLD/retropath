using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RetroPath.Gui.Views;

public partial class RunView : UserControl
{
    public RunView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}