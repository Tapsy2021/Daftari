using System.Web;
using System.Web.Optimization;

namespace Daftari
{
    public class BundleConfig
    {

        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/hyper").Include(
                     "~/Scripts/vendor.js",
                     "~/Scripts/styleConfig.js",
                      "~/Scripts/app.js"));

            bundles.Add(new StyleBundle("~/Content/hypericon").Include(
                      "~/Content/icon.css"));
            bundles.Add(new StyleBundle("~/Content/hyperlight").Include(
                "~/Content/app.css",
                       "~/Content/Site.css"));
            bundles.Add(new StyleBundle("~/Content/hyperdark").Include(
                "~/Content/app-dark.css",
                    "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/datatables").Include(
                "~/Content/vendor/dataTables.bootstrap4.css",
                "~/Content/vendor/responsive.bootstrap4.css",
                "~/Content/vendor/buttons.bootstrap4.css",
                "~/Content/vendor/select.bootstrap4.css",
                "~/Content/vendor/scroller.bootstrap4.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
              "~/Scripts/vendor/jquery.dataTables.min.js",
               "~/Scripts/vendor/dataTables.bootstrap4.js",
               "~/Scripts/vendor/dataTables.responsive.min.js",
               "~/Scripts/vendor/responsive.bootstrap4.min.js",
               "~/Scripts/vendor/dataTables.buttons.min.js",
               "~/Scripts/vendor/buttons.bootstrap4.min.js",
               "~/Scripts/vendor/buttons.colVis.js",
               "~/Scripts/vendor/buttons.html5.min.js",
               "~/Scripts/vendor/buttons.print.min.js",
               "~/Scripts/vendor/dataTables.keyTable.min.js",
               "~/Scripts/vendor/dataTables.select.min.js",
               "~/Scripts/vendor/dataTables.scroller.js",
               "~/Scripts/vendor/scroller.bootstrap4.js"));

            bundles.Add(new ScriptBundle("~/bundles/pdfmake").Include(
                "~/Scripts/pdfmake/pdfmake.js",
                "~/Scripts/pdfmake/vfs_fonts.js"
                ));

            bundles.Add(new StyleBundle("~/Content/quillcss").Include(
                "~/Content/quill/katex.css",
                "~/Content/quill/monokai-sublime.css",
                "~/Content/quill/quill.snow.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/quill").Include(
                "~/Scripts/quill/katex.js",
                "~/Scripts/quill/highlight.pack.js",
                "~/Scripts/quill/quill.js"
                ));

            bundles.Add(new StyleBundle("~/Content/mobilecss").Include(
                "~/Content/mobile/mobile.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/certGenerator").Include(
                "~/Scripts/pdfdefintions/certGenerator.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/buttonsSync").Include(
               "~/Scripts/main/buttonsSync.js"
           ));

            bundles.Add(new StyleBundle("~/Content/datetimepicker").Include(
                "~/Content/vendor/bootstrap-datetimepicker.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/datetimepicker").Include(
               "~/Scripts/vendor/bootstrap-datetimepicker.min.js"
            ));
        }
    }
}
