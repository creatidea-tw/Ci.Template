namespace $safeprojectname$.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 帳號與角色列表
    /// </summary>
    public class AccountViewModel
    {
        public AccountView AccountView { get; set; }

        public List<RoleCheck> RoleCheckList { get; set; }
    }

    public class AccountView
    {
        public Guid Id { get; set; }

        [Required]
        public string Account { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

    }

    public class RoleCheck
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }

    /// <summary>
    /// 登入
    /// </summary>
    public class LoginViewModel
    {
        [Required]
        public string Account { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    /// <summary>
    /// 修改密碼
    /// </summary>
    public class PasswordViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [DataType(DataType.Password)]
        public string OldPassword {get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "兩次密碼輸入不一致")]
        public string PasswordConfirm { get; set; }
    }
}
