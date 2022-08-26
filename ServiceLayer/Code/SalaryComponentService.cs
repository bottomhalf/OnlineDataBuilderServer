﻿using BottomhalfCore.DatabaseLayer.Common.Code;
using ModalLayer.Modal;
using ModalLayer.Modal.Accounts;
using ServiceLayer.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Data;
using BottomhalfCore.Services.Code;
using BottomhalfCore.Services.Interface;

namespace ServiceLayer.Code
{
    public class SalaryComponentService : ISalaryComponentService
    {
        private readonly IDb _db;
        private readonly CurrentSession _currentSession;
        private readonly IEvaluationPostfixExpression _postfixToInfixConversion;
        private readonly ITimezoneConverter _timezoneConverter;

        public SalaryComponentService(IDb db, CurrentSession currentSession,
            IEvaluationPostfixExpression postfixToInfixConversion,
            ITimezoneConverter timezoneConverter
            )
        {
            _db = db;
            _currentSession = currentSession;
            _postfixToInfixConversion = postfixToInfixConversion;
            _timezoneConverter = timezoneConverter;
        }

        public SalaryComponents GetSalaryComponentByIdService()
        {
            throw new NotImplementedException();
        }

        public List<SalaryComponents> GetSalaryComponentsDetailService()
        {
            List<SalaryComponents> salaryComponents = _db.GetList<SalaryComponents>("sp_salary_components_get", false);
            return salaryComponents;
        }

        public List<SalaryGroup> GetSalaryGroupService()
        {
            List<SalaryGroup> salaryComponents = _db.GetList<SalaryGroup>("sp_salary_group_getAll", false);
            return salaryComponents;
        }

        public SalaryGroup GetSalaryGroupsByIdService(int SalaryGroupId)
        {
            if (SalaryGroupId <= 0)
                throw new HiringBellException("Invalid SalaryGroupId");
            SalaryGroup salaryGroup = _db.Get<SalaryGroup>("sp_salary_group_getById", new { SalaryGroupId });
            return salaryGroup;
        }

        public List<SalaryComponents> UpdateSalaryComponentService(List<SalaryComponents> salaryComponents)
        {
            if (salaryComponents.Count > 0)
            {
                List<SalaryComponents> result = _db.GetList<SalaryComponents>("sp_salary_components_get", false);
                Parallel.ForEach(result, x =>
                {
                    var item = salaryComponents.Find(i => i.ComponentId == x.ComponentId);
                    if (item != null)
                    {
                        x.IsActive = item.IsActive;
                        x.Formula = item.Formula;
                        x.CalculateInPercentage = item.CalculateInPercentage;
                    }
                });


                var itemOfRows = (from n in result
                                  select new
                                  {
                                      n.ComponentId,
                                      n.ComponentFullName,
                                      n.ComponentDescription,
                                      n.CalculateInPercentage,
                                      n.TaxExempt,
                                      n.ComponentTypeId,
                                      n.ComponentCatagoryId,
                                      n.PercentageValue,
                                      n.MaxLimit,
                                      n.DeclaredValue,
                                      n.RejectedAmount,
                                      n.AcceptedAmount,
                                      n.UploadedFileIds,
                                      n.Formula,
                                      n.EmployeeContribution,
                                      n.EmployerContribution,
                                      n.IncludeInPayslip,
                                      n.IsOpted,
                                      n.IsActive,
                                      Admin = n.CreatedBy,
                                  }).ToList();

                var table = Converter.ToDataTable(itemOfRows);
                _db.BatchInsert("sp_salary_components_insupd", table, true);
            }

            return salaryComponents;
        }

