﻿using System;
using System.Collections.Generic;
using CallForm.Core.Models;
using Cirrious.MvvmCross.Plugins.File;
using Cirrious.MvvmCross.Plugins.Network.Rest;
using CallForm.Core.ViewModels;

namespace CallForm.Core.Services
{
    /// <summary>Implements the <see cref="IUserIdentityService"/> interface.
    /// </summary>
    public class UserIdentityService : IUserIdentityService
    {
        /// <summary>An instance of the <see cref="IMvxFileStore"/>.
        /// </summary>
        private readonly IMvxFileStore _fileStore;

        /// <summary>An instance of the <see cref="IMvxRestClient"/>.
        /// </summary>
        private readonly IMvxRestClient _restClient;

        //private readonly string _targetURL;

        // hack: fix the _targetURL definitions to match web.*.config
        // temporary config:
        //  - release/production "http://ProducerCRM.DairyDataProcessing.com";
        //  - beta/staging       "http://ProducerCRM.DairyDataProcessing.com";
        //  - alpha/testing      "http://dl-backend-02.azurewebsites.net";
        //  - debug/internal     "http://dl-websvcs-test.dairydata.local";

        // final config:
        //  - release/production "http://ProducerCRM.DairyDataProcessing.com";
        //  - beta/staging       "http://dl-backend.azurewebsites.net";
        //  - alpha/testing      "http://dl-backend-02.azurewebsites.net";
        //  - debug/internal     "http://dl-websvcs-test.dairydata.local";

        // others/not used:
        //    "http://dl-webserver-te.dairydata.local:480"; 
        //    "http://DL-WebSvcs-03:480";

        // Note: this value determines where the app will look for web services
#if (RELEASE)
        private static string _targetURL = "http://ProducerCRM.DairyDataProcessing.com"; 
#elif (BETA)
        private static string _targetURL = "http://ProducerCRM.DairyDataProcessing.com"; 
#elif (ALPHA)
        private static string _targetURL = "http://dl-backend-02.azurewebsites.net/";
#else
        private static string _targetURL = "http://dl-websvcs-test";
#endif

        /// <summary>Provides access to the <paramref name="fileStore"/> and <paramref name="restClient"/>.
        /// </summary>
        /// <param name="fileStore">The target <see cref="IMvxFileStore"/></param>
        /// <param name="restClient">The target <see cref="IMvxRestClient"/></param>
        public UserIdentityService(IMvxFileStore fileStore, IMvxRestClient restClient)
        {
            // FixMe: this seems to be the first method that requires a data connection
            _fileStore = fileStore;
            _restClient = restClient;
        }

        #region Required Definitions
        /// <inheritdoc/>
        public bool IdentityRecorded
        {
            get
            {
                _fileStore.EnsureFolderExists("Data");
                var filename = _fileStore.PathCombine("Data", "Identity.xml");
                return _fileStore.Exists(filename);
            }
        }

        /// <inheritdoc/>
        public void SaveIdentity(UserIdentity identity)
        {
            SaveIdentityToFile(identity);

            SaveIdentityToWebService(identity);
        }

        private void SaveIdentityToFile(UserIdentity identity)
        {
            try
            {
                if (!IdentityRecorded)
                {
                    _fileStore.EnsureFolderExists("Data");
                    var filename = _fileStore.PathCombine("Data", "Identity.xml");
                    _fileStore.WriteFile(filename, SemiStaticWebDataService.Serialize(identity));
                }
            }
            catch
            {
                // FixMe: just ignore any errors for now
                throw;
            }
        }

        private void SaveIdentityToWebService(UserIdentity identity)
        {
            try
            {
                var request =
                    new MvxJsonRestRequest<UserIdentity>(_targetURL + "/Visit/Identity/")
                    {
                        Body = identity
                    };

                // review: add error handling here
                _restClient.MakeRequest(request, (Action<MvxRestResponse>)ParseResponse, exception => { });
            }
            catch
            {
                // FixMe: just ignore any errors for now
                throw;
            }
        }
        
        /// <inheritdoc/>
        public UserIdentity GetSavedIdentity()
        {
            var filename = _fileStore.PathCombine("Data", "Identity.xml");
            string xml;
            if (_fileStore.Exists(filename) && _fileStore.TryReadTextFile(filename, out xml))
            {
                return SemiStaticWebDataService.Deserialize<UserIdentity>(xml);
            }
            return null;
        }
        #endregion

        private void ParseResponse(MvxRestResponse obj)
        {
        }

        public event EventHandler<ErrorEventArgs> Error;
    }
}
