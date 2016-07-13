using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ci.Template.Service
{
    using System.Data.Entity;
    using System.Web;
    using System.Web.Mvc;

    using Ci.Template.Library.Enums;
    using Ci.Template.Library.Extensions;
    using Ci.Template.Library.Models;
    using Ci.Template.Library.ViewModels;

    using Creatidea.Library.Results;

    public class MenuService : BaseService
    {
        /// <summary>
        /// 預設類別
        /// </summary>
        public const int DefaultType = (int)MenuType.BackStage;

        /// <summary>
        /// 尋找所有節點(Menu編輯側邊樹、Menu parentId下拉選單、後台選單、Role checkbox)
        /// </summary>
        /// <param name="parentMenu">The parent menu.</param>
        /// <param name="type">Enum:後台、前台</param>
        /// <param name="chooseMenu">Enum:全部、menu、包含角色</param>
        /// <param name="menuCheck">某角色所包含的選單</param>
        /// <returns></returns>
        public List<TreeViewModel> GetTrees(Menu parentMenu, MenuType type, MenuChoose chooseMenu = 0, IEnumerable<Menu> menuCheck = null)
        {
            IEnumerable<Menu> menuData;

            // 第一層:pareintId null
            if (parentMenu == null)
            {
                menuData =
                    Db.Menus.Where(x => x.ParentId == null && x.Type == (int)type && x.IsDelete == false)
                        .OrderBy(x => x.Sort)
                        .ToList();

                #region "取得使用者包含Menu"

                //if (chooseMenu == MenuChoose.Menu && roleMenu == null)
                //{
                //    roleMenu = Db.Admins.Find(UserHelper.Id).Roles.Where(x => x.IsDelete == false)
                //                                            .SelectMany(x => x.Menus).Where(x => x.IsDelete == false)
                //                                            .Distinct().ToList();
                //}

                #endregion
            }
            else
            {
                menuData = parentMenu.ChildMenus.Where(x => x.IsDelete == false).OrderBy(x => x.Sort).ToList();
            }

            // 篩選Menu(userMenu包含使用者擁有權限的menu)
            if (chooseMenu == MenuChoose.Menu)
            {
                menuData = menuData.Where(x => x.IsMenu);

                List<Guid> userMenu = UserHelper.UserMenu;

                if (UserHelper.IsSuperManager == false) //&& userMenu != null
                {
                    menuData = menuData.Where(x => userMenu.Contains(x.Id));
                }// super管理員的Menu、設定角色全限時 不用篩選menu
            }

            var tree = new List<TreeViewModel>();
            foreach (var menu in menuData)
            {
                var node = new TreeViewModel();
                node.Id = menu.Id;
                node.Name = menu.NativeName;
                node.IsMenu = menu.IsMenu ? "true" : "false";

                UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
                node.Url = url.Action(menu.Action, menu.Controller) + menu.Url.FieldToString(); //Keys.AdminRoot +

                // 角色是否包含選單
                node.IsChecked = (menuCheck != null) && menuCheck.Any(x => x.Id == menu.Id);

                // 若有子層繼續遞迴
                #region "遞迴"
                var child = Db.Menus.Where(x => x.ParentId == menu.Id && x.IsDelete == false);
                if (chooseMenu == MenuChoose.Menu) { child = child.Where(x => x.IsMenu); }
                int childCount = child.Count();
                if (childCount > 0)
                {
                    node.Nodes = GetTrees(menu, type, chooseMenu, menuCheck);
                }
                #endregion

                tree.Add(node);
            }

            return tree;
        }

        /// <summary>
        /// 取得選單(前台)
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="lang">The language.</param>
        /// <param name="routeUrl">{museum}/{language}</param>
        /// <returns></returns>
        public List<TreeViewModel> GetFrontMenu(int type, int lang, string routeUrl = "")
        {
            IEnumerable<Menu> menuData =
                Db.Menus.Where(x => x.ParentId == null && x.Type == type && x.IsDelete == false && x.IsMenu)
                    .OrderBy(x => x.Sort)
                    .ToList();

            if (!string.IsNullOrEmpty(routeUrl))
            {
                routeUrl = "/" + routeUrl;
            }

            var tree = new List<TreeViewModel>();
            foreach (var menu in menuData)
            {
                var node = new TreeViewModel();
                node.Id = menu.Id;
                node.Url = !string.IsNullOrEmpty(menu.Url) ? menu.Url : (routeUrl + "/" + menu.Controller + "/" + menu.Action);
                node.Name = menu.NativeName;
                tree.Add(node);
            }

            return tree;
        }

        /// <summary>
        /// 取得單筆資料
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Menu GetById(Guid id)
        {
            Menu data = Db.Menus.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
            return data;
        }

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public CiResult<Guid> Create(Menu model)
        {
            CiResult<Guid> ciResult = new CiResult<Guid>();

            //var maxSort = Db.Menus.Where(x => x.ParentId == model.ParentId).OrderByDescending(x => x.Sort).FirstOrDefault();
            //int sort = 0;
            //if (maxSort != null)
            //{
            //    sort = maxSort.Sort + 3;
            //}

            try
            {
                var data = new Menu
                {
                    Id = Guid.NewGuid(),
                    NativeName = model.NativeName.ToTrim(),
                    Controller = model.Controller.ToTrim(), // RoleName=Controler+Action
                    Action = model.Action.ToTrim(),
                    Description = model.Description,
                    Url = model.Url.ToTrim(),
                    Type = model.Type,
                    IsMenu = model.IsMenu,
                    Sort = model.Sort, // sort,
                    IsDelete = false
                };

                // 第二層
                if (model.ParentId != null)
                {
                    var parent = Db.Menus.Find(model.ParentId);
                    if (parent != null)
                    {
                        data.ParentId = model.ParentId;
                    }
                    else
                    {
                        ciResult.Message = string.Format("父層[{0}]不存在", model.ParentId);
                    }
                }
                Db.Menus.Add(data);
                Db.SaveChanges();

                ciResult.Data = data.Id;
                ciResult.ReturnResult = ReturnResult.Success;

            }
            catch (Exception)
            {
                ciResult.Message = string.Format("[{0}]建立失敗。", model.NativeName);
            }

            return ciResult;
        }

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public CiResult Update(Menu model)
        {
            CiResult ciResult = new CiResult();

            try
            {
                var data = Db.Menus.Find(model.Id);

                data.NativeName = model.NativeName.ToTrim();
                data.Controller = model.Controller.ToTrim();
                data.Action = model.Action.ToTrim();
                data.Description = model.Description;
                data.Url = model.Url.ToTrim();
                data.IsMenu = model.IsMenu;
                data.Sort = model.Sort;

                // 第二層
                if (model.ParentId != null)
                {
                    var parent = Db.Menus.Find(model.ParentId);
                    if (parent != null)
                    {
                        data.ParentId = model.ParentId;
                    }
                    else
                    {
                        ciResult.Message = string.Format("父層[{0}]不存在", model.ParentId);
                    }
                }
                else
                {
                    data.ParentId = null;
                }

                Db.SaveChanges();

                ciResult.Message = string.Format("[{0}]修改成功。", model.NativeName);
                ciResult.ReturnResult = ReturnResult.Success;
            }
            catch (Exception)
            {
                ciResult.Message = string.Format("[{0}]修改失敗。", model.NativeName);
            }

            return ciResult;
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否執行成功</returns>
        public CiResult Delete(Guid id)
        {
            CiResult ciResult = new CiResult();

            Menu data = Db.Menus.Find(id);

            if (data != null)
            {
                data.IsDelete = true;
                Db.SaveChanges();

                ciResult.Message = string.Format("[{0}]刪除成功。", data.NativeName);
                ciResult.ReturnResult = ReturnResult.Success;
            }
            else
            {
                ciResult.Message = "資料不存在，無法刪除。";
            }

            return ciResult;
        }

        /// <summary>
        /// 取得選單名稱 (前台)
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="lang">The language.</param>
        /// <returns></returns>
        public string GetNameByController(string controllerName, int lang)
        {
            string data =
                Db.Menus.FirstOrDefault(
                    x =>
                    x.Controller == controllerName.Trim() && x.IsDelete == false && x.Type == (int)MenuType.FrontStage)?
                    .NativeName;

            return data;
        }
    }
}
