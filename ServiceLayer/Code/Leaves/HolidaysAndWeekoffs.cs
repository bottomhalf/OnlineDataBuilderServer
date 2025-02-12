﻿using BottomhalfCore.Services.Interface;
using ModalLayer;
using ModalLayer.Modal;
using ModalLayer.Modal.Leaves;
using ServiceLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer.Code.Leaves
{
    public class HolidaysAndWeekoffs : IHolidaysAndWeekoffs
    {
        private readonly ITimezoneConverter _timezoneConverter;
        private readonly CurrentSession _currentSession;
        private LeavePlanConfiguration _leavePlanConfiguration;
        private readonly ICompanyCalendar _companyCalendar;

        public HolidaysAndWeekoffs(
            ITimezoneConverter timezoneConverter,
            ICompanyCalendar companyCalendar,
            CurrentSession currentSession)
        {
            _timezoneConverter = timezoneConverter;
            _currentSession = currentSession;
            _companyCalendar = companyCalendar;
        }

        public async Task CheckHolidayWeekOffRules(LeaveCalculationModal leaveCalculationModal)
        {
            _leavePlanConfiguration = leaveCalculationModal.leavePlanConfiguration;

            await CheckAdjoiningHolidyOnLeave(leaveCalculationModal);

            await CheckAdjoiningWeekOffOnLeave(leaveCalculationModal);

            await Task.CompletedTask;
        }

        // step - 1 -- adjoining holiday
        private async Task CheckAdjoiningHolidyOnLeave(LeaveCalculationModal leaveCalculationModal)
        {
            List<Calendar> holidays = new List<Calendar>();
            DateTime newFromDate = leaveCalculationModal.utcFromDate;
            DateTime newToDate = leaveCalculationModal.utcToDate;

            if (_leavePlanConfiguration.leaveHolidaysAndWeekoff.AdJoiningHolidayIsConsiderAsLeave)
            {
                // Yes
                bool flag = false;
                var totalDays = newToDate.Date.Subtract(newFromDate.Date).TotalDays + 1;
                if (totalDays >= (double)_leavePlanConfiguration.leaveHolidaysAndWeekoff.ConsiderLeaveIfNumOfDays)
                {
                    // for below condition in case of true consider all days as leave
                    // if (_leavePlanConfiguration.leaveHolidaysAndWeekoff.IfLeaveLieBetweenTwoHolidays)

                    if (_leavePlanConfiguration.leaveHolidaysAndWeekoff.IfHolidayIsRightBeforLeave)
                    {
                        holidays = await _companyCalendar.GetHolidayBetweenTwoDates(newFromDate, newToDate.AddDays(1));
                        flag = await _companyCalendar.IsHoliday(newToDate.AddDays(1));
                        if (flag)
                        {
                            // newFromDate = leaveCalculationModal.fromDate;
                            newToDate = newToDate.AddDays(1);
                        }
                    }
                    else if (_leavePlanConfiguration.leaveHolidaysAndWeekoff.IfHolidayIsRightAfterLeave)
                    {
                        holidays = await _companyCalendar.GetHolidayBetweenTwoDates(newFromDate.AddDays(-1), newFromDate);
                        flag = await _companyCalendar.IsHoliday(newFromDate.AddDays(-1));
                        if (flag)
                        {
                            newFromDate = newFromDate.AddDays(-1);
                            // newToDate = leaveCalculationModal.toDate;
                        }
                    }
                    else if (_leavePlanConfiguration.leaveHolidaysAndWeekoff.IfHolidayIsRightBeforeAfterOrInBetween)
                    {
                        flag = await _companyCalendar.IsHoliday(newFromDate.AddDays(-1));
                        if (flag)
                        {
                            newFromDate = newFromDate.AddDays(-1);
                            // newToDate = leaveCalculationModal.toDate;
                        }

                        flag = await _companyCalendar.IsHoliday(newToDate.AddDays(1));
                        if (flag)
                        {
                            newToDate = newToDate.AddDays(1);
                        }
                    }
                }
            }
            else
            {
                // No = take only week days don't consider weekends as leave
                holidays = await _companyCalendar.GetHolidayBetweenTwoDates(newFromDate, newToDate);
            }

            var appliedDays = newToDate.Date.Subtract(newFromDate.Date).TotalDays;
            if (holidays.Count == 0)
                appliedDays = appliedDays + 1;

            await RemoveHolidaysIfApplicable(leaveCalculationModal, holidays.Count() + appliedDays);
            await Task.CompletedTask;
        }

        // step - 2 -- adjoining weekoff
        private async Task CheckAdjoiningWeekOffOnLeave(LeaveCalculationModal leaveCalculationModal)
        {
            bool flag = false;

            var leaveDaysInWeek = 7 - leaveCalculationModal.companySetting.WorkingDaysInAWeek;
            var totalDays = leaveCalculationModal.toDate.Date.Subtract(leaveCalculationModal.fromDate.Date).TotalDays + 1;
            if (totalDays >= (double)_leavePlanConfiguration.leaveHolidaysAndWeekoff.ConsiderLeaveIfIncludeDays)
            {
                // if this condition is true then calculate all days
                // if (_leavePlanConfiguration.leaveHolidaysAndWeekoff.IfLeaveLieBetweenWeekOff)

                if (_leavePlanConfiguration.leaveHolidaysAndWeekoff.IfWeekOffIsRightBeforLeave)
                {
                    await RemoveWeekOffIfApplicable(leaveCalculationModal);
                    flag = await _companyCalendar.IsWeekOff(leaveCalculationModal.toDate.AddDays(1));
                    if (flag)
                    {
                        leaveCalculationModal.numberOfLeaveApplyring += leaveDaysInWeek;
                    }
                }
                else if (_leavePlanConfiguration.leaveHolidaysAndWeekoff.IfWeekOffIsRightAfterLeave)
                {
                    await RemoveWeekOffIfApplicable(leaveCalculationModal);
                    flag = await _companyCalendar.IsWeekOff(leaveCalculationModal.fromDate.AddDays(-1));
                    if (flag)
                    {
                        leaveCalculationModal.numberOfLeaveApplyring += leaveDaysInWeek;
                    }
                }
                else if (_leavePlanConfiguration.leaveHolidaysAndWeekoff.IfWeekOffIsRightBeforeAfterOrInBetween)
                {
                    var leavesCount = leaveCalculationModal.fromDate.Date.Subtract(leaveCalculationModal.toDate.Date).TotalDays + 1;

                    flag = await _companyCalendar.IsWeekOff(leaveCalculationModal.fromDate.AddDays(-1));
                    if (flag)
                    {
                        leaveCalculationModal.numberOfLeaveApplyring += leaveDaysInWeek;
                    }

                    flag = await _companyCalendar.IsWeekOff(leaveCalculationModal.toDate.AddDays(1));
                    if (flag)
                    {
                        leaveCalculationModal.numberOfLeaveApplyring += leaveDaysInWeek;
                    }

                    leaveCalculationModal.numberOfLeaveApplyring += (decimal)leavesCount;
                }
            }
            else
            {
                await RemoveWeekOffIfApplicable(leaveCalculationModal);
            }

            await Task.CompletedTask;
        }

        private async Task RemoveWeekOffIfApplicable(LeaveCalculationModal leaveCalculationModal)
        {
            var fromDate = leaveCalculationModal.fromDate;
            var toDate = leaveCalculationModal.toDate;

            leaveCalculationModal.numberOfLeaveApplyring = 0;
            while (toDate.Subtract(fromDate).TotalDays >= 0)
            {
                if (fromDate.DayOfWeek != DayOfWeek.Saturday && fromDate.DayOfWeek != DayOfWeek.Sunday)
                    leaveCalculationModal.numberOfLeaveApplyring++;

                fromDate = fromDate.AddDays(1);
            }

            await Task.CompletedTask;
        }

        private async Task RemoveHolidaysIfApplicable(LeaveCalculationModal leaveCalculationModal, double appliedDays)
        {
            leaveCalculationModal.numberOfLeaveApplyring = 0;
            leaveCalculationModal.numberOfLeaveApplyring = (decimal)appliedDays;

            await Task.CompletedTask;
        }
    }
}
