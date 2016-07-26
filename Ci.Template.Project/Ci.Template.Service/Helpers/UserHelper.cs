using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$.Helpers
{
    using System.Security.Claims;

    using Ci.Template.Library.Commons;

    public class UserHelper : ClaimsPrincipal
    {
        public ClaimsPrincipal principal;

        public UserHelper(ClaimsPrincipal principal) : base(principal)
        {
            this.principal = principal;
        }

        // 取得使用者id
        public Guid Id => new Guid(FindFirst(ClaimTypes.NameIdentifier).Value);

        // 取得使用者帳號
        public string Name => FindFirst(ClaimTypes.Name).Value;

        // 取得使用者Menu
        public List<Guid> UserMenu
        {
            get
            {
                try
                {
                    return FindAll(ClaimTypes.UserData).Select(x => Guid.Parse(x.Value)).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }

        // 使用者權限
        public IEnumerable<string> UserRole => FindAll(ClaimTypes.Role).Select(x => x.Value);


        // 是否是超級管理員
        public bool IsSuperManager
        {
            get
            {
                try
                {
                    return FindFirst(ClaimTypes.NameIdentifier).Value == Keys.SuperManager;
                }
                catch (Exception)
                {

                    return false;
                }
            }

        }

        // 取得所屬博物館id
        public Guid MuseumId
        {
            get
            {
                try
                {
                    return new Guid(FindFirst(ClaimTypes.Sid).Value);
                }
                catch (Exception)
                {
                    return default(Guid);
                }
            }
            set
            {

                //if (principal != null)
                //{
                //    ClaimsIdentity identity = principal.Identities.ElementAt(0);
                //    identity.RemoveClaim(FindFirst(ClaimTypes.Sid));
                //    identity.AddClaim(new Claim(ClaimTypes.Sid, value.ToString()));
                //}

            }
        }
    }
}
