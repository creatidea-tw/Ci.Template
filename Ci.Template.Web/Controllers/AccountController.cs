namespace Ci.Template.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using Ci.Template.Library.Models;
    using Ci.Template.Library.ViewModels;
    using Ci.Template.Service;
    using Ci.Template.Service.Helpers;

    using Creatidea.Library.Results;

    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security;

    /// <summary>
    /// 帳號管理
    /// </summary>
    public class AccountController : AuthBaseController
    {
        private AccountService accountService = new AccountService { };

        #region "登入登出"

        /// <summary>
        /// 帳號登入
        /// </summary>
        /// <returns>ActionResult.</returns>
        [AllowAnonymous]
        public ActionResult Login()
        {
            // 判斷使用者是否已經登入驗證
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        /// 帳號登入post
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Admin admin = accountService.CheckLogin(model.Account, model.Password, NetHelper.GetIp());
                if (admin != null)
                {
                    Session.RemoveAll();
                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["alert"] = "帳號密碼錯誤";
            return View(model);
        }

        /// <summary>
        /// 帳號登出
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            // 清除Cookies
            int limit = Request.Cookies.Count;
            for (int i = 0; i < limit; i++)
            {
                string cookieName = Request.Cookies[i].Name;
                HttpCookie aCookie = new HttpCookie(cookieName);
                aCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(aCookie);
            }

            Session.RemoveAll();

            if (AuthenticationManager != null)
            {
                var appTypes = AuthenticationManager.GetAuthenticationTypes();
                string[] auth = appTypes.Select(at => at.AuthenticationType).ToArray();
                AuthenticationManager.SignOut(auth);
            }

            return RedirectToAction("Login");
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        #endregion

        #region"帳號"

        /// <summary>
        /// 帳號列表
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Index(int page = 1)
        {
            var data = accountService.GetList(page);
            return View(data);
        }

        /// <summary>
        /// 新增帳號
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Create()
        {
            var viewModel = new AccountViewModel();
            viewModel.RoleCheckList = accountService.GetRoleByAdminId(null);

            return View(viewModel);
        }

        /// <summary>
        /// 新增帳號post
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public ActionResult Create(AccountViewModel model)
        {
            // 新增帳號
            CiResult<Guid> result = accountService.DbCreate(model.AccountView.Account, model.AccountView.Password);
            TempData["alert"] = result.Message;

            if (result.ReturnResult == ReturnResult.Success)
            {
                Guid adminId = result.Data;
                // 新增角色list
                CiResult resultRole = accountService.DbUpdateAdminRoles(adminId, model.RoleCheckList);
                TempData["alert"] += resultRole.Message;
                if (resultRole.ReturnResult == ReturnResult.Success)
                {
                    return RedirectToAction("Index", "Account");
                }
            }

            return View(model);
        }

        /// <summary>
        /// 編輯帳號
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Edit(Guid id)
        {
            var data = accountService.GetById(id);
            if (data == null)
                return HttpNotFound();

            var viewModel = new AccountViewModel();
            var account = new AccountView
            {
                Id = data.Id,
                Account = data.Account
            };

            viewModel.AccountView = account;
            viewModel.RoleCheckList = accountService.GetRoleByAdminId(id);

            return View(viewModel);
        }

        /// <summary>
        /// 編輯帳號post
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public ActionResult Edit(AccountViewModel model)
        {
            var data = accountService.GetById(model.AccountView.Id);
            if (data == null)
                return HttpNotFound();

            // 更改帳號
            CiResult resultAcount = accountService.DbUpdateAccount(model.AccountView.Id, model.AccountView.Account);
            TempData["alert"] = resultAcount.Message;
            if (resultAcount.ReturnResult != ReturnResult.Success)
            {
                return View(model);
            }

            // 更改密碼
            if (!string.IsNullOrEmpty(model.AccountView.Password))
            {
                CiResult resultPassword = accountService.DbUpdatePassword(model.AccountView.Id, model.AccountView.Password);
                TempData["alert"] += resultPassword.Message;
                if (resultPassword.ReturnResult != ReturnResult.Success)
                {
                    return View(model);
                }
            }

            // 更改角色list
            CiResult resultRole = accountService.DbUpdateAdminRoles(model.AccountView.Id, model.RoleCheckList);
            TempData["alert"] += resultRole.Message;
            if (resultRole.ReturnResult != ReturnResult.Success)
            {
                return View(model);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 修改自己的密碼
        /// </summary>
        /// <returns>ActionResult.</returns>
        [AllowAnonymous]
        public ActionResult EditPassword()
        {
            // 未登入
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("Login");
            }


            var data = accountService.GetById(Guid.Parse(User.Identity.GetUserId()));
            if (data == null)
                return HttpNotFound();

            var model = new PasswordViewModel { Id = data.Id };

            return View(model);
        }

        /// <summary>
        /// 修改自己的密碼post
        /// </summary>
        /// <param name="model">The Admin model.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditPassword(PasswordViewModel model)
        {
            var data = accountService.GetById(model.Id);
            if (data == null)
                return HttpNotFound();

            if (accountService.CheckPassword(model.OldPassword, data.Password))
            {
                TempData["alert"] = "舊密碼輸入錯誤";
                return View(model);
            }
            else if (model.NewPassword != model.PasswordConfirm)
            {
                TempData["alert"] = "新密碼輸入不一致";
                return View(model);
            }
            else
            {
                accountService.DbUpdatePassword(model.Id, model.NewPassword);
                TempData["alert"] = "密碼已成功更改";
                return RedirectToAction("Index", "Home");
            }

        }

        /// <summary>
        /// 刪除帳號
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>index</returns>
        public ActionResult Delete(Guid id)
        {
            var data = accountService.GetById(id);
            if (data == null)
                return HttpNotFound();

            accountService.DbDelete(id);
            TempData["alert"] = "帳號已成功刪除";
            return RedirectToAction("Index", "Account");
        }

        /// <summary>
        /// 確認帳號是否可以使用
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult CheckName(string account, Guid? id)
        {
            bool result = accountService.CheckName(account, id);

            return Content(result.ToString());
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            accountService.Dispose();
            base.Dispose(disposing);
        }
    }
}
