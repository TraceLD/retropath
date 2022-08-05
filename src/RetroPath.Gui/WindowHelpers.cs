using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace RetroPath.Gui;

public static class WindowHelpers
{
    public static Window? CurrentDesktopWindow
        => Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopApp
            ? desktopApp.MainWindow
            : default;
}