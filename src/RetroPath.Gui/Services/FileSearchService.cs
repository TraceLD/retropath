using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;
using RetroPath.Gui.Views;

namespace RetroPath.Gui.Services
{
    public class FileSearchService
    {
        public async Task<string?> GetCsvFile(string? starterDirPath = null) => await GetFile(FileFilters.CsvFilter, starterDirPath);

        public async Task<string?> GetJsonFile(string? starterDirPath = null) => await GetFile(FileFilters.JsonFilter, starterDirPath);

        public async Task<string?> GetFile(FileDialogFilter filter, string? starterDirPath = null)
        {
            var dlg = new OpenFileDialog
            {
                AllowMultiple = false
            };

            if (starterDirPath is not null)
            {
                dlg.Directory = starterDirPath;
            }
            
            dlg.Filters.Add(filter);

            var result = await dlg.ShowAsync(WindowHelpers.CurrentDesktopWindow!);

            return result?[0];
        }

        public async Task<string?> GetDir(string? starterDirPath = null)
        {
            OpenFolderDialog dlg = starterDirPath is null ? new() : new() {Directory = starterDirPath};

            var result = await dlg.ShowAsync(WindowHelpers.CurrentDesktopWindow!);

            return result;
        }

        public async Task<string?> GetSaveJsonFile(string? starterDirPath = null)
        {
            var jsonFilter = FileFilters.JsonFilter;
            var dlg = new SaveFileDialog
            {
                Filters = new() {jsonFilter},
                DefaultExtension = jsonFilter.Extensions[0],
                InitialFileName = "configuration"
            };

            if (starterDirPath is not null)
            {
                dlg.Directory = starterDirPath;
            }

            var result = await dlg.ShowAsync(WindowHelpers.CurrentDesktopWindow!);

            return result;
        }
    }
}
