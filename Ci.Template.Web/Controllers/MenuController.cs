using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ci.Template.Web.Controllers
{
    using Ci.Template.Library.Enums;
    using Ci.Template.Library.Models;
    using Ci.Template.Library.ViewModels;
    using Ci.Template.Service;

    using Creatidea.Library.Results;

    public class MenuController : Controller
    {
        private MenuService menuService = new MenuService{ };

        /// <summary>
        /// 選單列表
        /// </summary>
        /// <param name="typeOpt">The type opt.</param>
        /// <param name="currentId">The current identifier.</param>
        /// <returns></returns>
        public ActionResult Index(MenuType typeOpt = MenuService.DefaultType, Guid? currentId = null)
        {
            ViewBag.TypeOpt = typeOpt;
            ViewBag.CurrentId = currentId;
            return View();
        }

        /// <summary>
        /// 新增選單
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(Guid? parentId, MenuType typeOpt = MenuService.DefaultType)
        {
            var menu = new Menu()
            {
                Type = (int)typeOpt
            };
            ViewBag.infoList = GetSelect(null, parentId, typeOpt);
            ViewBag.TypeOpt = typeOpt;
            ViewBag.CurrentId = parentId;
            return View(menu);
        }

        ///<summary>
        /// 新增選單post
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Menu model)
        {
            if (ModelState.IsValid)
            {
                CiResult<Guid> result = menuService.Create(model);
                TempData["alert"] = result.Message;

                if (result.ReturnResult == ReturnResult.Success)
                {
                    return RedirectToAction("Index", new { typeOpt = model.Type, currentId = result.Data });
                }
            }


            // 驗證失敗錯誤訊息
            var errors = ModelState.Keys.SelectMany(key => ModelState[key].Errors);
            foreach (var err in errors)
            {
                TempData["alert"] += err.ErrorMessage;
            }

            ViewBag.infoList = GetSelect(null, model.ParentId, (MenuType)model.Type);
            ViewBag.TypeOpt = model.Type;
            return View(model);
        }

        /// <summary>
        /// 編輯選單
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="typeOpt">The type opt.</param>
        /// <returns></returns>
        public ActionResult Update(Guid id, MenuType typeOpt)
        {
            var data = menuService.GetById(id);
            if (data == null)
                return HttpNotFound();

            ViewBag.infoList = GetSelect(id, data.ParentId, typeOpt);
            ViewBag.TypeOpt = typeOpt;
            ViewBag.CurrentId = id;
            return View(data);
        }

        /// <summary>
        /// 編輯選單post
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Menu model)
        {
            if (ModelState.IsValid)
            {
                CiResult result = menuService.Update(model);
                TempData["alert"] = result.Message;

                if (result.ReturnResult == ReturnResult.Success)
                {
                    return RedirectToAction("Index", new { typeOpt = model.Type, currentId = model.Id });
                }
            }

            // 驗證失敗錯誤訊息
            var errors = ModelState.Keys.SelectMany(key => ModelState[key].Errors);
            foreach (var err in errors)
            {
                TempData["alert"] += err.ErrorMessage;
            }

            ViewBag.infoList = GetSelect(model.Id, model.ParentId, (MenuType)model.Type);
            ViewBag.CurrentId = model.Id;
            return View(model);
        }

        /// <summary>
        /// 刪除選單
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="typeOpt">The type opt.</param>
        /// <returns></returns>
        public ActionResult Delete(Guid id, MenuType typeOpt = MenuService.DefaultType)
        {
            CiResult result = menuService.Delete(id);
            TempData["alert"] = result.Message;

            return RedirectToAction("Index", new { typeOpt });
        }

        #region tree

        /// <summary>
        /// tree view
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult _Tree(MenuType typeOpt = MenuService.DefaultType, Guid? currentId = null)
        {
            var tree = menuService.GetTrees(null, typeOpt);
            ViewBag.TypeOpt = typeOpt;
            ViewBag.CurrentId = currentId;
            return PartialView(tree);
        }

        /// <summary>
        /// tree menu
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult _TreeMenu(MenuType typeOpt)
        {
            if (User.Identity.IsAuthenticated)
            {
                var tree = menuService.GetTrees(null, typeOpt, MenuChoose.Menu);

                //MuseumService museumService = new MuseumService { GlobalMuseumId = UserMuseumId };
                //var museum = museumService.GetMuseumLang(UserMuseumId, Keys.DefaultLanguage);
                //ViewBag.MuseumName = museum?.Name;

                return PartialView(tree);
            }

            return PartialView();

        }

        /// <summary>
        /// 取得下拉選單
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <param name="typeOpt">The type opt.</param>
        /// <returns></returns>
        private List<SelectListItem> GetSelect(Guid? id, Guid? parentId, MenuType typeOpt = MenuService.DefaultType)
        {
            var tree = menuService.GetTrees(null, typeOpt);
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "(無上層)", Value = "" });
            selectList.AddRange(GetSelectChild(tree, id, parentId));

            return selectList;
        }


        /// <summary>
        /// 遞迴取得下拉選單項目
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        private List<SelectListItem> GetSelectChild(List<TreeViewModel> nodes, Guid? id, Guid? parentId, int level = 0)
        {
            var selectList = new List<SelectListItem>();
            if (nodes != null)
            {
                foreach (var item in nodes)
                {
                    // 依level加入空白
                    string tab = string.Empty;
                    for (int i = 0; i < level; i++)
                    {
                        tab += "　";
                    }
                    // 排除自己與子項
                    bool disabled = (id == item.Id);

                    selectList.Add(new SelectListItem() { Text = tab + item.Name, Value = item.Id.ToString(), Selected = (parentId == item.Id), Disabled = disabled });
                    if (disabled == false)
                    {
                        selectList.AddRange(GetSelectChild(item.Nodes, id, parentId, level + 1));
                    }
                }
            }

            return selectList;
        }

        #endregion


        protected override void Dispose(bool disposing)
        {
            menuService.Dispose();
            base.Dispose(disposing);
        }
    }
}