using System;
using System.Web.Mvc;
using Creatidea.Library.Results;
using Ci.Template.Library.Enums;
using Ci.Template.Service;
using Ci.Template.Library.ViewModels;

namespace $safeprojectname$.Controllers
{
    /// <summary>
    /// 角色管理
    /// </summary>
    public class RoleController : AuthBaseController
    {
        private RoleService roleService = new RoleService {};

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var data = roleService.GetList();
            return View(data);
        }


        /// <summary>
        /// 新增角色
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var viewModel = new RoleViewModel
            {
                MenuCheckList = roleService.GetMenuByRoleId(null)
            };

            return View(viewModel);
        }

        /// <summary>
        /// 新增角色post
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoleViewModel model)
        {
            // 新增角色
            CiResult<Guid> result = roleService.DbCreate(model.Role);
            TempData["alert"] = result.Message;

            if (result.ReturnResult == ReturnResult.Success)
            {
                Guid roleId = result.Data;
                // 新增選單list
                CiResult resultMenu = roleService.DbUpdateRoleMenus(roleId, model.MenuCheckList);
                TempData["alert"] += resultMenu.Message;
                if (resultMenu.ReturnResult == ReturnResult.Success)
                {
                    return RedirectToAction("Index", "Role");
                }
            }

            return View(model);
        }


        /// <summary>
        /// 編輯角色
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            var data = roleService.GetById(id);
            if (data == null)
                return HttpNotFound();

            var viewModel = new RoleViewModel
            {
                Role = data,
                MenuCheckList = roleService.GetMenuByRoleId(id)
            };

            return View(viewModel);
        }


        /// <summary>
        /// 編輯角色post
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(RoleViewModel model)
        {
            var data = roleService.GetById(model.Role.Id);
            if (data == null)
                return HttpNotFound();

            // 修改角色
            CiResult result = roleService.DbUpdate(model.Role);
            TempData["alert"] = result.Message;
            if (result.ReturnResult == ReturnResult.Success)
            {
                // 修改選單list
                CiResult resultMenu = roleService.DbUpdateRoleMenus(model.Role.Id, model.MenuCheckList);
                TempData["alert"] += resultMenu.Message;
                if (resultMenu.ReturnResult == ReturnResult.Success)
                {
                    return RedirectToAction("Index", new { });
                }
            }

            return View(model);
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否執行成功</returns>
        public ActionResult Delete(Guid id)
        {
            CiResult result = roleService.DbDelete(id);
            TempData["alert"] = result.Message;

            return RedirectToAction("Index", new { });
        }

        protected override void Dispose(bool disposing)
        {
            roleService.Dispose();
            base.Dispose(disposing);
        }
    }
}
