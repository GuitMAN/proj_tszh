using System.Web;
using System.Web.Optimization;

namespace Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Styles/css").Include("~/Styles/style.css"));
            bundles.Add(new StyleBundle("~/Styles/error").Include("~/Styles/error.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/external/jquery/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            "~/Scripts/jquery-ui.js",
            "~/Scripts/jquery-ui.min.js"));
            bundles.Add(new StyleBundle("~/Styles/themes/start/jquery-ui").Include("~/Scripts/jquery-ui.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            "~/Scripts/jquery.validate.unobtrusive.js",
            "~/Scripts/jquery.validate.unobtrusive.min.js"));
        }
    }
}