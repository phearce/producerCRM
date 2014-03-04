﻿namespace CallForm.iOS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MonoTouch.Foundation;
    using MonoTouch.UIKit;

    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            bool hostWasReachable = false;

            if (!Reachability.IsHostReachable("dl-backend-02.azurewebsites.net"))
            {
                hostWasReachable = false;

                // if you want to use a different Application Delegate class from "AppDelegate"
                // you can specify it here.
                UIApplication.Main(args, null, "AppDelegate"); 
            }
            else
            {
                hostWasReachable = true;
                
                UIApplication.Main(args, null, "AppDelegate");
            }

            


        }
    }
}