        public List<SalaryComponents> InsertUpdateSalaryComponentsByExcelService(List<SalaryComponents> salaryComponents)
        {
            List<SalaryComponents> finalResult = new List<SalaryComponents>();
            if (salaryComponents.Count > 0)
            {
                List<SalaryComponents> result = _db.GetList<SalaryComponents>("sp_salary_components_get", false);

                foreach (SalaryComponents item in salaryComponents)
                {
                    if (string.IsNullOrEmpty(item.ComponentId) || string.IsNullOrEmpty(item.ComponentFullName))
                        throw new HiringBellException("ComponentId or ComponentFullName is empty.");
                }

                var itemOfRows = (from n in salaryComponents
                                  select new
                                  {
                                      n.ComponentId,
                                      n.ComponentFullName,
                                      n.ComponentDescription,
                                      n.CalculateInPercentage,
                                      n.TaxExempt,
                                      n.ComponentTypeId,
                                      n.ComponentCatagoryId,
                                      n.PercentageValue,
                                      n.MaxLimit,
                                      n.DeclaredValue,
                                      n.AcceptedAmount,
                                      n.RejectedAmount,
                                      n.UploadedFileIds,
                                      n.Formula,
                                      n.EmployeeContribution,
                                      n.EmployerContribution,
                                      n.IncludeInPayslip,
                                      n.IsAdHoc,
                                      n.AdHocId,
                                      n.Section,
                                      n.SectionMaxLimit,
                                      n.IsAffectInGross,
                                      n.RequireDocs,
                                      n.IsOpted,
                                      n.IsActive,
                                      AdminId = _currentSession.CurrentUserDetail.UserId,
                                  }).ToList();

                var table = BottomhalfCore.Services.Code.Converter.ToDataTable(itemOfRows);
                int count = _db.BatchInsert("sp_salary_components_insupd", table, true);
                if (count > 0)
                {
                    foreach (var item in result)
                    {
                        var modified = salaryComponents.Find(x => x.ComponentId == item.ComponentId);
                        if (modified != null)
                        {
                            item.ComponentFullName = modified.ComponentFullName;
                            item.AdHocId = modified.AdHocId;
                            item.AdminId = modified.AdminId;
                            item.ComponentId = modified.ComponentId;
                            item.ComponentDescription = modified.ComponentDescription;
                            item.CalculateInPercentage = modified.CalculateInPercentage;
                            item.TaxExempt = modified.TaxExempt;
                            item.ComponentTypeId = modified.ComponentTypeId;
                            item.ComponentCatagoryId = modified.ComponentCatagoryId;
                            item.PercentageValue = modified.PercentageValue;
                            item.MaxLimit = modified.MaxLimit;
                            item.DeclaredValue = modified.DeclaredValue;
                            item.Formula = modified.Formula;
                            item.EmployeeContribution = modified.EmployeeContribution;
                            item.EmployerContribution = modified.EmployerContribution;
                            item.IncludeInPayslip = modified.IncludeInPayslip;
                            item.IsAdHoc = modified.IsAdHoc;
                            item.Section = modified.Section;
                            item.SectionMaxLimit = modified.SectionMaxLimit;
                            item.IsAffectInGross = modified.IsAffectInGross;
                            item.RequireDocs = modified.RequireDocs;
                            item.IsOpted = modified.IsOpted;
                            item.IsActive = modified.IsActive;

                            finalResult.Add(item);
                        }
                        else
                        {
                            finalResult.Add(item);
                        }
                    }
                }
                else
                {
                    finalResult = result;
                }
            }

            return finalResult;
        }

        public List<SalaryGroup> AddSalaryGroup(SalaryGroup salaryGroup)
        {
            List<SalaryGroup> salaryGroups = _db.GetList<SalaryGroup>("sp_salary_group_getAll", false);
            SalaryGroup salaryGrp = _db.Get<SalaryGroup>("sp_salary_group_getById", new { salaryGroup.SalaryGroupId });
            foreach (SalaryGroup existSalaryGroup in salaryGroups)
            {
                if ((salaryGroup.MinAmount > existSalaryGroup.MinAmount && salaryGroup.MinAmount < existSalaryGroup.MaxAmount) || (salaryGroup.MaxAmount > existSalaryGroup.MinAmount && salaryGroup.MaxAmount < existSalaryGroup.MaxAmount))
                    throw new HiringBellException("Salary group limit already exist");
            }

            List<SalaryComponents> initialSalaryComponents = _db.GetList<SalaryComponents>("sp_salary_group_get_initial_components");

            if (salaryGrp == null)
            {
                salaryGrp = salaryGroup;
                salaryGrp.SalaryComponents = JsonConvert.SerializeObject(initialSalaryComponents);
                salaryGrp.AdminId = _currentSession.CurrentUserDetail.AdminId;
            }
            else
                throw new HiringBellException("Salary Group already exist.");

            var result = _db.Execute<SalaryGroup>("sp_salary_group_insupd", salaryGrp, true);
            if (string.IsNullOrEmpty(result))
                throw new HiringBellException("Fail to insert or update.");
            List<SalaryGroup> value = this.GetSalaryGroupService();
            return value;
        }

        public List<SalaryComponents> AddUpdateRecurringComponents(SalaryStructure recurringComponent)
        {
            if (string.IsNullOrEmpty(recurringComponent.ComponentName))
                throw new HiringBellException("Invalid component name.");

            if (recurringComponent.ComponentTypeId <= 0)
                throw new HiringBellException("Invalid component type.");


            if (recurringComponent.ComponentCatagoryId <= 0)
                throw new HiringBellException("Invalid component type.");

            List<SalaryComponents> components = _db.GetList<SalaryComponents>("sp_salary_components_get");
            var value = components.Find(x => x.ComponentId == recurringComponent.ComponentName);
            if (value == null)
                value = new SalaryComponents();

            value.ComponentId = recurringComponent.ComponentName;
            value.ComponentFullName = recurringComponent.ComponentFullName;
            value.ComponentDescription = recurringComponent.ComponentDescription;
            value.MaxLimit = recurringComponent.MaxLimit;
            value.DeclaredValue = recurringComponent.DeclaredValue;
            value.AcceptedAmount = recurringComponent.AcceptedAmount;
            value.RejectedAmount = recurringComponent.RejectedAmount;
            value.UploadedFileIds = recurringComponent.UploadedFileIds;
            value.TaxExempt = recurringComponent.TaxExempt;
            value.Section = recurringComponent.Section;
            value.ComponentTypeId = recurringComponent.ComponentTypeId;
            value.SectionMaxLimit = recurringComponent.SectionMaxLimit;
            value.ComponentCatagoryId = recurringComponent.ComponentCatagoryId;
            value.AdminId = _currentSession.CurrentUserDetail.AdminId;

            var result = _db.Execute<SalaryComponents>("sp_salary_components_insupd", value, true);
            if (string.IsNullOrEmpty(result))
                throw new HiringBellException("Fail insert salary component.");

            List<SalaryComponents> salaryComponents = this.GetSalaryComponentsDetailService();
            updateSalaryGroupByUdatingComponent(recurringComponent);

            return salaryComponents;
        }

