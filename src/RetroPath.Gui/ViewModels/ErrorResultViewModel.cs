using System;
using RetroPath.Gui.Models;

namespace RetroPath.Gui.ViewModels;

public class ErrorResultViewModel : ViewModelBase
{
    public string ErrorMsg { get; }
    public string ErrorDetailsMsg { get; }
    
    public ErrorResultViewModel(RunResult errResult)
    {
        if (errResult.IsSuccess)
        {
            throw new Exception("Error screen should never be reached if run was successful.");
        }
        
        ErrorMsg = $"Error name: {errResult.Exception?.GetType().ToString() ?? "Exception"}";
        ErrorDetailsMsg = errResult.Exception?.StackTrace ?? "No stack trace available.";
    }
}