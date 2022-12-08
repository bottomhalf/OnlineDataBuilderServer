﻿using System;
using System.Collections.Generic;

namespace ModalLayer.Modal.Accounts
{
    public class CompanySetting
    {
        public CompanySetting()
        {
            SetupRecommandedWeekEnds();
        }

        private void SetupRecommandedWeekEnds()
        {
            OfficialWeekOffDays = new List<DayOfWeek>();
            OfficialWeekOffDays.Add(DayOfWeek.Saturday);
            OfficialWeekOffDays.Add(DayOfWeek.Sunday);
        }

        public int SettingId { set; get; }
        public int CompanyId { set; get; }
        public int ProbationPeriodInDays { set; get; }
        public int NoticePeriodInDays { set; get; }
        public int FinancialYear { set; get; }
        public int DeclarationStartMonth { set; get; }
        public int DeclarationEndMonth { set; get; }
        public bool IsPrimary { set; get; }
        public List<ManagerLevel> DefaultManager { get; set; }
        // This value will come from database and filled by admin using page.
        public int EveryMonthLastDayOfDeclaration { set; get; } = 20;
        public bool IsUseInternationalWeekDays { set; get; } = true;
        public List<DayOfWeek> OfficialWeekOffDays { set; get; }
        public bool IsAccrualLeaveForNoticePeriodOnly { set; get; } // override all rule and allow leave for 2 or 3 months (define as per rule) leaves only.
        public bool IsAccrualLeaveForProbationPeriondOnly { set; get; } // override all rule and allow leave for 2 or 3 months (define as per rule) leaves only.
    }
    public class ManagerLevel
    {
        public int DesignationId { get; set; }
    }
}