        private void updateSalaryGroupByUdatingComponent(SalaryStructure recurringComponent)
        {
            List<SalaryGroup> salaryGroups = _db.GetList<SalaryGroup>("sp_salary_group_getAll", false);
            foreach (var item in salaryGroups)
            {
                if (string.IsNullOrEmpty(item.SalaryComponents))
                    throw new HiringBellException("Salary component not found");

                List<SalaryComponents> salaryComponents = JsonConvert.DeserializeObject<List<SalaryComponents>>(item.SalaryComponents);
                var component = salaryComponents.Find(x => x.ComponentId == recurringComponent.ComponentName);
                if (component != null)
                {
                    component.ComponentId = recurringComponent.ComponentName;
                    component.ComponentCatagoryId = recurringComponent.ComponentCatagoryId;
                    component.ComponentTypeId = recurringComponent.ComponentTypeId;
                    component.ComponentFullName = recurringComponent.ComponentFullName;
                    component.MaxLimit = recurringComponent.MaxLimit;
                    component.ComponentDescription = recurringComponent.ComponentDescription;
                    component.TaxExempt = recurringComponent.TaxExempt;
                    component.Section = recurringComponent.Section;
                    component.SectionMaxLimit = recurringComponent.SectionMaxLimit;
                }
                item.SalaryComponents = JsonConvert.SerializeObject(salaryComponents);
            }
            var table = Converter.ToDataTable(salaryGroups);
            var result = _db.BatchInsert("sp_salary_group_insupd", table, true);
            if (result <= 0)
                throw new HiringBellException("Fail to insert or update salary group.");
        }

        public List<SalaryComponents> AddAdhocComponents(SalaryStructure adhocComponent)
        {
            if (string.IsNullOrEmpty(adhocComponent.ComponentName))
                throw new HiringBellException("Invalid AdHoc component name.");
            if (adhocComponent.AdHocId <= 0)
                throw new HiringBellException("Invalid AdHoc type component.");
            List<SalaryComponents> adhocComp = _db.GetList<SalaryComponents>("sp_salary_components_get");
            var value = adhocComp.Find(x => x.ComponentId == adhocComponent.ComponentName);
            if (value == null)
            {
                value = new SalaryComponents();
                value.ComponentId = adhocComponent.ComponentName;
                value.ComponentFullName = adhocComponent.ComponentFullName;
                value.ComponentDescription = adhocComponent.ComponentDescription;
                value.MaxLimit = adhocComponent.MaxLimit;
                value.DeclaredValue = adhocComponent.DeclaredValue;
                value.AcceptedAmount = adhocComponent.AcceptedAmount;
                value.RejectedAmount = adhocComponent.RejectedAmount;
                value.UploadedFileIds = adhocComponent.UploadedFileIds;
                value.TaxExempt = adhocComponent.TaxExempt;
                value.Section = adhocComponent.Section;
                value.AdHocId = Convert.ToInt32(adhocComponent.AdHocId);
                value.SectionMaxLimit = adhocComponent.SectionMaxLimit;
                value.IsAdHoc = adhocComponent.IsAdHoc;
                value.AdminId = _currentSession.CurrentUserDetail.AdminId;
            }
            else
                throw new HiringBellException("Component already exist.");

            var result = _db.Execute<SalaryComponents>("sp_salary_components_insupd", value, true);
            if (string.IsNullOrEmpty(result))
                throw new HiringBellException("Fail insert salary component.");

            return this.GetSalaryComponentsDetailService();
        }

