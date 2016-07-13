using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ci.Template.Library.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum MenuType
    {
        [Display(Name = "後台選單")]
        BackStage = 0,

        [Display(Name = "前台選單")]
        FrontStage = 1
    }

    public enum MenuChoose
    {
        All = 0,
        Menu = 1,
        RoleCheck = 2
    }
}
