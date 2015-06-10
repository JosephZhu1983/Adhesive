using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Adhesive.AppInfoCenter
{
    public interface IUnhandledExceptionFilter
    {
        bool DoFilter(HttpContext httpContext);
    }
}
