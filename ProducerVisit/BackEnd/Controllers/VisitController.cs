﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BackEnd.Models;
using CallForm.Core.Models;

namespace BackEnd.Controllers
{
    public class VisitController : Controller
    {
        private readonly VisitContext _db = new VisitContext();

        public ActionResult Index()
        {
            ViewBag.Count = _db.ProducerVisitReports.Count();
            return View();
        }

        public ActionResult Recent(string id)
        {
            var spvrs = _db.ProducerVisitReports.Where(vr => vr.FarmNumber == id)
                .OrderByDescending(vr => vr.VisitDate)
                .Take(10).ToList();

            var rlis = spvrs.Select(spvr =>
            {
                var pvr = Hydrated(spvr);
                var userID = _db.UserIdentities.FirstOrDefault(uid => uid.DeviceID == spvr.UserID);
                return new ReportListItem
                {
                    ID = spvr.ID,
                    UserEmail = (userID ?? new UserIdentity{UserEmail = "Unkown"}).UserEmail,
                    FarmNumber = pvr.FarmNumber,
                    Local = false,
                    PrimaryReasonCode = pvr.ReasonCodes[0],
                    VisitDate = pvr.VisitDate,
                    Uploaded = true
                };
            }).ToList();

            return Json(rlis, JsonRequestBehavior.AllowGet);
        }

        public ActionResult All(string id)
        {
            var spvrs = _db.ProducerVisitReports.Where(vr => vr.FarmNumber == id)
                .OrderByDescending(vr => vr.VisitDate).ToList();

            var rlis = spvrs.Select(spvr =>
            {
                var pvr = Hydrated(spvr);
                return new ReportListItem
                {
                    ID = spvr.ID,
                    UserEmail = _db.UserIdentities.First(uid => uid.DeviceID == spvr.UserID).UserEmail,
                    FarmNumber = pvr.FarmNumber,
                    Local = false,
                    PrimaryReasonCode = pvr.ReasonCodes[0],
                    VisitDate = pvr.VisitDate,
                    Uploaded = true
                };
            }).ToList();

            return Json(rlis, JsonRequestBehavior.AllowGet);
        }

        private ProducerVisitReport Hydrated(StoredProducerVisitReport spvr)
        {
            var vxrs = _db.VisitXReason.Where(vxr => vxr.VisitID == spvr.ID).ToList();
            var ids = vxrs.Select(vxr => vxr.ReasonID).ToList();
            var rcs = _db.ReasonCodes.Where(rc => ids.Contains(rc.ID)).ToArray();
            return spvr.Hydrate(rcs);
        }

        [HttpPost]
        public ActionResult Log(ProducerVisitReport report)
        {
            report.ID = 0;
            var spvr = new StoredProducerVisitReport(report);
            _db.ProducerVisitReports.Add(spvr);
            _db.SaveChanges();
            if (report.ReasonCodes != null)
            {
                foreach (var rc in report.ReasonCodes)
                {
                    _db.VisitXReason.Add(new VisitXReason {ReasonID = rc.ID, VisitID = spvr.ID});
                }
                _db.SaveChanges();
            }
            return Content("Success");
        }

        [HttpPost]
        public ActionResult Identity(UserIdentity report)
        {
            _db.UserIdentities.Add(report);
            _db.SaveChanges();
            return Content("Success");
        }

        public ActionResult Reasons(string id)
        {
            if (!_db.ReasonCodes.Any())
            {
                var list = new List<ReasonCode>(new[]
                {
                    new ReasonCode {Name = "Regulatory - Inspection", Code = 51},
                    new ReasonCode {Name = "Regulatory - Hi-Count / Antibiotic", Code = 11},
                    new ReasonCode {Name = "Regulatory - Calibrations", Code = 12},
                    new ReasonCode {Name = "Regulatory - Samples", Code = 15},
                    new ReasonCode {Name = "Membership - Relationship Call", Code = 55},
                    new ReasonCode {Name = "Membership - Solicitation", Code = 56},
                    new ReasonCode {Name = "Membership - Cancellation", Code = 50},
                    new ReasonCode {Name = "Farm Services - Agri-Max", Code = 31},
                    new ReasonCode {Name = "Farm Services - ASA - Insurance", Code = 32},
                    new ReasonCode {Name = "Farm Services - Dairy One", Code = 33},
                    new ReasonCode {Name = "Farm Services - Eagle - Farm Supplies", Code = 34},
                    new ReasonCode {Name = "Farm Services - Empire Livestock", Code = 35},
                    new ReasonCode {Name = "Farm Services - Risk Management", Code = 36},
                    new ReasonCode {Name = "Other", Code = -1},
                });
                foreach (var reasonCode in list)
                {
                    _db.ReasonCodes.Add(reasonCode);
                }
                _db.SaveChanges();
            }
            return Json(_db.ReasonCodes.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Report(string id)
        {
            return Json(Hydrated(_db.ProducerVisitReports.Find(int.Parse(id))), JsonRequestBehavior.AllowGet);

        }

        public ActionResult CallTypes(string id)
        {
            return Json(new List<string>(new[]
                {
                    "Farm Visit",
                    "Phone Call",
                    "Email",
                    "Farm Show",
                    "Other"
                }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmailRecipients(string id)
        {
            return Json(new List<string>(new[]
            {
                "info@agri-maxfinancial.com",
                "info@agri-servicesagency.com",
                "communications@dairylea.com",
                "FieldStaffNotification-DairyOne@DairyOne.com",
                "FieldStaffNotification-DMS@dairylea.com",
                "drms@dairylea.com",
                "FieldStaffNotification-Eagle@dairylea.com",
                "FieldStaffNotification-HR@dairylea.com",
                "technicalsupport-brittonfield@dairylea.com",
                "FieldStaffNotification-Membership@dairylea.com",
                "FieldStaffNotification-Payroll@dairylea.com",
                "Recipients Not Listed"
            }), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}