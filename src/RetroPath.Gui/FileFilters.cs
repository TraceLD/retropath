using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroPath.Gui
{
    public static class FileFilters
    {
        public static FileDialogFilter CsvFilter = new() {Name = "CSV Files", Extensions = {"csv"}};
        public static FileDialogFilter JsonFilter = new() { Name = "JSON Files", Extensions = {"json"}};
    }
}
