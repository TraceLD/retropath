using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RetroPath.Gui.Views
{
    public partial class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
