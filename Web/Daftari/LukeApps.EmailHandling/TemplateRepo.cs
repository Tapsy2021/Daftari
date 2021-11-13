using System.Collections.Generic;
using System.IO;
using System.Web;

namespace LukeApps.EmailHandling
{
    public class TemplateRepo
    {
        private const string _defaultTemplatePath = "Content/template.html";

        private static readonly TemplateRepo _instance =
                  new TemplateRepo();

        private Dictionary<string, string> _templates;

        private TemplateRepo()
        {
            _templates = new Dictionary<string, string>
            {
                { _defaultTemplatePath, getTemplate(_defaultTemplatePath) }
            };
        }

        public static TemplateRepo GetInstance()
        {
            return _instance;
        }

        public string GetTemplate(string path = null)
        {
            if (path == null)
                path = _defaultTemplatePath;

            _templates.TryGetValue(path, out string template);

            if (template != null)
                return template;

            _templates.Add(path, getTemplate(path));

            return GetTemplate(path);
        }

        private string getTemplate(string template)
        {
            string value;
            using (StreamReader reader = new StreamReader(Path.Combine(HttpRuntime.AppDomainAppPath, template)))
            {
                value = reader.ReadToEnd();
            }
            return value;
        }
    }
}