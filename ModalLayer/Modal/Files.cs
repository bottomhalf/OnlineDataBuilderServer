﻿using System;

namespace ModalLayer.Modal
{
    public enum UserType
    {
        Employee = 1,
        Client = 2,
        Candidate = 3,
        Other = 4,
        Admin = 9
    }

    public enum DeclarationType
    {
        PPF = 1,
        SeniorCitizenSavingScheme = 2,
        HousingLoan = 3,
        MutualFund = 4,
        NationalSavingCerificate = 5,
        UnitLinkInsurancePlan = 6,
        LifeInsurancePolicy = 7,
        EducationTuitionFees = 8,
        ScheduleBankFD = 9,
        PostOfficeTimeDeposit = 10,
        DeferredAnnuity = 11,
        SuperAnnuity = 12,
        NABARDnotifiesbond = 13,
        SukanyaSamriddhiYojna = 14,
        Other = 15,
        MutualFundPension = 16,
        NPSEmployeeContribution = 17
    }

    public class DocumentFile
    {
        public long FileUid { set; get; }
        public long DocumentId { set; get; }
        public string ProfileUid { set; get; }
        public UserType UserTypeId { set; get; }
    }

    public class Files : DocumentFile
    {
        public string FilePath { set; get; }
        public string FileName { set; get; }
        public string FileExtension { set; get; }
        public string Status { set; get; }
        public DateTime? PaidOn { set; get; }
        public long BillTypeId { set; get; }
        public long UserId { set; get; }
        public string Mobile { set; get; }
        public string Email { set; get; }
        public string FileType { set; get; }
        public string FileSize { set; get; }
        public string ParentFolder { set; get; } = null;
        public FileSystemType SystemFileType { set; get; }
    }
}
