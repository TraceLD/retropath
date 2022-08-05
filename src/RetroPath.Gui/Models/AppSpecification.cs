using RetroPath.Core.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroPath.Gui.Models
{
    public class AppSpecification
    {
        public InputConfiguration InputConfiguration { get; set; }
        public OutputConfiguration OutputConfiguration { get; set; }
    }
}
