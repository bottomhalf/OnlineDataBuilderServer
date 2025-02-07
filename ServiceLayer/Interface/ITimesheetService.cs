﻿using ModalLayer.Modal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceLayer.Interface
{
    public interface ITimesheetService
    {
        dynamic GetTimesheetByUserIdService(TimesheetDetail timesheetDetail);
        Task<List<DailyTimesheetDetail>> InsertUpdateTimesheet(List<DailyTimesheetDetail> dailyTimesheetDetail);
        List<TimesheetDetail> GetPendingTimesheetByIdService(long employeeId, long clientId);
        dynamic GetEmployeeTimeSheetService(TimesheetDetail timesheetDetail);
        void UpdateTimesheetService(List<DailyTimesheetDetail> dailyTimesheetDetails, TimesheetDetail timesheetDetail, string comment);
        (List<DailyTimesheetDetail>, List<DateTime>) BuildFinalTimesheet(TimesheetDetail currentTimesheetDetail);
        BillingDetail EditEmployeeBillDetailService(GenerateBillFileDetail fileDetail);
    }
}
