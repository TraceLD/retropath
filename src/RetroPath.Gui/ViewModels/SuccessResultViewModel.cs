using RetroPath.Gui.Models;

namespace RetroPath.Gui.ViewModels;

public class SuccessResultViewModel : ViewModelBase
{
    public string Message { get; }

    public SuccessResultViewModel(AppSpecification appSpecification)
        => Message = $"Results have been written to {appSpecification.OutputConfiguration.OutputDir}.";
}