        public List<SalaryComponents> AddDeductionComponents(SalaryStructure deductionComponent)
        {
            if (string.IsNullOrEmpty(deductionComponent.ComponentName))
                throw new HiringBellException("Invalid AdHoc component name.");
            if (deductionComponent.AdHocId <= 0)
                throw new HiringBellException("Invalid AdHoc type component.");
            List<SalaryComponents> adhocComp = _db.GetList<SalaryComponents>("sp_salary_components_get");
            var value = adhocComp.Find(x => x.ComponentId == deductionComponent.ComponentName);
            if (value == null)
            {
                value = new SalaryComponents();
                value.ComponentId = deductionComponent.ComponentName;
                value.ComponentFullName = deductionComponent.ComponentFullName;
                value.ComponentDescription = deductionComponent.ComponentDescription;
                value.IsAffectInGross = deductionComponent.IsAffectInGross;
                value.AdHocId = Convert.ToInt32(deductionComponent.AdHocId);
                value.MaxLimit = deductionComponent.MaxLimit;
                value.DeclaredValue = deductionComponent.DeclaredValue;
                value.AcceptedAmount = deductionComponent.AcceptedAmount;
                value.RejectedAmount = deductionComponent.RejectedAmount;
                value.UploadedFileIds = deductionComponent.UploadedFileIds;
                value.IsAdHoc = deductionComponent.IsAdHoc;
                value.AdminId = _currentSession.CurrentUserDetail.AdminId;
            }
            else
                throw new HiringBellException("Deduction Component already exist.");

            var result = _db.Execute<SalaryComponents>("sp_salary_components_insupd", value, true);
            if (string.IsNullOrEmpty(result))
                throw new HiringBellException("Fail insert salary component.");

            return this.GetSalaryComponentsDetailService();
        }

        public List<SalaryComponents> AddBonusComponents(SalaryStructure bonusComponent)
        {
            if (string.IsNullOrEmpty(bonusComponent.ComponentName))
                throw new HiringBellException("Invalid component name.");
            if (bonusComponent.AdHocId <= 0)
                throw new HiringBellException("Invalid AdHoc type component.");

            List<SalaryComponents> bonuses = _db.GetList<SalaryComponents>("sp_salary_components_get");
            var value = bonuses.Find(x => x.ComponentId == bonusComponent.ComponentName);
            if (value == null)
            {
                value = new SalaryComponents();
                value.ComponentId = bonusComponent.ComponentName;
                value.ComponentFullName = bonusComponent.ComponentFullName;
                value.ComponentDescription = bonusComponent.ComponentDescription;
                value.AdHocId = Convert.ToInt32(bonusComponent.AdHocId);
                value.MaxLimit = bonusComponent.MaxLimit;
                value.DeclaredValue = bonusComponent.DeclaredValue;
                value.AcceptedAmount = bonusComponent.AcceptedAmount;
                value.RejectedAmount = bonusComponent.RejectedAmount;
                value.UploadedFileIds = bonusComponent.UploadedFileIds;
                value.IsAdHoc = bonusComponent.IsAdHoc;
                value.AdminId = _currentSession.CurrentUserDetail.AdminId;
            }
            else
                throw new HiringBellException("Bonus Component already exist.");

            var result = _db.Execute<SalaryComponents>("sp_salary_components_insupd", value, true);
            if (string.IsNullOrEmpty(result))
                throw new HiringBellException("Fail insert salary component.");

            return this.GetSalaryComponentsDetailService();
        }

        public List<SalaryGroup> UpdateSalaryGroup(SalaryGroup salaryGroup)
        {
            List<SalaryGroup> salaryGroups = _db.GetList<SalaryGroup>("sp_salary_group_getAll", false);
            SalaryGroup salaryGrp = _db.Get<SalaryGroup>("sp_salary_group_getById", new { salaryGroup.SalaryGroupId });
            foreach (SalaryGroup existSalaryGroup in salaryGroups)
            {
                if ((salaryGroup.MinAmount < existSalaryGroup.MinAmount && salaryGroup.MinAmount > existSalaryGroup.MaxAmount) || (salaryGroup.MaxAmount > existSalaryGroup.MinAmount && salaryGroup.MaxAmount < existSalaryGroup.MaxAmount))
                    throw new HiringBellException("Salary group limit already exist");
            }
            if (salaryGrp == null)
                throw new HiringBellException("Salary Group already exist.");
            else
            {
                salaryGrp = salaryGroup;
                if (string.IsNullOrEmpty(salaryGrp.SalaryComponents))
                    salaryGrp.SalaryComponents = "[]";
                else
                    salaryGrp.SalaryComponents = JsonConvert.SerializeObject(salaryGroup.GroupComponents);
                salaryGrp.AdminId = _currentSession.CurrentUserDetail.AdminId;
            }

            var result = _db.Execute<SalaryGroup>("sp_salary_group_insupd", salaryGrp, true);
            if (string.IsNullOrEmpty(result))
                throw new HiringBellException("Fail to insert or update.");
            List<SalaryGroup> value = this.GetSalaryGroupService();
            return value;
        }

