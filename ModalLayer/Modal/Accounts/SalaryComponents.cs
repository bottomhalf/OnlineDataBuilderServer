﻿namespace ModalLayer.Modal.Accounts
{
    public class SalaryComponents : SalaryCommon
    {
        public string ComponentId { set; get; }
        public int ComponentTypeId { get; set; }
        public decimal PercentageValue { set; get; }
        public decimal EmployeeContribution { set; get; }
        public decimal EmployerContribution { set; get; }
        public bool IncludeInPayslip { set; get; }
        public bool IsOpted { set; get; }
        public bool IsActive { set; get; }
        public int AdHocId { get; set; }
        public bool IsAdHoc { get; set; }
    }

    public class SalaryCommon : CreationInfo
    {
        public bool CalculateInPercentage { set; get; }
        public string ComponentDescription { set; get; }
        public bool IsComponentEnabled { set; get; }
        public decimal MaxLimit { set; get; }
        public string Formula { set; get; }
        public string Section { get; set; }
        public decimal SectionMaxLimit { get; set; }
        public bool IsAffectInGross { get; set; }
        public bool RequireDocs { get; set; }
        public string TaxExempt { get; set; }


    }
}
