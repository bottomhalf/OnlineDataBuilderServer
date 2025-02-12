﻿using ModalLayer.Modal.Accounts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceLayer.Interface
{
    public interface ISettingService
    {
        string AddUpdateComponentService(SalaryComponents salaryComponents);
        PfEsiSetting GetSalaryComponentService(int CompanyId);
        PfEsiSetting PfEsiSetting(int CompanyId, PfEsiSetting PfesiSetting);
        List<OrganizationDetail> GetOrganizationInfo();
        BankDetail GetOrganizationBankDetailInfoService(int organizationId);
        string InsertUpdatePayrollSetting(Payroll payroll);
        Payroll GetPayrollSetting(int companyId);
        string InsertUpdateSalaryStructure(List<SalaryStructure> salaryStructure);
        Task<List<SalaryComponents>> ActivateCurrentComponentService(List<SalaryComponents> components);
        string UpdateGroupSalaryComponentDetailService(string componentId, int groupId,SalaryComponents component);
        List<SalaryComponents> EnableSalaryComponentDetailService(string componentId, SalaryComponents component);
        List<SalaryComponents> FetchComponentDetailByIdService(int componentTypeId);
        List<SalaryComponents> FetchActiveComponentService();
    }
}
