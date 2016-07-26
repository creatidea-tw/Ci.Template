using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    using Ci.Template.Library.Models;
    using $safeprojectname$.Helpers;

    public class BaseService : IDisposable
    {
        protected CiTemplateEntities Db = new CiTemplateEntities();

        protected UserHelper UserHelper;

        public void Dispose()
        {
        }
    }
}
