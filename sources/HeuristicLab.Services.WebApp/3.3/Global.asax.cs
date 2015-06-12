﻿using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using HeuristicLab.Services.WebApp.Configs;

namespace HeuristicLab.Services.WebApp {
  public class MvcApplication : System.Web.HttpApplication {
    protected void Application_Start() {
      var pluginManager = PluginManager.Instance;
      pluginManager.Configuration = GlobalConfiguration.Configuration;
      pluginManager.DiscoverPlugins();
      AreaRegistration.RegisterAllAreas();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
    }
  }
}
