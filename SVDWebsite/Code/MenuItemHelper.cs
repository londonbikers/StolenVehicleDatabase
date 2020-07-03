using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace SVDWebsite.Code
{
    /// <summary>
    /// This helper method renders a link within an HTML LI tag.
    /// A class="selected" attribute is added to the tag when
    /// the link being rendered corresponds to the current
    /// controller and action.
    /// 
    /// This helper method is used in the Site.Master View 
    /// Master Page to display the website menu.
    /// </summary>
    public static class MenuItemHelper
    {
        public static string MenuItem(this HtmlHelper helper, string linkText, string actionName, string controllerName, string areaName = null, string cssClass = null)
        {
            var currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];
            var currentActionName = (string)helper.ViewContext.RouteData.Values["action"];

            var sb = new StringBuilder();

            //if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase) && 
            //    currentActionName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase))
            if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase))
            {
                sb.Append("<li class=\"selected rounded\">");
            }
            else
            {
                var cssClassName = cssClass != null ? string.Format(" class=\"{0}\"", cssClass) : string.Empty;
                sb.AppendFormat("<li{0}>", cssClassName);
            }

            if (areaName != null)
                sb.Append(helper.ActionLink(linkText, actionName, controllerName, new { area = areaName }, null));
            else
                sb.Append(helper.ActionLink(linkText, actionName, controllerName));

            sb.Append("</li>");
            return sb.ToString();
        }
    }
}