        public List<SalaryComponents> UpdateSalaryGroupComponentService(SalaryGroup salaryGroup)
        {
            SalaryGroup salaryGrp = _db.Get<SalaryGroup>("sp_salary_group_getById", new { salaryGroup.SalaryGroupId });
            if (salaryGrp == null)
                throw new HiringBellException("Salary Group already exist.");
            else
            {
                salaryGrp = salaryGroup;
                if (salaryGrp.GroupComponents == null)
                    salaryGrp.SalaryComponents = "[]";
                else
                    salaryGrp.SalaryComponents = JsonConvert.SerializeObject(salaryGroup.GroupComponents);
                salaryGrp.AdminId = _currentSession.CurrentUserDetail.AdminId;
            }

            var result = _db.Execute<SalaryGroup>("sp_salary_group_insupd", salaryGrp, true);
            if (string.IsNullOrEmpty(result))
                throw new HiringBellException("Fail to insert or update.");
            List<SalaryComponents> value = this.GetSalaryGroupComponents(salaryGroup.SalaryGroupId);
            return value;
        }

        public List<SalaryComponents> GetSalaryGroupComponents(int salaryGroupId)
        {
            SalaryGroup salaryGroup = _db.Get<SalaryGroup>("sp_salary_group_getById", new { SalaryGroupId = salaryGroupId });
            if (salaryGroup == null)
                throw new HiringBellException("Unable to get salary group. Please contact admin");

            salaryGroup.GroupComponents = JsonConvert.DeserializeObject<List<SalaryComponents>>(salaryGroup.SalaryComponents);
            return salaryGroup.GroupComponents;
        }

        public List<SalaryComponents> GetSalaryGroupComponentsByCTC(decimal CTC)
        {
            SalaryGroup salaryGroup = _db.Get<SalaryGroup>("sp_salary_group_get_by_ctc", new { CTC });
            if (salaryGroup == null)
                throw new HiringBellException("Unable to get salary group. Please contact admin");

            salaryGroup.GroupComponents = JsonConvert.DeserializeObject<List<SalaryComponents>>(salaryGroup.SalaryComponents);
            return salaryGroup.GroupComponents;
        }

        private bool CompareFieldsValue(AnnualSalaryBreakup matchedSalaryBreakup, List<CalculatedSalaryBreakupDetail> completeSalaryBreakup)
        {
            bool flag = false;
            int i = 0;
            while (i < matchedSalaryBreakup.SalaryBreakupDetails.Count)
            {
                var item = matchedSalaryBreakup.SalaryBreakupDetails.ElementAt(i);
                var elem = completeSalaryBreakup.Find(x => x.ComponentId == item.ComponentId);
                if (elem == null)
                    break;

                if (item.FinalAmount != elem.FinalAmount)
                {
                    flag = true;
                    break;
                }

                i++;
            }

            return flag;
        }

        private void UpdateIfChangeFound(List<AnnualSalaryBreakup> annualSalaryBreakups, List<CalculatedSalaryBreakupDetail> salaryBreakup, int presentMonth, int PresentYear)
        {
            DateTime present = new DateTime(PresentYear, presentMonth, 1);
            if (_currentSession.TimeZone != null)
                present = _timezoneConverter.ToIstTime(present);

            AnnualSalaryBreakup matchedSalaryBreakups = annualSalaryBreakups.Where(x => x.MonthFirstDate.Subtract(present).TotalDays >= 0).FirstOrDefault<AnnualSalaryBreakup>();
            if (matchedSalaryBreakups == null)
                throw new HiringBellException("Invalid data found in salary detail. Please contact to admin.");
            else
                matchedSalaryBreakups.SalaryBreakupDetails = salaryBreakup;
        }

        private void ValidateCorrectnessOfSalaryDetail(List<CalculatedSalaryBreakupDetail> calculatedSalaryBreakupDetail)
        {
            // implement code to check the correctness of the modal on value level.
        }

        public string SalaryDetailService(long EmployeeId, List<CalculatedSalaryBreakupDetail> calculatedSalaryBreakupDetail, int PresentMonth, int PresentYear)
        {
            if (EmployeeId <= 0)
                throw new HiringBellException("Invalid EmployeeId");

            EmployeeSalaryDetail employeeSalaryDetail = _db.Get<EmployeeSalaryDetail>("sp_employee_salary_detail_get_by_empid", new { EmployeeId = EmployeeId });
            if (employeeSalaryDetail == null)
                throw new HiringBellException("Fail to get salary detail. Please contact to admin.");

            List<AnnualSalaryBreakup> annualSalaryBreakups = JsonConvert.DeserializeObject<List<AnnualSalaryBreakup>>(employeeSalaryDetail.CompleteSalaryDetail);

            // implement code to check the correctness of the modal on value level.
            ValidateCorrectnessOfSalaryDetail(calculatedSalaryBreakupDetail);

            UpdateIfChangeFound(annualSalaryBreakups, calculatedSalaryBreakupDetail, PresentMonth, PresentYear);

            EmployeeSalaryDetail salaryBreakup = new EmployeeSalaryDetail
            {
                CompleteSalaryDetail = JsonConvert.SerializeObject(calculatedSalaryBreakupDetail),
                CTC = employeeSalaryDetail.CTC,
                EmployeeId = EmployeeId,
                GrossIncome = employeeSalaryDetail.GrossIncome,
                GroupId = employeeSalaryDetail.GroupId,
                NetSalary = employeeSalaryDetail.NetSalary,
                TaxDetail = employeeSalaryDetail.TaxDetail
            };
            var result = _db.Execute<EmployeeSalaryDetail>("sp_employee_salary_detail_InsUpd", salaryBreakup, true);
            if (string.IsNullOrEmpty(result))
                throw new HiringBellException("Unable to insert or update salary breakup");
            else
                result = "Inserted/Updated successfully";
            return result;
        }

