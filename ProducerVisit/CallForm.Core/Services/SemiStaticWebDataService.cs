﻿namespace CallForm.Core.Services
{
    using CallForm.Core.Models;
    using CallForm.Core.ViewModels;
    using Cirrious.MvvmCross.Plugins.File;
    using Cirrious.MvvmCross.Plugins.Network.Rest;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Xml.Serialization;

    /// <summary>Implements the <see cref="ISemiStaticWebDataService"/> interface.
    /// </summary>
    public class SemiStaticWebDataService : ISemiStaticWebDataService
    {
        private readonly IMvxFileStore _fileStore;
        private readonly IMvxJsonRestClient _jsonRestClient;
        private readonly IDataService _localDatabaseService;
        //private readonly string _targetURL;
        private string _request;

        // hack: fix the _targetURL definitions to match web.*.config
        // temporary config:
        //  - release/production "http://ProducerCRM.DairyDataProcessing.com";
        //  - beta/staging       "http://ProducerCRM.DairyDataProcessing.com";
        //  - alpha/testing      "http://dl-backend-02.azurewebsites.net";
        //  - debug/internal     "http://dl-websvcs-test.dairydata.local:480";

        // final config:
        //  - release/production "http://ProducerCRM.DairyDataProcessing.com";
        //  - beta/staging       "http://dl-backend.azurewebsites.net";
        //  - alpha/testing      "http://dl-backend-02.azurewebsites.net";
        //  - debug/internal     "http://dl-websvcs-test.dairydata.local:480";

        // others/not used:
        //    "http://DL-WebSvcs-03:480";
        //    "http://dl-websvcs-03.dairydata.local:480"; 

        //    "http://dl-webserver-te.dairydata.local:480"; 
        //    "http://dl-WebServer-Te";
        //    "http://dl-WebSvcs-tes2";


        // Note: this value determines where the app will look for web services


        //private static string _targetURL = "http://dl-backend-02.azurewebsites.net";
        //private static string _targetURL = "http://dl-websvcs-test.dairydata.local:480";
        private static string _targetURL = "http://dl-websvcs-03:480";
        //private static string _targetURL = "http://ProducerCRM.DairyDataProcessing.com";

        private static string _dataFolderPathName = "Data";
        private static string _callTypeFileName = "CallTypes.xml";
        private static string _emailRecipientFileName = "EmailRecipients.xml";
        private static string _reasonCodeFileName = "ReasonCodes.xml";

        /// <summary>Provides access to the <paramref name="fileStore"/>, <paramref name="jsonRestClient"/>, and <paramref name="localSQLiteDataService"/>.
        /// </summary>
        /// <param name="fileStore">The target <see cref="Cirrious.MvvmCross.Plugins.File.IMvxFileStore"/></param>
        /// <param name="jsonRestClient">The target <see cref="Cirrious.MvvmCross.Plugins.Network.Rest.IMvxJsonRestClient"/></param>
        /// <param name="localSQLiteDataService">The target <see cref="IDataService"/></param>
        public SemiStaticWebDataService(IMvxFileStore fileStore, IMvxJsonRestClient jsonRestClient, IDataService localSQLiteDataService)
        {
            _fileStore = fileStore;
            _jsonRestClient = jsonRestClient;
            _localDatabaseService = localSQLiteDataService;
        }

        #region Required Definitions
        /// <inheritdoc/>
        public List<ReasonCode> GetReasonCodes()
        {
            return _localDatabaseService.GetSQLiteReasonCodes();
        }

        /// <inheritdoc/>
        public List<string> GetCallTypesAsList()
        {
            List<string> stringList = new List<string>(new[] { "initialized" });

            CheckFolder(_dataFolderPathName);

            string xml = string.Empty;
            string targetFilename = _fileStore.PathCombine(_dataFolderPathName, _callTypeFileName);

            if (!_fileStore.Exists(targetFilename))
            {
                stringList.Clear();
                stringList.Add("file doesn't exist.");
                //UpdateXmlCallTypes();
            }

            if (_fileStore.TryReadTextFile(targetFilename, out xml))
            {
                List<CallType> objectList = Deserialize<List<CallType>>(xml);
                stringList = objectList.Select(i => i.ToString()).ToList();
            }
            else
            {
                stringList.Clear();
                stringList.Add("Error reading file.");
            }

            // double-check that we got some result
            int objectCount = stringList.Count();
            stringList.Add("count is " + objectCount);

            return stringList;
        }

        /// <inheritdoc/>
        public List<string> GetEmailRecipientsAsList()
        {
            List<string> stringList = new List<string>(new[] { "initialized" } );
                    
            CheckFolder(_dataFolderPathName);

            string xml = string.Empty;
            string targetFilename = _fileStore.PathCombine(_dataFolderPathName, _reasonCodeFileName);

            if (!_fileStore.Exists(targetFilename))
            {
                stringList.Clear();
                stringList.Add("file doesn't exist.");
            }

            if (_fileStore.TryReadTextFile(targetFilename, out xml))
            {
                List<EmailRecipient> objectList = Deserialize<List<EmailRecipient>>(xml);
                stringList = objectList.Select(i => i.ToString()).ToList();
            }
            else
            {
                stringList.Clear();
                stringList.Add("Error reading file.");
            }

            // double-check that we got some result
            int objectCount = stringList.Count();
            stringList.Add("count is " + objectCount);

            return stringList;
        }

        /// <inheritdoc/>
        public void UpdateModels()
        {
            string filename = string.Empty;

            try
            {
                CheckFolder(_dataFolderPathName);

                // review: how often are these tables going to be changing? do we really need to pull the fresh list every time?
                // request Reason Codes from the web service, and save them on-device
                Request = _targetURL + "/Visit/Reasons/";
                var request = new MvxRestRequest(Request);
                // (Action<MvxRestResponse>)ParseResponse

                _jsonRestClient.MakeRequestFor<List<ReasonCode>>(request,
                    response =>
                    {
                        _localDatabaseService.UpdateSQLiteReasonCodes(response.Result);

                        filename = _fileStore.PathCombine(_dataFolderPathName, _reasonCodeFileName);
                        _fileStore.WriteFile(filename, Serialize(response.Result));
                    },
                    (Action<Exception>)RestException);

                // request Call Types from the web service
                Request = _targetURL + "/Visit/CallTypes/";
                request = new MvxRestRequest(Request);
                _jsonRestClient.MakeRequestFor<List<CallType>>(request,
                    response =>
                    {
                        filename = _fileStore.PathCombine(_dataFolderPathName, _callTypeFileName);
                        _fileStore.WriteFile(filename, Serialize(response.Result));
                    },
                    (Action<Exception>)RestException);

                // request Email Recipients from the web service
                Request = _targetURL + "/Visit/EmailRecipients/";
                request = new MvxRestRequest(Request);
                _jsonRestClient.MakeRequestFor<List<EmailRecipient>>(request,
                    response =>
                    {
                        filename = _fileStore.PathCombine(_dataFolderPathName, _emailRecipientFileName);
                        _fileStore.WriteFile(filename, Serialize(response.Result));
                    },
                    (Action<Exception>)RestException);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        #endregion

        private void CheckFolder(string folderPath)
        {
            _fileStore.EnsureFolderExists(folderPath);
        }

        private void ParseResponse(MvxRestResponse response)
        {
            _localDatabaseService.ReportUploaded(int.Parse(response.Tag));
        }

        private void RestException(Exception exception)
        {
            Debug.WriteLine("Original request: " + Request);
            Debug.WriteLine("Exception message: " + exception.Message);
        }

        public string Request
        {
            get { return _request; }
            set
            {
                _request = value;
            }
        }

        public event EventHandler<ErrorEventArgs> Error;

        /// <summary>Convert XML to an object of type <paramref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to apply to the XML.</typeparam>
        /// <param name="xml">An XML ("serialized") string.</param>
        /// <returns>The <paramref name="xml"/> deserialized to an object of type <paramref name="T"/>.</returns>
        public static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            T container;
            using (TextReader stream = new StringReader(xml))
            {
                container = (T)serializer.Deserialize(stream);
            }
            return container;
        }

        /// <summary>Convert an object of type <paramref name="T"/> to XML.
        /// </summary>
        /// <typeparam name="T">The type to apply to <paramref name="obj"/>.</typeparam>
        /// <param name="obj">An object that needs to be serialized.</param>
        /// <returns>An object "serialized" to XML.</returns>
        public static string Serialize<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (StringWriter stream = new StringWriter())
            {
                serializer.Serialize(stream, obj);
                return stream.ToString();
            }
        }
    }
}
