using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$.ViewModels
{
    public class TreeViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string IsMenu { get; set; }

        public string Url { get; set; }

        public bool IsChecked { get; set; }

        public List<TreeLang> TreeLangList { get; set; }

        public List<TreeViewModel> Nodes { get; set; }
    }

    public class TreeLang
    {
        public int Lang { get; set; }

        public int Status { get; set; }
    }
}
