using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ExpensesApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Attribute Routing
            config.MapHttpAttributeRoutes();

            // Changing XML format to JSON
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.Indent = true;
        }
    }
}
