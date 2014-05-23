﻿using BackEnd.Models;

namespace BackEnd
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public string EntityType { get; set;}


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // broken: this line must be commented out on the initial Publish/Deploy. can this be automated?
            // Doing so enables the tables (schema) to be established in the database.
            // After the first run, the uncommented line allows everything to work.
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<VisitContext>());

            // The following implementations are provided: 
            // DropCreateDatabaseIfModelChanges<TContext>, DropCreateDatabaseAlways<TContext>, CreateDatabaseIfNotExists<TContext>.
            // this seems safer, but still must be commented out on first run:
            // Database.SetInitializer(new CreateDatabaseIfNotExists<VisitContext>());

            Database.SetInitializer<VisitContext>(new SeededSiteDBInitialize());
            using (var myContext = new VisitContext())
            {
                var x = myContext.Database.Exists(); // hack
            }

            // FixMe: the issue seems to be that later on, if the empty table(s) exist, the code
            // will not populate them with the default values. It should be possible to move the database
            // seeding here, and never comment out "CreateDatabaseIfNotExists".

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // Note: Visual Studio does not do anything with the database during the deployment process. However, when 
        // the deployed application accesses the database for the first time after deployment, Code First 
        // automatically creates the database or updates the database schema to the latest version. If the 
        // application implements a Migrations Seed method, the method runs after the database is created or the schema is updated.

        /// <summary>Create a new instance of type <VisitContext>. 
        /// </summary>
        /// <remarks>On the first connection to the server, if the specified database does not exist it will be created.</remarks>
        public class SiteDBInitialize : CreateDatabaseIfNotExists<VisitContext>
        {
            // use the Seed method below to initially populate the database
        }

        /// <summary>Dangerous. If the database model has changed, Drop/Create a new instance of type <VisitContext>. 
        /// </summary>
        /// <remarks>On the first connection to the server, if the specified database exists and the database model is 
        /// different, the existing instance of the database will be dropped and a new instance (with the new model)
        /// will be created.</remarks>
        //public class SiteDBInitialize : DropCreateDatabaseIfModelChanges<VisitContext>
        //{
        //    protected override void Seed(VisitContext context)
        //    {
        //        context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "global.asax.cs DropCreateDatabaseIfModelChanges", });
        //        context.SaveChanges();
        //    }
        //}

        ///// <summary>Very dangerous. ALWAYS Drop/Create a new instance of type <VisitContext>. 
        ///// </summary>
        //public class SiteDBInitialize : DropCreateDatabaseAlways<VisitContext>
        //{
        //    protected override void Seed(VisitContext context)
        //    {
        //        context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "global.asax.cs DropCreateDatabaseAlways", });
        //        context.SaveChanges();
        //    }
        //}


        /// <summary>Inherited object that "wraps" SiteDBInitialize so that this one Seed method can service all three initialize classes
        /// (by simply commenting out the two inactive classes).
        /// </summary>
        public class SeededSiteDBInitialize : SiteDBInitialize
        {
            /// <summary>Runs after the database is created or the schema is updated.
            /// </summary>
            /// <param name="context">The VisitContext model from BackEnd.</param>
            protected override void Seed(VisitContext context)
            {
                context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "info@agri-maxfinancial.com", DisplayName = "info@agri-maxfinancial.com" });
                context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "info@agri-servicesagency.com", DisplayName = "info@agri-servicesagency.com" });
                context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "communications@dairylea.com", DisplayName = "Member Communications" });
                context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "FieldStaffNotification-DairyOne@DairyOne.com", DisplayName = "FieldStaffNotification-DairyOne@DairyOne.com" });
                context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "FieldStaffNotification-DMS@dairylea.com", DisplayName = "FieldStaffNotification-DMS@dairylea.com" });
                context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "drms@dairylea.com", DisplayName = "DRMS" });
                context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "FieldStaffNotification-Eagle@dairylea.com", DisplayName = "FieldStaffNotification-Eagle@dairylea.com" });
                context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "FieldStaffNotification-HR@dairylea.com", DisplayName = "FieldStaffNotification-HR@dairylea.com" });
                context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "TechnicalSupport-brittonfield@dairylea.com", DisplayName = "Technical Support" });
                context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "FieldStaffNotification-Membership@dairylea.com", DisplayName = "FieldStaffNotification-Membership@dairylea.com" });
                context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "FieldStaffNotification-Payroll@dairylea.com", DisplayName = "FieldStaffNotification-Payroll@dairylea.com" });

                context.SaveChanges();
            }
        }
    }
}
