using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ci.Template.Service
{
    using Ci.Template.Library.Models;
    using Ci.Template.Service.Helpers;

    public class BaseService : IDisposable
    {
        protected CiTemplateEntities Db = new CiTemplateEntities();

        protected UserHelper UserHelper;

        public void Dispose()
        {
        }
    }
}
