﻿namespace CallForm.Core.Models
{
    using System;
    using Cirrious.MvvmCross.Plugins.Sqlite;

    /// <summary>Creates an object representing a "StoredProducerVisitReport" record.
    /// </summary>
    /// <remarks>The database stores each visit as a record in the "StoredProducerVisitReport" table, and the 
    /// possible reason for a visit in the "ReasonCode" table. For any given visit the reasons are stored in a cross reference table 
    /// "VisitXReason".
    /// 
    /// A <seealso cref="StoredProducerVisitReport"/> object represents a single records from the "StoredProducerVisitReport" table. 
    /// A <seealso cref="ProducerVisitReport"/> is that same record with a <seealso cref="ReasonCode[]"/> holding the 
    /// reason codes for the specific visit.</remarks>
    public class StoredProducerVisitReport
    {
        /// <summary>The internal ID for this object.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        // fixme: change userID to DeviceID

        /// <summary>The device ID associated with this visit.
        /// </summary>
        public string UserID { get; set; }

        // fixme: refactor FarmNumber to MemberNumber

        /// <summary>The 8 digit member number.
        /// </summary>
        public string FarmNumber { get; set; }

        /// <summary>Decimal latitude.
        /// </summary>
        public double Lat { get; set; }

        /// <summary>Decimal longitude.
        /// </summary>
        public double Lng { get; set; }

        /// <summary>The date the visit took place.
        /// </summary>
        public DateTime VisitDate { get; set; }

        /// <summary>The length of the visit.
        /// </summary>
        public decimal Duration { get; set; }

        /// <summary>The timestamp of this Visit Report.
        /// </summary>
        public DateTime EntryDateTime { get; set; }

        /// <summary>The Call Type for this visit.
        /// </summary>
        public string CallType { get; set; }

        /// <summary>Text notes associated with this visit.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>The email recipients notified about this visit.
        /// </summary>
        public string EmailRecipients { get; set; }

        /// <summary>A picture associated with this visit.
        /// </summary>
        public byte[] PictureBytes { get; set; }

        /// <summary>Has this <seealso cref="StoredProducerVisitReport"/> been uploaded?
        /// </summary>
        public bool Uploaded { get; set; }

        /// <summary>The class constructor.
        /// </summary>
        public StoredProducerVisitReport()
        {}

        /// <summary>Creates a <seealso cref="StoredProducerVisitReport"/> based on a <seealso cref="ProducerVisitReport"/>.
        /// </summary>
        /// <remarks>Creates a <seealso cref="StoredProducerVisitReport"/> by dropping the <seealso cref="ReasonCode[]"/>, and
        /// marking the Uploaded properties as false.</remarks>
        /// <param name="visitReport">The visit report.</param>
        public StoredProducerVisitReport(ProducerVisitReport visitReport)
        {
            ID = visitReport.ID;
            UserID = visitReport.UserID;
            FarmNumber = visitReport.FarmNumber;
            Lat = visitReport.Lat;
            Lng = visitReport.Lng;
            VisitDate = visitReport.VisitDate;
            Duration = visitReport.Duration;
            EntryDateTime = visitReport.EntryDateTime;
            CallType = visitReport.CallType;
            Notes = visitReport.Notes;
            EmailRecipients = visitReport.EmailRecipients;
            PictureBytes = visitReport.PictureBytes;
            Uploaded = false;
        }

        /// <summary>Creates a <seealso cref="ProducerVisitReport"/> by appending a <seealso cref="ReasonCode[]"/> to this 
        /// <seealso cref="StoredProducerVisitReport"/>.
        /// </summary>
        /// <param name="reasonCodes">An array of <seealso cref="ReasonCode"/> to be added.</param>
        /// <returns>A <seealso cref="ProducerVisitReport"/>.</returns>
        public ProducerVisitReport Hydrate(ReasonCode[] reasonCodes)
        {
            return new ProducerVisitReport
            {
                ID = ID,
                UserID = UserID,
                FarmNumber = FarmNumber,
                Lat = Lat,
                Lng = Lng,
                VisitDate = VisitDate,
                Duration = Duration,
                EntryDateTime = EntryDateTime,
                CallType = CallType,
                Notes = Notes,
                EmailRecipients = EmailRecipients,
                PictureBytes = PictureBytes,
                ReasonCodes = reasonCodes
            };
        }
    }
}