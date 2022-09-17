﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ModalLayer.Modal;
using Newtonsoft.Json;
using OnlineDataBuilder.ContextHandler;
using ServiceLayer.Code;
using ServiceLayer.Interface;
using System.Collections.Generic;
using System.Net;

namespace OnlineDataBuilder.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class TimesheetController : BaseController
    {
        private readonly ITimesheetService _timesheetService;
        private readonly HttpContext _httpContext;
        public TimesheetController(ITimesheetService timesheetService, IHttpContextAccessor httpContext)
        {
            _timesheetService = timesheetService;
            _httpContext = httpContext.HttpContext;
        }

        [HttpPost("GetTimesheetByUserId")]
        public IResponse<ApiResponse> GetTimesheetByUserId(TimesheetDetail attendenceDetail)
        {
            var result = _timesheetService.GetTimesheetByUserIdService(attendenceDetail);
            return BuildResponse(result, HttpStatusCode.OK);
        }

        [HttpPost("InsertUpdateTimesheet")]
        public IResponse<ApiResponse> InsertUpdateTimesheet(List<DailyTimesheetDetail> dailyTimesheetDetails)
        {
            var result = _timesheetService.InsertUpdateTimesheet(dailyTimesheetDetails);
            return BuildResponse(result, HttpStatusCode.OK);
        }


        [HttpGet("GetPendingTimesheetById/{EmployeeId}/{clientId}")]
        public IResponse<ApiResponse> GetPendingTimesheetById(long employeeId, long clientId)
        {
            var result = _timesheetService.GetPendingTimesheetByIdService(employeeId, clientId);
            return BuildResponse(result, HttpStatusCode.OK);
        }

        [HttpPost("GetEmployeeTimeSheet")]
        public IResponse<ApiResponse> GetEmployeeTimeSheet(TimesheetDetail timesheetDetail)
        {
            var result = _timesheetService.GetEmployeeTimeSheetService(timesheetDetail);
            return BuildResponse(result, HttpStatusCode.OK);
        }

        [HttpPost("UpdateTimesheet")]
        public IResponse<ApiResponse> UpdateTimesheet()
        {
            _httpContext.Request.Form.TryGetValue("comment", out StringValues comment);
            _httpContext.Request.Form.TryGetValue("timesheet", out StringValues timesheet);
            string Comment = JsonConvert.DeserializeObject<string>(comment);
            List<DailyTimesheetDetail> dailyTimesheetDetails = JsonConvert.DeserializeObject<List<DailyTimesheetDetail>>(timesheet);
            var result = _timesheetService.UpdateTimesheetService(dailyTimesheetDetails, Comment);
            return BuildResponse(result, HttpStatusCode.OK);
        }
    }
}
