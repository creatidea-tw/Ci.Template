namespace Ci.Template.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Helpers;

    using Ci.Template.Library.Commons;
    using Ci.Template.Library.Models;
    using Ci.Template.Library.ViewModels;

    using Creatidea.Library.Results;

    using X.PagedList;

    /// <summary>
    /// 登入驗證、新增修改帳號
    /// </summary>
    public class AccountService : BaseService
    {
        public int PageSize = 10;
        private const string Salt = "8557F449-E554-4147-BCD1-560AA3CFB849";

        /// <summary>
        /// 登入-驗證帳號密碼
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="password">The password.</param>
        /// <param name="ip">The ip.</param>
        /// <returns>userId</returns>
        public Admin CheckLogin(string account, string password, string ip = "")
        {
            var passHash = Crypto.SHA256(password + Salt);
            var admin = Db.Admins.FirstOrDefault(x => x.Account == account && x.Password == passHash && x.IsDelete == false);

            // 更新登入時間
            if (admin != null)
            {
                admin.LastLoginTime = DateTime.Now;
                admin.LastLoginIp = ip;

                Db.SaveChanges();
            }

            return admin;
        }

        /// <summary>
        /// 使用者是否存在
        /// </summary>
        /// <param name="account">欲檢查之帳號.</param>
        /// <param name="id">操作者本身Id.</param>
        /// <returns>bool</returns>
        public bool CheckName(string account, Guid? id)
        {
            IQueryable<Admin> admin = Db.Admins;

            // 排除自己
            if (id != null)
            {
                admin = admin.Where(x => x.Id != id);
                // 查無帳號
                if (Db.Admins.Find(id) == null)
                {
                    return true;
                }
            }

            bool result = (admin.FirstOrDefault(x => x.Account == account)) != null;

            return result;
        }

        /// <summary>
        /// 比對密碼是否正確
        /// </summary>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool CheckPassword(string oldPassword, string password)
        {
            var passHash = Crypto.SHA256(password + Salt);

            return (oldPassword == passHash);
        }

        /// <summary>
        /// 取得帳號列表
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>list Admins</returns>
        public IPagedList<Admin> GetList(int page)
        {
            IQueryable<Admin> data = Db.Admins.Where(x => x.IsDelete == false).OrderBy(x => x.CreateTime);

            // TODO 普通使用者看不到超級管理員
            //if (!UserHelper.IsSuperManager)
            //{
            //    data = data.Where(x => x.Id != Guid.Parse(Keys.SuperManager));
            //}

            var paged = data.ToPagedList(page, PageSize);

            return paged;
        }

        /// <summary>
        /// 取得單筆帳號
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Admins</returns>
        public Admin GetById(Guid id)
        {
            Admin data = Db.Admins.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
            return data;
        }

        /// <summary>
        /// 取得帳號所有角色
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public List<RoleCheck> GetRoleByAdminId(Guid? id)
        {
            IEnumerable<Role> data = null;
            if (id != null)
            {
                // 帳號所有角色
                data = Db.Admins.Find(id).Roles.Where(x => x.IsDelete == false).ToList();
            }

            var roleList = Db.Roles.Where(x => x.IsDelete == false).OrderBy(x => x.Sort);
            var checkList = new List<RoleCheck>();

            // 全部角色
            foreach (var role in roleList)
            {
                var check = new RoleCheck
                {
                    Id = role.Id,
                    Name = role.Name,
                    // 帳號是否包含角色
                    IsChecked = (id != null) && data.Any(x => x.Id == role.Id)
                };
                checkList.Add(check);
            }

            return checkList;
        }

        /// <summary>
        /// 新增帳號
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="password">The password.</param>
        /// <returns>Admin id</returns>
        public CiResult<Guid> DbCreate(string account, string password)
        {
            CiResult<Guid> ciResult = new CiResult<Guid>();

            try
            {
                var exist = CheckName(account, null);
                if (!exist)
                {
                    var passHash = Crypto.SHA256(password + Salt);
                    var data = new Admin
                    {
                        Id = Guid.NewGuid(),
                        Account = account,
                        Password = passHash,
                        CreateTime = DateTime.Now,
                        IsDelete = false
                    };
                    Db.Admins.Add(data);
                    Db.SaveChanges();

                    ciResult.Data = data.Id;
                    ciResult.Message = string.Format("[{0}]帳號新增成功。", data.Account);
                    ciResult.ReturnResult = ReturnResult.Success;
                }
                else
                {
                    ciResult.Message = "帳號已被註冊。";
                }
            }
            catch (Exception)
            {
                ciResult.Message = "帳號新增失敗。";
            }

            return ciResult;
        }

        /// <summary>
        /// 修改密碼
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="password">The password.</param>
        /// <returns>是否執行成功</returns>
        public CiResult DbUpdatePassword(Guid id, string password)
        {
            CiResult ciResult = new CiResult();

            var passHash = Crypto.SHA256(password + Salt);
            Admin data = Db.Admins.Find(id);

            if (data != null)
            {
                try
                {
                    data.Password = passHash;
                    Db.SaveChanges();

                    ciResult.Message = string.Format("[{0}]密碼修改成功。", data.Account);
                    ciResult.ReturnResult = ReturnResult.Success;
                }
                catch (Exception)
                {
                    ciResult.Message = "密碼修改失敗。";
                }
            }
            else
            {
                ciResult.Message = "帳號不存在，無法修改。";
            }

            return ciResult;
        }

        /// <summary>
        /// 修改帳號
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="account">The account.</param>
        /// <returns> 是否執行成功 </returns>
        public CiResult DbUpdateAccount(Guid id, string account)
        {
            CiResult ciResult = new CiResult();

            Admin data = Db.Admins.Find(id);

            if (data != null)
            {
                try
                {
                    var exist = CheckName(account, id);
                    if (!exist)
                    {
                        data.Account = account;
                        Db.SaveChanges();

                        ciResult.Message = string.Format("[{0}]帳號修改成功。", data.Account);
                        ciResult.ReturnResult = ReturnResult.Success;
                    }
                    else
                    {
                        ciResult.Message = "帳號已被註冊。";
                    }
                }
                catch (Exception)
                {
                    ciResult.Message = "帳號修改失敗。";
                }
            }
            else
            {
                ciResult.Message = "帳號不存在，無法修改。";
            }

            return ciResult;
        }

        /// <summary>
        /// 修改帳號角色
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="roleCheck">The role check.</param>
        /// <returns></returns>
        public CiResult DbUpdateAdminRoles(Guid id, List<RoleCheck> roleCheck)
        {
            CiResult ciResult = new CiResult();

            var data = Db.Admins.Find(id);
            if (data != null)
            {
                try
                {
                    data.Roles.Clear();

                    foreach (var c in roleCheck)
                    {
                        if (c.IsChecked)
                        {
                            var role = Db.Roles.Find(c.Id);
                            data.Roles.Add(role);
                        }
                    }

                    Db.SaveChanges();

                    ciResult.ReturnResult = ReturnResult.Success;
                }
                catch (Exception)
                {
                    ciResult.Message = "角色修改失敗。";
                }

            }
            else
            {
                ciResult.Message = "帳號不存在，無法修改。";
            }
            return ciResult;
        }

        /// <summary>
        /// 刪除帳號
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否執行成功</returns>
        public CiResult DbDelete(Guid id)
        {
            CiResult ciResult = new CiResult();

            Admin data = Db.Admins.Find(id);

            if (data != null)
            {
                data.DeleteTime = DateTime.UtcNow;
                data.IsDelete = true;
                Db.SaveChanges();

                ciResult.Message = string.Format("[{0}]刪除成功。", data.Account);
                ciResult.ReturnResult = ReturnResult.Success;
            }
            else
            {
                ciResult.Message = "帳號不存在，無法刪除";
            }

            return ciResult;
        }

    }
}
