namespace Ci.Template.Library.ViewModels
{
    using System.Collections.Generic;

    using Ci.Template.Library.Models;

    /// <summary>
    /// 角色與選單列表
    /// </summary>
    public class RoleViewModel
    {
        public Role Role { get; set; }

        public List<TreeViewModel> MenuCheckList { get; set; } 
    }

}
