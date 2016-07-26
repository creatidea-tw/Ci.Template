namespace $safeprojectname$.ViewModels
{
    using System.Collections.Generic;

    using $safeprojectname$.Models;

    /// <summary>
    /// 角色與選單列表
    /// </summary>
    public class RoleViewModel
    {
        public Role Role { get; set; }

        public List<TreeViewModel> MenuCheckList { get; set; } 
    }

}
