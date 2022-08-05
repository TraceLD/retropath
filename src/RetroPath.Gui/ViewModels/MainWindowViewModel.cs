using System;
using System.Threading.Tasks;
using ReactiveUI;
using RetroPath.Gui.Models;
using RetroPath.Gui.Services;

namespace RetroPath.Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly AppSpecification _appSpecification;
        private readonly FileSearchService _fileSearchService;

        private ViewModelBase? _content;

        public ViewModelBase? Content
        {
            get => _content;
            private set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        public WelcomeViewModel WelcomeVm { get; }

        public MainWindowViewModel(AppSpecification appSpec, FileSearchService fileSearchService)
        {
            _appSpecification = appSpec;
            _fileSearchService = fileSearchService;

            Content = WelcomeVm = new WelcomeViewModel();
        }

        public void GoToConfigurationScreen() => Content = new ConfigurationViewModel(_fileSearchService);

        public async Task Run()
        {
            if (Content is not ConfigurationViewModel cvm)
            {
                return;
            }
            
            var newAppSpec = cvm.GetAppSpecificationFromProps();

            _appSpecification.InputConfiguration = newAppSpec.InputConfiguration;
            _appSpecification.OutputConfiguration = newAppSpec.OutputConfiguration;

            Content = new RunViewModel(_appSpecification);

            await ((RunViewModel) Content).Run();
        }

        public void Cancel() => Environment.Exit(18);

        public void GoToResultsScreen() => throw new NotImplementedException();
    }
}
