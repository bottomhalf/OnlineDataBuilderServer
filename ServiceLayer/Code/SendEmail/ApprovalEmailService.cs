﻿using ModalLayer.Modal;
using ModalLayer.Modal.Leaves;
using ServiceLayer.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer.Code.SendEmail
{
    public class ApprovalEmailService
    {
        private readonly IEmailService _emailService;
        private readonly CurrentSession _currentSession;

        public ApprovalEmailService(IEmailService emailService, CurrentSession currentSession)
        {
            _emailService = emailService;
            _currentSession = currentSession;
        }

        #region ATTENDANCE APPROVAL
        private async Task<TemplateReplaceModal> GetAttendanceApprovalTemplate(AttendanceDetails attendanceDetails, ItemStatus status)
        {
            var templateReplaceModal = new TemplateReplaceModal
            {
                DeveloperName = attendanceDetails.EmployeeName,
                RequestType = ApplicationConstants.WorkFromHome,
                ToAddress = new List<string> { attendanceDetails.Email },
                ActionType = status.ToString(),
                FromDate = attendanceDetails.AttendanceDay,
                ToDate = attendanceDetails.AttendanceDay,
                ManagerName = _currentSession.CurrentUserDetail.FullName,
                Message = string.IsNullOrEmpty(attendanceDetails.UserComments)
                    ? "NA"
                    : attendanceDetails.UserComments,
            };

            return await Task.FromResult(templateReplaceModal);
        }

        public async Task AttendaceApprovalStatusSendEmail(AttendanceDetails attendanceDetails, ItemStatus status)
        {
            var templateReplaceModal = await GetAttendanceApprovalTemplate(attendanceDetails, status);
            await _emailService.SendEmailWithTemplate(ApplicationConstants.AttendanceApprovalStatusEmailTemplate, templateReplaceModal);
            await Task.CompletedTask;
        }

        #endregion

        #region LEAVE APPROVAL

        private async Task<TemplateReplaceModal> GetLeaveApprovalTemplate(LeaveRequestDetail leaveRequestDetail, ItemStatus status)
        {
            var templateReplaceModal = new TemplateReplaceModal
            {
                DeveloperName = leaveRequestDetail.FirstName + " " + leaveRequestDetail.LastName,
                RequestType = ApplicationConstants.Leave,
                ToAddress = new List<string> { leaveRequestDetail.Email },
                ActionType = status.ToString(),
                FromDate = leaveRequestDetail.LeaveFromDay,
                ToDate = leaveRequestDetail.LeaveToDay,
                LeaveType = leaveRequestDetail.LeaveToDay.ToString(),
                ManagerName = _currentSession.CurrentUserDetail.FullName,
                Message = string.IsNullOrEmpty(leaveRequestDetail.Reason)
                                    ? "NA"
                                    : leaveRequestDetail.Reason
            };

            return await Task.FromResult(templateReplaceModal);
        }

        public async Task LeaveApprovalStatusSendEmail(LeaveRequestDetail leaveRequestDetail, ItemStatus status)
        {
            var templateReplaceModal = await GetLeaveApprovalTemplate(leaveRequestDetail, status);
            await _emailService.SendEmailWithTemplate(ApplicationConstants.AttendanceApprovalStatusEmailTemplate, templateReplaceModal);
            await Task.CompletedTask;
        }

        #endregion

        #region TIMESHEET APPROVAL

        private async Task<TemplateReplaceModal> GetTimesheetApprovalTemplate(DailyTimesheetDetail dailyTimesheetDetail, List<DailyTimesheetDetail> dailyTimesheetDetails, ItemStatus status)
        {
            var sortedTimesheetByDate = dailyTimesheetDetails.OrderByDescending(x => x.PresentDate);
            var templateReplaceModal = new TemplateReplaceModal
            {
                DeveloperName = dailyTimesheetDetail.EmployeeName,
                RequestType = ApplicationConstants.Timesheet,
                ToAddress = new List<string> { dailyTimesheetDetail.Email },
                ActionType = status.ToString(),
                FromDate = sortedTimesheetByDate.First().PresentDate,
                ToDate = sortedTimesheetByDate.Last().PresentDate,
                LeaveType = null,
                ManagerName = _currentSession.CurrentUserDetail.FullName,
                Message = string.IsNullOrEmpty(dailyTimesheetDetail.UserComments)
                            ? "NA"
                            : dailyTimesheetDetail.UserComments,
            };

            return await Task.FromResult(templateReplaceModal);
        }

        public async Task TimesheetApprovalStatusSendEmail(DailyTimesheetDetail dailyTimesheetDetail, List<DailyTimesheetDetail> dailyTimesheetDetails, ItemStatus status)
        {
            var templateReplaceModal = await GetTimesheetApprovalTemplate(dailyTimesheetDetail, dailyTimesheetDetails, status);
            await _emailService.SendEmailWithTemplate(ApplicationConstants.TimesheetApprovalStatusEmailTemplate, templateReplaceModal);
            await Task.CompletedTask;
        }

        #endregion
    }
}
