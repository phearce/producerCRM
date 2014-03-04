﻿namespace BackEnd
{
    using System;
    //using BackEnd.Models;
    using System.Data.Entity;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            try
            {
                // broken: this line must be commented out on the initial Publish/Deploy. can this be automated?
                // Doing so enables the tables (schema) to be established in the database.
                // After the first run, the uncommented line allows everything to work.
                Database.SetInitializer(new DropCreateDatabaseIfModelChanges<VisitContext>());

                WebApiConfig.Register(GlobalConfiguration.Configuration);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                Console.WriteLine(exc.StackTrace);
            }
            
        }
    }
}