        private decimal GetEmployeeContributionAmount(List<SalaryComponents> salaryComponents, decimal CTC)
        {
            decimal finalAmount = 0;
            var gratutity = salaryComponents.Find(x => x.ComponentId.ToUpper() == "GRA");
            if (gratutity != null && !string.IsNullOrEmpty(gratutity.Formula))
            {
                if (gratutity.Formula.Contains("[CTC]"))
                    gratutity.Formula = gratutity.Formula.Replace("[CTC]", (Convert.ToDecimal(CTC)).ToString());

                finalAmount += this.calculateExpressionUsingInfixDS(gratutity.Formula, gratutity.DeclaredValue);
            }

            var employeePF = salaryComponents.Find(x => x.ComponentId.ToUpper() == "EPER-PF");
            if (employeePF != null && !string.IsNullOrEmpty(employeePF.Formula))
            {
                if (employeePF.Formula.Contains("[CTC]"))
                    employeePF.Formula = employeePF.Formula.Replace("[CTC]", (Convert.ToDecimal(CTC)).ToString());

                finalAmount += this.calculateExpressionUsingInfixDS(employeePF.Formula, employeePF.DeclaredValue);
            }

            var employeeInsurance = salaryComponents.Find(x => x.ComponentId.ToUpper() == "ECI");
            if (employeeInsurance != null && !string.IsNullOrEmpty(employeeInsurance.Formula))
            {
                if (employeeInsurance.Formula.Contains("[CTC]"))
                    employeeInsurance.Formula = employeeInsurance.Formula.Replace("[CTC]", (Convert.ToDecimal(CTC)).ToString());

                finalAmount += this.calculateExpressionUsingInfixDS(employeeInsurance.Formula, employeeInsurance.DeclaredValue);
            }

            return finalAmount;
        }

        private decimal GetPerquisiteAmount(List<SalaryComponents> salaryComponents)
        {
            decimal finalAmount = 0;
            var prequisiteComponents = salaryComponents.FindAll(x => x.ComponentTypeId == 6);
            if (prequisiteComponents.Count > 0)
            {
                foreach (var item in prequisiteComponents)
                {
                    finalAmount += this.calculateExpressionUsingInfixDS(item.Formula, item.DeclaredValue);
                }
            }
            return finalAmount;
        }

        private decimal GetBaiscAmountValue(List<SalaryComponents> salaryComponents, decimal grossAmount, decimal CTC)
        {
            decimal finalAmount = 0;
            var basicComponent = salaryComponents.Find(x => x.ComponentId.ToUpper() == "BS");
            if (basicComponent != null)
            {
                if (!string.IsNullOrEmpty(basicComponent.Formula))
                {
                    if (basicComponent.Formula.Contains("[CTC]"))
                        basicComponent.Formula = basicComponent.Formula.Replace("[CTC]", (Convert.ToDecimal(CTC)).ToString());
                    else if (basicComponent.Formula.Contains("[GROSS]"))
                        basicComponent.Formula = basicComponent.Formula.Replace("[GROSS]", grossAmount.ToString());
                }

                finalAmount = this.calculateExpressionUsingInfixDS(basicComponent.Formula, basicComponent.DeclaredValue);
            }

            return finalAmount;
        }

        public List<AnnualSalaryBreakup> SalaryBreakupCalcService(long EmployeeId, decimal CTCAnnually)
        {
            if (EmployeeId < 0)
                throw new HiringBellException("Invalid EmployeeId");

            if (CTCAnnually <= 0)
                return this.CreateSalaryBreakUpWithZeroCTC(EmployeeId, CTCAnnually);
            else
                return this.CreateSalaryBreakupWithValue(EmployeeId, CTCAnnually);
        }

