﻿using BottomhalfCore.DatabaseLayer.Common.Code;
using ModalLayer.Modal;
using ModalLayer.Modal.Accounts;
using ServiceLayer.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ServiceLayer.Code
{
    public class SettingService : ISettingService
    {
        private readonly IDb _db;
        private readonly CurrentSession _currentSession;
        public SettingService(IDb db, CurrentSession currentSession)
        {
            _db = db;
            _currentSession = currentSession;
        }

        public string AddUpdateComponentService(SalaryComponents salaryComponents)
        {
            salaryComponents = _db.Get<SalaryComponents>("");
            return null;
        }

        public dynamic GetSalaryComponentService()
        {
            List<SalaryComponents> salaryComponents = _db.GetList<SalaryComponents>("sp_fixed_salary_component_percent_get");
            PfEsiSetting pfEsiSettings = _db.Get<PfEsiSetting>("sp_pf_esi_setting_get");
            var value = new { SalaryComponent = salaryComponents, PfEsiSettings = pfEsiSettings };
            return value;
        }

        public string PfEsiSetting(SalaryComponents PfSetting, SalaryComponents EsiSetting, PfEsiSetting PfesiSetting)
        {
            string value = string.Empty;
            List<SalaryComponents> salaryComponents = _db.GetList<SalaryComponents>("sp_fixed_salary_component_percent_get");
            var pfsetting = salaryComponents.Find(x => x.ComponentId == PfSetting.ComponentId);
            var esisetting = salaryComponents.Find(x => x.ComponentId == EsiSetting.ComponentId);
            DbParam[] param = new DbParam[]
            {
                new DbParam (PfSetting.ComponentId, typeof(string), "_ComponentId"),
                new DbParam (PfSetting.CalculateInPercentage, typeof(bool), "_CalculateInPercentage"),
                new DbParam (pfsetting.EmployeeContribution, typeof(decimal), "_EmployeeContribution"),
                new DbParam (PfSetting.IsDeductions, typeof(bool), "_IsDeductions"),
                new DbParam (PfSetting.IsActive, typeof(bool), "_IsActive"),
                new DbParam (PfSetting.IncludeInPayslip, typeof(bool), "_IncludeInPayslip"),
                new DbParam (pfsetting.ComponentDescription, typeof(string), "_ComponentDescription"),
                new DbParam (pfsetting.Amount, typeof(decimal), "_Amount"),
                new DbParam (PfSetting.EmployerContribution, typeof(decimal), "_EmployerContribution"),
                new DbParam (pfsetting.IsOpted, typeof(bool), "_IsOpted"),
                new DbParam (pfsetting.PercentageValue, typeof(decimal), "_PercentageValue"),
                new DbParam (_currentSession.CurrentUserDetail.UserId, typeof(long), "_Admin")
            };
            value = _db.ExecuteNonQuery("sp_fixed_salary_component_percent_insupd", param, true);
            if (string.IsNullOrEmpty(value))
                throw new HiringBellException("Unable to update PF Setting.");

            param = new DbParam[]
            {
                new DbParam (EsiSetting.ComponentId, typeof(string), "_ComponentId"),
                new DbParam (esisetting.CalculateInPercentage, typeof(bool), "_CalculateInPercentage"),
                new DbParam (EsiSetting.EmployeeContribution, typeof(decimal), "_EmployeeContribution"),
                new DbParam (EsiSetting.IsDeductions, typeof(bool), "_IsDeductions"),
                new DbParam (EsiSetting.IsActive, typeof(bool), "_IsActive"),
                new DbParam (EsiSetting.IncludeInPayslip, typeof(bool), "_IncludeInPayslip"),
                new DbParam (esisetting.ComponentDescription, typeof(string), "_ComponentDescription"),
                new DbParam (EsiSetting.Amount, typeof(decimal), "_Amount"),
                new DbParam (EsiSetting.EmployerContribution, typeof(decimal), "_EmployerContribution"),
                new DbParam (esisetting.IsOpted, typeof(bool), "_IsOpted"),
                new DbParam (esisetting.PercentageValue, typeof(decimal), "_PercentageValue"),
                new DbParam (_currentSession.CurrentUserDetail.UserId, typeof(long), "_Admin")
            };
            value = _db.ExecuteNonQuery("sp_fixed_salary_component_percent_insupd", param, true);
            if (string.IsNullOrEmpty(value))
                throw new HiringBellException("Unable to update PF Setting.");

            param = new DbParam[]
            {
                new DbParam (PfesiSetting.PfEsi_setting_Id, typeof(int), "_PfEsi_setting_Id"),
                new DbParam (PfesiSetting.PF_Limit_Amount_Statutory, typeof(bool), "_PF_Limit_Amount_Statutory"),
                new DbParam (PfesiSetting.PF_Allow_overriding, typeof(bool), "_PF_Allow_overriding"),
                new DbParam (PfesiSetting.PF_EmployerContribution_Outside_GS, typeof(bool), "_PF_EmployerContribution_Outside_GS"),
                new DbParam (PfesiSetting.PF_OtherChgarges_Outside_GS, typeof(bool), "_PF_OtherChgarges_Outside_GS"),
                new DbParam (PfesiSetting.PF_Employess_Contribute_VPF, typeof(bool), "_PF_Employess_Contribute_VPF"),
                new DbParam (PfesiSetting.ESI_Allow_overriding, typeof(bool), "_ESI_Allow_overriding"),
                new DbParam (PfesiSetting.ESI_EmployerContribution_Outside_GS, typeof(bool), "_ESI_EmployerContribution_Outside_GS"),
                new DbParam (PfesiSetting.ESI_Exclude_EmployerShare_fromGross, typeof(bool), "_ESI_Exclude_EmployerShare_fromGross"),
                new DbParam (PfesiSetting.ESI_Exclude_EmpGratuity_fromGross, typeof(bool), "_ESI_Exclude_EmpGratuity_fromGross"),
                new DbParam (PfesiSetting.ESI_Restrict_Statutory, typeof(bool), "_ESI_Restrict_Statutory"),
                new DbParam (PfesiSetting.ESI_IncludeBonuses_OTP_inGross_Eligibility, typeof(bool), "_ESI_IncludeBonuses_OTP_inGross_Eligibility"),
                new DbParam (PfesiSetting.ESI_IncludeBonuses_OTP_inGross_Calculation, typeof(bool), "_ESI_IncludeBonuses_OTP_inGross_Calculation"),
                new DbParam (PfesiSetting.PF_IsEmployerPFLimit, typeof(bool), "_PF_IsEmployerPFLimit"),
                new DbParam (_currentSession.CurrentUserDetail.UserId, typeof(long), "_Admin")
            };
            value = _db.ExecuteNonQuery("sp_pf_esi_setting_insupd", param, true);
            if (string.IsNullOrEmpty(value))
                throw new HiringBellException("Unable to update PF Setting.");
            return value;
        }
    }
}
