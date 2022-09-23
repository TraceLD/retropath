using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia.Models;
using ReactiveUI;
using RetroPath.Gui.Models;
using Serilog;

namespace RetroPath.Gui.ViewModels;

public class RunViewModel : ViewModelBase
{
    private readonly AppSpecification _appSpecification;

    #region Main Progress Bar

    private double _mainProgressBarValue;
    public double MainProgressBarValue
    {
        get => _mainProgressBarValue;
        set => this.RaiseAndSetIfChanged(ref _mainProgressBarValue, value);
    }
    
    private string _mainProgressBarText = null!;
    public string MainProgressBarText
    {
        get => _mainProgressBarText;
        set => this.RaiseAndSetIfChanged(ref _mainProgressBarText, value);
    }

    private void MainProgressBarIncrementPreparedOutputDir() => MainProgressBarValue++;
    private void MainProgressBarIncrementParsedInputs() => MainProgressBarValue += 9;

    private void MainProgressBarIncrementProcessedSources() => MainProgressBarValue += 70; 

    #endregion

    #region Source Progress Bar

    private double _sourceProgressBarValue;
    public double SourceProgressBarValue
    {
        get => _sourceProgressBarValue;
        set => this.RaiseAndSetIfChanged(ref _sourceProgressBarValue, value);
    }
    
    private string _sourceProgressBarText = null!;
    public string SourceProgressBarText
    {
        get => _sourceProgressBarText;
        set => this.RaiseAndSetIfChanged(ref _sourceProgressBarText, value);
    }
    
    private bool _sourceProgressBarIsVisible;
    public bool SourceProgressBarIsVisible
    {
        get => _sourceProgressBarIsVisible;
        set => this.RaiseAndSetIfChanged(ref _sourceProgressBarIsVisible, value);
    }

    private void SourceProgressBarIncrementSourceProcessed(int sourcesCount)
        => SourceProgressBarValue = (int) SourceProgressBarValue == 10
            ? Math.Min(100, (double) 100 / sourcesCount)
            : Math.Min(100, SourceProgressBarValue + (double) 100 / sourcesCount);

    #endregion

    #region Iteration Progress Bar

    private string _iterationProgressBarText = null!;
    public string IterationProgressBarText
    {
        get => _iterationProgressBarText;
        set => this.RaiseAndSetIfChanged(ref _iterationProgressBarText, value);
    }
    
    private bool _iterationProgressBarIsVisible;
    public bool IterationProgressBarIsVisible
    {
        get => _iterationProgressBarIsVisible;
        set => this.RaiseAndSetIfChanged(ref _iterationProgressBarIsVisible, value);
    }

    #endregion
    
    
    public RunViewModel(AppSpecification appSpecification)
    {
        _appSpecification = appSpecification;
        
        MainProgressBarValue = 0;
        MainProgressBarText = "Main progress: Preparing output directory...";
        
        SourceProgressBarValue = 0;
        SourceProgressBarText = "Sources progress";
        SourceProgressBarIsVisible = false;
        
        IterationProgressBarText = "Iteration progress";
        SourceProgressBarIsVisible = false;
    }

    public async Task<RunResult> Run()
    {
        try
        {
            using var rp = new RetroPath.Core.RetroPath(_appSpecification.InputConfiguration,
                _appSpecification.OutputConfiguration);

            rp.PrepareOutputDir();
            MainProgressBarIncrementPreparedOutputDir();

            MainProgressBarText = "Main progress: Parsing inputs...";
            await rp.ParseInputsAsync().ConfigureAwait(false);
            MainProgressBarIncrementParsedInputs();

            // TODO: compatible with multiple sources;
            MainProgressBarText = "Main progress: Running RP2.0 for sources...";
            SourceProgressBarText = "Sources progress: Processing source (1/1)...";
            SourceProgressBarValue = 10;
            SourceProgressBarIsVisible = true;
            IterationProgressBarText = "Current source progress: Firing rules...";
            IterationProgressBarIsVisible = true;
            await Task.Run(() => rp.Compute());
            SourceProgressBarIncrementSourceProcessed(1);
            MainProgressBarIncrementProcessedSources();
            IterationProgressBarIsVisible = false;

            MainProgressBarText = "Main progress: Writing results to CSV...";
            await rp.WriteResultsToCsvAsync();
            MainProgressBarValue = 100;
            
            return new RunResult();
        }
        catch (Exception e)
        {
            //await Dispatcher.UIThread.InvokeAsync(async () => await HandleExceptionWhileRunning(e), DispatcherPriority.MaxValue);
            
            return new RunResult(e);
        }
    }

    private async Task HandleExceptionWhileRunning(Exception e)
    {
        var errorWindow = MessageBox.Avalonia.MessageBoxManager
            .GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                ContentTitle = $"Error: {e.GetType()}",
                ContentMessage =
                    $"The following error has occurred while running RP2.0 with provided inputs:\n{e.Message}",
                ButtonDefinitions = ButtonEnum.Ok,
                ShowInCenter = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                SizeToContent = SizeToContent.WidthAndHeight,
                Icon = Icon.Error
            });

        Log.Error(e, "Error while running RP2.0 on provided input configuration");

        await errorWindow.ShowDialog(WindowHelpers.CurrentDesktopWindow);
        
        Environment.Exit(2);
    }
}