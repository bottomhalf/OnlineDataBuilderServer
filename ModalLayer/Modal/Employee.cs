﻿using System;

namespace ModalLayer.Modal
{
    public class Employee : EmployeePersonalDetail
    {
        public long EmployeeLoginId { set; get; }        
        public int OrganizationId { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public new int CompanyId { set; get; }
        public string PANNo { set; get; }
        public string AadharNo { set; get; }
        public string AccountNumber { set; get; }
        public string BankName { set; get; }
        public string BranchName { set; get; }
        public string IFSCCode { set; get; }
        public string Domain { set; get; }
        public string Specification { set; get; }
        public decimal ExprienceInYear { set; get; }
        public string LastCompanyName { set; get; }
        public int Index { set; get; }
        public bool IsActive { set; get; }
        public int Total { set; get; }
        public int RowIndex { set; get; }
        public DateTime DOB { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ProfessionalDetail_Json { get; set; }
        public string ClientJson { set; get; }
        public long EmpProfDetailUid { set; get; }
        public decimal ExperienceInYear { set; get; }
        public int DesignationId { set; get; }
        public int AccessLevelId { set; get; }
        public int UserTypeId { set; get; } = 2;
        public int LeavePlanId { get; set; }
        public int ProbationPeriodDaysLimit { get; set; }
        public int NoticePeriodDaysLimit { get; set; }
        public int PayrollGroupId { get; set; }
        public int SalaryGroupId { get; set; }
        public int NoticePeriodId { get; set; }
        public DateTime? NoticePeriodAppliedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string OldFileName { get; set; }
    }

    public class EmployeeEmailMobileCheck
    {
        public int EmployeeCount { get; set; } = 0;
        public int MobileCount { get; set; } = 0;
        public int EmailCount { get; set; } = 0;
    }
}
