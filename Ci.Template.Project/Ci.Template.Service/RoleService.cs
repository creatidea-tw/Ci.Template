using System;
using System.Collections.Generic;
using System.Linq;
using Ci.Template.Library.Models;
using Ci.Template.Library.Commons;
using System.Web.Helpers;
using Ci.Template.Library.ViewModels;
using Ci.Template.Library.Enums;
using Ci.Template.Library.Extensions;
using Creatidea.Library.Results;
using X.PagedList;

namespace $safeprojectname$
{
    /// <summary>
    /// 角色管理
    /// </summary>
    public class RoleService : BaseService
    {
        public int PageSize = 20;

        /// <summary>
        /// 取得資料列表
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>list Roles</returns>
        public IPagedList<Role> GetList(int page = 1)
        {
            var datas = Db.Roles.Where(x => x.IsDelete == false).OrderBy(x => x.Sort);
            var paged = datas.ToPagedList(page, PageSize);

            return paged;
        }

        /// <summary>
        /// 取得單筆資料
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Admins</returns>
        public Role GetById(Guid id)
        {
            Role data = Db.Roles.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
            return data;
        }

        /// <summary>
        /// 取得角色所有選單(後台)
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public List<TreeViewModel> GetMenuByRoleId(Guid? id)
        {
            IEnumerable<Menu> data = null;
            if (id != null)
            {
                // 角色所有選單
                data = Db.Roles.Find(id).Menus.Where(x => x.IsDelete == false).ToList();
            }

            MenuService menuService = new MenuService { };
            var tree = menuService.GetTrees(null, (int)MenuType.BackStage, MenuChoose.RoleCheck, data);
            
            return tree;
        }


        /// <summary>
        /// 新增資料
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>RoleId</returns>
        public CiResult<Guid> DbCreate(Role model)
        {
            CiResult<Guid> ciResult = new CiResult<Guid>();

            try
            {
                var data = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Sort = model.Sort,
                    IsDelete = false
                };
                Db.Roles.Add(data);
                Db.SaveChanges();

                ciResult.Data = data.Id;
                ciResult.Message = string.Format("[{0}]新增成功。", data.Name);
                ciResult.ReturnResult = ReturnResult.Success;
            }
            catch (Exception)
            {
                ciResult.Message = "新增失敗。";
            }

            return ciResult;
        }

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public CiResult DbUpdate(Role model)
        {
            CiResult ciResult = new CiResult();

            try
            {
                var data = Db.Roles.Find(model.Id);

                data.Name = model.Name.ToTrim();
                data.Sort = model.Sort;

                Db.SaveChanges();

                ciResult.Message = string.Format("[{0}]修改成功。", model.Name);
                ciResult.ReturnResult = ReturnResult.Success;
            }
            catch (Exception)
            {
                ciResult.Message = string.Format("[{0}]修改失敗。", model.Name);
            }

            return ciResult;
        }

        /// <summary>
        /// 修改角色選單
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="menuCheck">The menu check.</param>
        /// <returns></returns>
        public CiResult DbUpdateRoleMenus(Guid id, List<TreeViewModel> menuCheck)
        {
            CiResult ciResult = new CiResult();

            // treeview reindex 無遞迴
            var data = Db.Roles.Find(id);
            if (data != null)
            {
                try
                {
                    data.Menus.Clear();

                    foreach (var c in menuCheck)
                    {
                        if (c.IsChecked)
                        {
                            var menu = Db.Menus.Find(c.Id);
                            data.Menus.Add(menu);
                        }
                    }

                    Db.SaveChanges();

                    ciResult.ReturnResult = ReturnResult.Success;
                }
                catch (Exception)
                {
                    ciResult.Message = "選單修改失敗。";
                }

            }
            else
            {
                ciResult.Message = "角色不存在，無法修改。";
            }
            return ciResult;
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否執行成功</returns>
        public CiResult DbDelete(Guid id)
        {
            CiResult ciResult = new CiResult();

            Role data = Db.Roles.Find(id);

            if (data != null)
            {
                data.IsDelete = true;
                Db.SaveChanges();

                ciResult.Message = string.Format("[{0}]刪除成功。", data.Name);
                ciResult.ReturnResult = ReturnResult.Success;
            }
            else
            {
                ciResult.Message = "角色不存在，無法刪除";
            }

            return ciResult;
        }
    }
}
