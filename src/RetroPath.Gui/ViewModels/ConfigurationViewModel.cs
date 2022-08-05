using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using RetroPath.Gui.Models;
using RetroPath.Gui.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Styling;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using RetroPath.Gui.Views;

namespace RetroPath.Gui.ViewModels
{
    public class ConfigurationViewModel : ViewModelBase
    {
        private readonly FileSearchService _fileSearchService;

        private string? _rulesFile;
        public string? RulesFile
        {
            get => _rulesFile;
            set => this.RaiseAndSetIfChanged(ref _rulesFile, value);
        }

        private string? _sourceFile;
        public string? SourceFile
        {
            get => _sourceFile;
            set => this.RaiseAndSetIfChanged(ref _sourceFile, value);
        }

        private string? _sinkFile;
        public string? SinkFile
        {
            get => _sinkFile;
            set => this.RaiseAndSetIfChanged(ref _sinkFile, value);
        }
        
        private string? _cofactorsFile;
        public string? CofactorsFile
        {
            get => _cofactorsFile;
            set => this.RaiseAndSetIfChanged(ref _cofactorsFile, value);
        }
        
        private string _outputDir = null!;
        public string OutputDir
        {
            get => _outputDir;
            set => this.RaiseAndSetIfChanged(ref _outputDir, value);
        }

        private int _pathwayLength = 3;
        public int PathwayLength
        {
            get => _pathwayLength;
            set => this.RaiseAndSetIfChanged(ref _pathwayLength, value);
        }
        
        private int _sourceMw = 1000;
        public int SourceMw
        {
            get => _sourceMw;
            set => this.RaiseAndSetIfChanged(ref _sourceMw, value);
        }
        
        private int _cofactorMw = 1000;
        public int CofactorMw
        {
            get => _cofactorMw;
            set => this.RaiseAndSetIfChanged(ref _cofactorMw, value);
        }
        
        private int _minDiameter = 0;
        public int MinDiameter
        {
            get => _minDiameter;
            set => this.RaiseAndSetIfChanged(ref _minDiameter, value);
        }
        
        private int _maxDiameter = 1000;
        public int MaxDiameter
        {
            get => _maxDiameter;
            set => this.RaiseAndSetIfChanged(ref _maxDiameter, value);
        }
        
        private int _maxStructures = 100;
        public int MaxStructures
        {
            get => _maxStructures;
            set => this.RaiseAndSetIfChanged(ref _maxStructures, value);
        }

        public ReactiveCommand<Unit, Unit> Next { get; }

        public ConfigurationViewModel(FileSearchService fileSearchService)
        {
            _fileSearchService = fileSearchService;
            
            OutputDir = Path.Combine(Directory.GetCurrentDirectory(), "results");

            //TODO: implement proper validation;
            var nextEnabled = this.WhenAnyValue(
                x => x.RulesFile,
                x => !string.IsNullOrWhiteSpace(x));

            Next = ReactiveCommand.Create(
                () => { },
                nextEnabled);
        }

        public AppSpecification GetAppSpecificationFromProps()
            => new()
            {
                InputConfiguration = new(RulesFile!, SinkFile!, SourceFile!, SourceMw, CofactorsFile, CofactorMw,
                    MinDiameter, MaxDiameter, PathwayLength, MaxStructures),
                OutputConfiguration = new(OutputDir)
            };

        public async Task GetRulesFile()
        {
            var res = await _fileSearchService.GetCsvFile();

            if (res is not null)
            {
                RulesFile = res;
            }
        }
        
        public async Task GetSourceFile()
        {
            var res = await _fileSearchService.GetCsvFile();

            if (res is not null)
            {
                SourceFile = res;
            }
        }
        
        public async Task GetSinkFile()
        {
            var res = await _fileSearchService.GetCsvFile();

            if (res is not null)
            {
                SinkFile = res;
            }
        }
        
        public async Task GetCofactorsFile()
        {
            var res = await _fileSearchService.GetCsvFile();

            if (res is not null)
            {
                CofactorsFile = res;
            }
        }

        public async Task GetOutputDir()
        {
            var res = await _fileSearchService.GetDir();

            if (res is not null)
            {
                OutputDir = res;
            }
        }
        
        public async Task LoadConfiguration()
        {
            var path = await _fileSearchService.GetJsonFile(Directory.GetCurrentDirectory());

            if (path is null)
            {
                return;
            }

            var jsonText = await File.ReadAllTextAsync(path);

            AppSpecification? a;
            try
            {
                a = JsonSerializer.Deserialize<AppSpecification>(jsonText);
            }
            catch (JsonException)
            {
                var errorWindow = MessageBox.Avalonia.MessageBoxManager
                    .GetMessageBoxStandardWindow(new MessageBoxStandardParams{
                        ContentTitle = "JSON Error",
                        ContentMessage = "The specified JSON file could not be parsed into a valid app configuration",
                        ButtonDefinitions = ButtonEnum.OkAbort,
                        Icon = Icon.Error
                    });
                
                await errorWindow.Show();
                
                return;
            }

            RulesFile = a!.InputConfiguration.RulesFilePath;
            SinkFile = a.InputConfiguration.SinkFilePath;
            SourceFile = a.InputConfiguration.SourceFilePath;
            SourceMw = (int)a.InputConfiguration.SourceMw;
            CofactorsFile = a.InputConfiguration.CofactorsFilePath;
            CofactorMw = (int)a.InputConfiguration.CofactorMw;
            MinDiameter = a.InputConfiguration.MinDiameter;
            MaxDiameter = a.InputConfiguration.MaxDiameter;
            PathwayLength = a.InputConfiguration.PathwayLength;
            MaxStructures = a.InputConfiguration.MaxStructures;
        }

        public async Task SaveConfiguration()
        {
            var path = await _fileSearchService.GetSaveJsonFile(Directory.GetCurrentDirectory());

            if (path is null)
            {
                return;
            }
            
            var appSpecToSave = GetAppSpecificationFromProps();
            var json = JsonSerializer.Serialize(appSpecToSave, new JsonSerializerOptions {WriteIndented = true});
            await File.WriteAllTextAsync(path, json);
        }
    }
}