        public List<AnnualSalaryBreakup> CreateSalaryBreakupWithValue(long EmployeeId, decimal CTCAnnually)
        {
            List<AnnualSalaryBreakup> annualSalaryBreakups = new List<AnnualSalaryBreakup>();
            DateTime startDate = new DateTime(DateTime.Now.Year, 4, 1);
            List<SalaryComponents> salaryComponents = this.GetSalaryGroupComponentsByCTC(CTCAnnually);

            string stringifySalaryComponents = JsonConvert.SerializeObject(salaryComponents);
            decimal perquisiteAmount = GetPerquisiteAmount(salaryComponents);
            decimal EmployeeContributionAmount = (GetEmployeeContributionAmount(salaryComponents, CTCAnnually)) + perquisiteAmount;
            decimal grossAmount = Convert.ToDecimal(CTCAnnually - EmployeeContributionAmount);
            decimal basicAmountValue = GetBaiscAmountValue(salaryComponents, grossAmount, CTCAnnually);

            int index = 0;
            while (index < 12)
            {
                salaryComponents = JsonConvert.DeserializeObject<List<SalaryComponents>>(stringifySalaryComponents);
                List<CalculatedSalaryBreakupDetail> calculatedSalaryBreakupDetails = new List<CalculatedSalaryBreakupDetail>();

                int i = 0;
                while (i < salaryComponents.Count)
                {
                    var item = salaryComponents.ElementAt(i);
                    if (!string.IsNullOrEmpty(item.Formula))
                    {
                        if (item.Formula.Contains("[BASIC]"))
                            item.Formula = item.Formula.Replace("[BASIC]", basicAmountValue.ToString());
                        else if (item.Formula.Contains("[CTC]"))
                            item.Formula = item.Formula.Replace("[CTC]", (Convert.ToDecimal(CTCAnnually)).ToString());
                        else if (item.Formula.Contains("[GROSS]"))
                            item.Formula = item.Formula.Replace("[GROSS]", grossAmount.ToString());
                    }

                    i++;
                }

                decimal amount = 0;
                CalculatedSalaryBreakupDetail calculatedSalaryBreakupDetail = null;
                foreach (var item in salaryComponents)
                {
                    if (!string.IsNullOrEmpty(item.ComponentId))
                    {
                        calculatedSalaryBreakupDetail = new CalculatedSalaryBreakupDetail();

                        amount = this.calculateExpressionUsingInfixDS(item.Formula, item.DeclaredValue);

                        calculatedSalaryBreakupDetail.ComponentId = item.ComponentId;
                        calculatedSalaryBreakupDetail.Formula = item.Formula;
                        calculatedSalaryBreakupDetail.ComponentName = item.ComponentFullName;
                        calculatedSalaryBreakupDetail.ComponentTypeId = item.ComponentTypeId;
                        calculatedSalaryBreakupDetail.FinalAmount = amount / 12;

                        calculatedSalaryBreakupDetails.Add(calculatedSalaryBreakupDetail);
                    }
                }

                var value = calculatedSalaryBreakupDetails.Where(x => x.ComponentTypeId == 2).Sum(x => x.FinalAmount);

                calculatedSalaryBreakupDetail = new CalculatedSalaryBreakupDetail
                {
                    ComponentId = nameof(ComponentNames.Special),
                    Formula = null,
                    ComponentName = ComponentNames.Special,
                    FinalAmount = (grossAmount / 12 - calculatedSalaryBreakupDetails.Where(x => x.ComponentTypeId == 2).Sum(x => x.FinalAmount)),
                    ComponentTypeId = 102
                };

                calculatedSalaryBreakupDetails.Add(calculatedSalaryBreakupDetail);

                calculatedSalaryBreakupDetail = new CalculatedSalaryBreakupDetail
                {
                    ComponentId = nameof(ComponentNames.Gross),
                    Formula = null,
                    ComponentName = ComponentNames.Gross,
                    FinalAmount = (CTCAnnually - EmployeeContributionAmount) / 12,
                    ComponentTypeId = 100
                };

                calculatedSalaryBreakupDetails.Add(calculatedSalaryBreakupDetail);

                calculatedSalaryBreakupDetail = new CalculatedSalaryBreakupDetail
                {
                    ComponentId = nameof(ComponentNames.CTC),
                    Formula = null,
                    ComponentName = ComponentNames.CTC,
                    FinalAmount = CTCAnnually / 12,
                    ComponentTypeId = 101
                };

                calculatedSalaryBreakupDetails.Add(calculatedSalaryBreakupDetail);

                annualSalaryBreakups.Add(new AnnualSalaryBreakup
                {
                    MonthName = startDate.ToString("MMM"),
                    MonthNumber = startDate.Month,
                    MonthFirstDate = startDate,
                    SalaryBreakupDetails = calculatedSalaryBreakupDetails
                });

                startDate = startDate.AddMonths(1);
                index++;
            }

            return annualSalaryBreakups;
        }

