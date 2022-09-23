using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RetroPath.Gui.Views;

public partial class SuccessResultView : UserControl
{
    public SuccessResultView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}