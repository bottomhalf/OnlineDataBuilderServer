﻿using BottomhalfCore.DatabaseLayer.Common.Code;
using BottomhalfCore.Services.Interface;
using ModalLayer.Modal;
using ServiceLayer.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceLayer.Code.SendEmail
{
    public class AttendanceEmailService
    {
        private readonly IDb _db;
        private readonly CurrentSession _currentSession;
        private readonly IEmailService _emailService;
        private readonly ITimezoneConverter _timezoneConverter;

        public AttendanceEmailService(
            IDb db,
            CurrentSession currentSession,
            ITimezoneConverter timezoneConverter,
            IEmailService emailService)
        {
            _db = db;
            _currentSession = currentSession;
            _timezoneConverter = timezoneConverter;
            _emailService = emailService;
        }

        public async Task SendSubmitAttendanceEmail(AttendenceDetail attendenceApplied)
        {
            var emailRequestModal = await GetTemplate(attendenceApplied);
            await _emailService.SendEmailWithTemplate(ApplicationConstants.AttendanceSubmittedEmailTemplate, emailRequestModal);
        }

        private async Task<TemplateReplaceModal> GetTemplate(AttendenceDetail attendenceApplied)
        {
            var fromDate = _timezoneConverter.ToTimeZoneDateTime((DateTime)attendenceApplied.AttendenceFromDay, _currentSession.TimeZone);
            var toDate = _timezoneConverter.ToTimeZoneDateTime((DateTime)attendenceApplied.AttendenceToDay, _currentSession.TimeZone);
            long reportManagerId = 0;
            if (_currentSession.CurrentUserDetail.ReportingManagerId == 0)
                reportManagerId = 1;
            else
                reportManagerId = _currentSession.CurrentUserDetail.ReportingManagerId;
            FilterModel filterModel = new FilterModel
            {
                SearchString = $"1=1 and EmployeeUid = {reportManagerId}",
                SortBy = "",
                PageIndex = 1,
                PageSize = 10
            };

            var managerDetail = _db.Get<Employee>("SP_Employees_Get", filterModel);
            if (managerDetail == null)
                throw new Exception("No manager record found. Please add manager first.");

            var numOfDays = fromDate.Date.Subtract(toDate.Date).TotalDays + 1;

            var emailRequestModal = new TemplateReplaceModal
            {
                DeveloperName = _currentSession.CurrentUserDetail.FullName,
                ManagerName = managerDetail.ManagerName,
                ToAddress = new List<string> { managerDetail.Email },
                RequestType = ApplicationConstants.DailyAttendance,
                FromDate = fromDate,
                ToDate = toDate,
                DayCount = Convert.ToInt32(numOfDays),
                ActionType = nameof(ItemStatus.Submitted),
                Message = string.IsNullOrEmpty(attendenceApplied.UserComments)
                                    ? "NA"
                                    : attendenceApplied.UserComments
            };

            return await Task.FromResult(emailRequestModal);
        }
    }
}