        private List<AnnualSalaryBreakup> CreateSalaryBreakUpWithZeroCTC(long EmployeeId, decimal CTCAnnually)
        {
            List<AnnualSalaryBreakup> annualSalaryBreakups = new List<AnnualSalaryBreakup>();


            List<CalculatedSalaryBreakupDetail> calculatedSalaryBreakupDetails = new List<CalculatedSalaryBreakupDetail>();
            List<SalaryComponents> salaryComponents = this.GetSalaryGroupComponentsByCTC(CTCAnnually);

            CalculatedSalaryBreakupDetail calculatedSalaryBreakupDetail = null;
            foreach (var item in salaryComponents)
            {
                if (!string.IsNullOrEmpty(item.ComponentId))
                {
                    calculatedSalaryBreakupDetail = new CalculatedSalaryBreakupDetail();

                    calculatedSalaryBreakupDetail.ComponentId = item.ComponentId;
                    calculatedSalaryBreakupDetail.Formula = item.Formula;
                    calculatedSalaryBreakupDetail.ComponentName = item.ComponentFullName;
                    calculatedSalaryBreakupDetail.ComponentTypeId = item.ComponentTypeId;
                    calculatedSalaryBreakupDetail.FinalAmount = 0;

                    calculatedSalaryBreakupDetails.Add(calculatedSalaryBreakupDetail);
                }
            }

            calculatedSalaryBreakupDetail = new CalculatedSalaryBreakupDetail
            {
                ComponentId = nameof(ComponentNames.Special),
                Formula = null,
                ComponentName = ComponentNames.Special,
                FinalAmount = 0,
                ComponentTypeId = 102
            };

            calculatedSalaryBreakupDetails.Add(calculatedSalaryBreakupDetail);

            calculatedSalaryBreakupDetail = new CalculatedSalaryBreakupDetail
            {
                ComponentId = nameof(ComponentNames.Gross),
                Formula = null,
                ComponentName = ComponentNames.Gross,
                FinalAmount = 0,
                ComponentTypeId = 100
            };

            calculatedSalaryBreakupDetails.Add(calculatedSalaryBreakupDetail);

            calculatedSalaryBreakupDetail = new CalculatedSalaryBreakupDetail
            {
                ComponentId = nameof(ComponentNames.CTC),
                Formula = null,
                ComponentName = ComponentNames.CTC,
                FinalAmount = 0,
                ComponentTypeId = 101
            };

            calculatedSalaryBreakupDetails.Add(calculatedSalaryBreakupDetail);

            return annualSalaryBreakups;
        }

        public EmployeeSalaryDetail GetSalaryBreakupByEmpIdService(long EmployeeId)
        {
            EmployeeSalaryDetail completeSalaryBreakup = _db.Get<EmployeeSalaryDetail>("sp_employee_salary_detail_get_by_empid", new { EmployeeId });
            return completeSalaryBreakup;
        }

        public SalaryGroup GetSalaryGroupByCTC(decimal CTC)
        {
            SalaryGroup salaryGroup = _db.Get<SalaryGroup>("sp_salary_group_get_by_ctc", new { CTC });
            if (salaryGroup == null)
                throw new HiringBellException("Unable to get salary group. Please contact admin");
            return salaryGroup;
        }

        private decimal calculateExpressionUsingInfixDS(string expression, decimal declaredAmount)
        {
            if (string.IsNullOrEmpty(expression))
                return declaredAmount;

            if (!expression.Contains("()"))
                expression = string.Format("({0})", expression);

            List<string> operatorStact = new List<string>();
            var expressionStact = new List<object>();
            int index = 0;
            var lastOp = "";
            var ch = "";

            while (index < expression.Length)
            {
                ch = expression[index].ToString();
                if (ch.Trim() == "")
                {
                    index++;
                    continue;
                }
                int number;
                if (!int.TryParse(ch.ToString(), out number))
                {
                    switch (ch)
                    {
                        case "+":
                        case "-":
                        case "/":
                        case "%":
                        case "*":
                            if (operatorStact.Count > 0)
                            {
                                lastOp = operatorStact[operatorStact.Count - 1];
                                if (lastOp == "+" || lastOp == "-" || lastOp == "/" || lastOp == "*" || lastOp == "%")
                                {
                                    lastOp = operatorStact[operatorStact.Count - 1];
                                    operatorStact.RemoveAt(operatorStact.Count - 1);
                                    expressionStact.Add(lastOp);
                                }
                            }
                            operatorStact.Add(ch);
                            break;
                        case ")":
                            while (true)
                            {
                                lastOp = operatorStact[operatorStact.Count - 1];
                                operatorStact.RemoveAt(operatorStact.Count - 1);
                                if (lastOp == "(")
                                {
                                    break;
                                }
                                expressionStact.Add(lastOp);
                            }
                            break;
                        case "(":
                            operatorStact.Add(ch);
                            break;
                    }
                }
                else
                {
                    decimal value = 0;
                    decimal fraction = 0;
                    bool isFractionFound = false;
                    while (true)
                    {
                        ch = expression[index].ToString();
                        if (ch == ".")
                        {
                            index++;
                            isFractionFound = true;
                            break;
                        }

                        if (ch.Trim() == "")
                        {
                            expressionStact.Add($"{value}.{fraction}");
                            break;
                        }

                        if (int.TryParse(ch.ToString(), out number))
                        {
                            if (!isFractionFound)
                                value = Convert.ToDecimal(value + ch);
                            else
                                fraction = Convert.ToDecimal(fraction + ch);
                            index++;
                        }
                        else
                        {
                            index--;
                            expressionStact.Add($"{value}.{fraction}");
                            break;
                        }
                    }
                }

                index++;
            }

            var exp = expressionStact.Aggregate((x, y) => x.ToString() + " " + y.ToString()).ToString();
            return _postfixToInfixConversion.evaluatePostfix(exp);
        }

    }
}
