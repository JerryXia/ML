using System.Web;
using System.Web.Mvc;

namespace ML.Web.Mvc.EasyUI.Demo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}