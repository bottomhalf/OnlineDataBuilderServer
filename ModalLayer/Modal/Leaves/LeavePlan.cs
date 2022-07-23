﻿using System;

namespace ModalLayer.Modal.Leaves
{
    public class LeavePlan
    {
        public int LeavePlanId { set; get; }
        public string PlanName { set; get; }
        public string PlanDescription { set; get; }
        public DateTime PlanStartCalendarDate { set; get; }
        public bool IsShowLeavePolicy { set; get; }
        public bool IsUploadedCustomLeavePolicy { set; get; }
    }
}
