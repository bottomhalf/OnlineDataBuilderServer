﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ModalLayer.Modal;
using Newtonsoft.Json;
using OnlineDataBuilder.ContextHandler;
using ServiceLayer.Interface;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OnlineDataBuilder.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly HttpContext _httpContext;

        public EmployeeController(IEmployeeService employeeService, IHttpContextAccessor httpContext)
        {
            _employeeService = employeeService;
            _httpContext = httpContext.HttpContext;
        }

        [HttpPost]
        [Route("GetEmployees")]
        public ApiResponse GetEmployees([FromBody] FilterModel filterModel)
        {
            var Result = _employeeService.GetEmployees(filterModel);
            return BuildResponse(Result, HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("EmployeesListData")]
        public ApiResponse EmployeesListData([FromRoute] FilterModel filterModel)
        {
            var Result = _employeeService.EmployeesListDataService(filterModel);
            return BuildResponse(Result, HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("GetManageEmployeeDetail/{EmployeeId}")]
        public ApiResponse GetManageEmployeeDetail(long EmployeeId)
        {
            var Result = _employeeService.GetEmployeeLeaveDetailService(EmployeeId);
            return BuildResponse(Result, HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("LoadMappedClients/{EmployeeId}")]
        public ApiResponse LoadMappedClients(long EmployeeId)
        {
            var Result = _employeeService.LoadMappedClientService(EmployeeId);
            return BuildResponse(Result, HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("GetAllManageEmployeeDetail/{EmployeeId}")]
        public ApiResponse GetAllManageEmployeeDetail(long EmployeeId)
        {
            var Result = _employeeService.GetManageEmployeeDetailService(EmployeeId);
            return BuildResponse(Result, HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("GetManageClient/{EmployeeId}")]
        public ApiResponse GetManageClient(long EmployeeId)
        {
            var Result = _employeeService.GetManageClientService(EmployeeId);
            return BuildResponse(Result, HttpStatusCode.OK);
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        [Route("UpdateEmployeeMappedClientDetail/{IsUpdating}")]
        public ApiResponse UpdateEmployeeMappedClientDetail([FromBody] Employee employee, bool IsUpdating)
        {
            var Result = _employeeService.UpdateEmployeeMappedClientDetailService(employee, IsUpdating);
            return BuildResponse(Result, HttpStatusCode.OK);
        }

        [HttpGet("GetEmployeeById/{EmployeeId}/{IsActive}")]
        public ApiResponse GetEmployeeById(int EmployeeId, int IsActive)
        {
            var Result = _employeeService.GetEmployeeByIdService(EmployeeId, IsActive);
            return BuildResponse(Result, HttpStatusCode.OK);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpDelete("ActivateOrDeActiveEmployee/{EmployeeId}/{IsActive}")]
        public ApiResponse DeleteEmployeeById(int EmployeeId, bool IsActive)
        {
            var Result = _employeeService.ActivateOrDeActiveEmployeeService(EmployeeId, IsActive);
            return BuildResponse(Result, HttpStatusCode.OK);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("employeeregistration")]
        public async Task<ApiResponse> EmployeeRegistration()
        {
            try
            {
                StringValues UserInfoData = default(string);
                _httpContext.Request.Form.TryGetValue("employeeDetail", out UserInfoData);
                if (UserInfoData.Count > 0)
                {
                    Employee employee = JsonConvert.DeserializeObject<Employee>(UserInfoData);
                    IFormFileCollection files = _httpContext.Request.Form.Files;
                    var resetSet = await _employeeService.RegisterEmployeeService(employee, files);
                    return BuildResponse(resetSet);
                }
                else
                {
                    return BuildResponse(this.responseMessage, HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("updateemployeedetail")]
        public async Task<ApiResponse> UpdateEmployeeDetail()
        {
            try
            {
                StringValues UserInfoData = default(string);
                _httpContext.Request.Form.TryGetValue("employeeDetail", out UserInfoData);
                if (UserInfoData.Count > 0)
                {
                    Employee employee = JsonConvert.DeserializeObject<Employee>(UserInfoData);
                    IFormFileCollection files = _httpContext.Request.Form.Files;
                    var resetSet = await _employeeService.UpdateEmployeeService(employee, files);
                    return BuildResponse(resetSet);
                }
                else
                {
                    return BuildResponse(this.responseMessage, HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("GenerateOfferLetter")]
        public async Task<ApiResponse> GenerateOfferLetter(EmployeeOfferLetter employeeOfferLetter)
        {
            var result = _employeeService.GenerateOfferLetterService(employeeOfferLetter);
            return BuildResponse(result);
        }
    }
}
