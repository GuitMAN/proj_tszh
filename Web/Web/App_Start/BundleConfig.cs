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
                        "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery.min").Include(
                        "~/Scripts/jquery-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                                    "~/Scripts/jquery-ui.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryui.min").Include(
                        "~/Scripts/jquery-ui.min.js"));

            bundles.Add(new StyleBundle("~/Styles/jqueryui.min").Include("~/Scripts/jquery-ui.min.css"));

            bundles.Add(new StyleBundle("~/Styles/jquery-ui/start").Include("~/Scripts/themes/start/jquery-ui.min.css"));    

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            "~/Scripts/jquery.unobtrusive*",
            "~/Scripts/jquery.validate*"));
        }
    }
}