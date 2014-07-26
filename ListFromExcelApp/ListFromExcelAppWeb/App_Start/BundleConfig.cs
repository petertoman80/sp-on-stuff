using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Orderers;
using System.Web;
using System.Web.Optimization;

namespace ListFromExcelAppWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/spcontext").Include(
                        "~/Scripts/spcontext.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/jquery.fileupload").Include(
                        "~/Scripts/jquery.ui.widget.js",
                        "~/Scripts/jquery.iframe-transport.js",
                  //      "~/Scripts/load-image.min.js",
                        "~/Scripts/JQuery-fileupload/jquery.fileupload.js"
                        //"~/Scripts/JQuery-fileupload/jquery.fileupload-process.js",
                        //"~/Scripts/JQuery-fileupload/jquery.fileupload-validate.js",
                        //"~/Scripts/JQuery-fileupload/jquery.fileupload-image.js",
                        //"~/Scripts/JQuery-fileupload/jquery.fileupload-audio.js",
                        //"~/Scripts/JQuery-fileupload/jquery.fileupload-video.js",
                  //      "~/Scripts/JQuery-fileupload/jquery.fileupload-jquery-ui.js",
                  //      "~/Scripts/JQuery-fileupload/jquery.fileupload-ui.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                      "~/Content/bootstrap.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/jquery.fileupload").Include(
                      "~/Content/jquery.fileupload.css"));

            var commonStylesBundle = new CustomStyleBundle("~/Content/site");
            commonStylesBundle.Orderer = new NullOrderer();
            commonStylesBundle.Include("~/Content/site.less");
            bundles.Add(commonStylesBundle);
            

        }
    }
}
