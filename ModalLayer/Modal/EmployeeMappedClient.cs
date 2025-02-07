﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ModalLayer.Modal
{
    public class EmployeeMappedClient
    {
        public long EmployeeMappedClientUid { get; set; }
        public long EmployeeUid { get; set; }
        public int ClientUid { get; set; }
        public string ClientName { get; set; }
        public decimal FinalPackage { get; set; }
        public decimal ActualPackage { get; set; }
        public decimal TakeHomeByCandidate { get; set; }
        public bool IsPermanent { get; set; }
        public bool IsActive { get; set; }
        public int BillingHours { get; set; }
        public int DaysPerWeek { get; set; }
        public DateTime DateOfJoining { get; set; }
        public DateTime DateOfLeaving { get; set; }

    }
}
