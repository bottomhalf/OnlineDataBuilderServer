﻿using BottomhalfCore.DatabaseLayer.Common.Code;
using BottomhalfCore.Services.Code;
using Microsoft.AspNetCore.Http;
using ModalLayer.Modal;
using ServiceLayer.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ServiceLayer.Code
{
    public class ClientsService : IClientsService
    {
        private readonly IDb _db;
        private readonly CommonFilterService _commonFilterService;
        private readonly CurrentSession _currentSession;
        private readonly IFileService _fileService;
        private readonly FileLocationDetail _fileLocationDetail;
        public ClientsService(IDb db, CommonFilterService commonFilterService, CurrentSession currentSession, IFileService fileService, FileLocationDetail fileLocationDetail)
        {
            _db = db;
            _commonFilterService = commonFilterService;
            _currentSession = currentSession;
            _fileService = fileService;
            _fileLocationDetail = fileLocationDetail;
        }
        public List<Organization> GetClients(FilterModel filterModel)
        {
            List<Organization> client = _commonFilterService.GetResult<Organization>(filterModel, "SP_Clients_Get");
            return client;
        }

        public DataSet GetClientDetailById(long ClientId, bool IsActive, int UserTypeId)
        {
            if (ClientId <= 0)
                throw new HiringBellException { UserMessage = "Invalid ClientId", FieldName = nameof(ClientId), FieldValue = ClientId.ToString() };

            //Organization client = default;
            DbParam[] param = new DbParam[]
            {
                new DbParam(ClientId, typeof(long), "_ClientId"),
                new DbParam(IsActive, typeof(bool), "_IsActive"),
                new DbParam(UserTypeId, typeof(int), "_UserTypeId")
            };

            var resultSet = _db.GetDataset("SP_Client_ById", param);
            if (resultSet.Tables.Count == 2)
            {
                resultSet.Tables[0].TableName = "client";
                resultSet.Tables[1].TableName = "file";
            }
            //if (resultSet.Tables.Count > 0 && resultSet.Tables[0].Rows.Count > 0)
            //{
            //    var emps = Converter.ToList<Organization>(resultSet.Tables[0]);
            //    if (emps != null && emps.Count > 0)
            //        client = emps[0];
            //}
            return resultSet;

            //return client;
        }

        public async Task<Organization> RegisterClient(Organization client, IFormFileCollection fileCollection, bool isUpdating)
        {
            try
            {
                if (isUpdating == true)
                {
                    if (client.ClientId <= 0)
                        throw new HiringBellException { UserMessage = "Invalid ClientId", FieldName = nameof(client.ClientId), FieldValue = client.ClientId.ToString() };
                }

                ClientValidation(client);
                Organization organization = null;
                DbParam[] param = new DbParam[]
                {
                new DbParam(client.ClientId, typeof(long), "_ClientId"),
                new DbParam(client.ClientName, typeof(string), "_ClientName"),
                new DbParam(client.PrimaryPhoneNo, typeof(string), "_PrimaryPhoneNo"),
                new DbParam(client.SecondaryPhoneNo, typeof(string), "_SecondaryPhoneNo"),
                new DbParam(client.MobileNo, typeof(string), "_MobileNo"),
                new DbParam(client.Email, typeof(string), "_Email"),
                new DbParam(client.OtherEmail_1, typeof(string), "_OtherEmail_1"),
                new DbParam(client.OtherEmail_2, typeof(string), "_OtherEmail_2"),
                new DbParam(client.OtherEmail_3, typeof(string), "_OtherEmail_3"),
                new DbParam(client.OtherEmail_4, typeof(string), "_OtherEmail_4"),
                new DbParam(client.Fax, typeof(string), "_Fax"),
                new DbParam(client.GSTNO, typeof(string), "_GSTNO"),
                new DbParam(client.PanNo, typeof(string), "_PanNo"),
                new DbParam(client.Pincode, typeof(int), "_Pincode"),
                new DbParam(client.Country, typeof(string), "_Country"),
                new DbParam(client.State, typeof(string), "_State"),
                new DbParam(client.City, typeof(string), "_City"),
                new DbParam(client.FirstAddress, typeof(string), "_FirstAddress"),
                new DbParam(client.SecondAddress, typeof(string), "_SecondAddress"),
                new DbParam(client.ThirdAddress, typeof(string), "_ThirdAddress"),
                new DbParam(client.ForthAddress, typeof(string), "_ForthAddress"),
                new DbParam(client.IFSC, typeof(string), "_IFSC"),
                new DbParam(client.AccountNo, typeof(string), "_AccountNo"),
                new DbParam(client.BankName, typeof(string), "_BankName"),
                new DbParam(client.BranchName, typeof(string), "_BranchName"),
                new DbParam(_currentSession.CurrentUserDetail.UserId, typeof(long), "_AdminId")
                };

                var status = string.Empty;
                var resultSet = _db.GetDataset("SP_Client_IntUpd", param, true, ref status);
                if (resultSet != null && resultSet.Tables.Count > 0)
                    organization = Converter.ToType<Organization>(resultSet.Tables[0]);

                if (organization.ClientId <= 0)
                    throw new Exception("fail to get client id");

                if (fileCollection.Count > 0)
                {
                    var files = fileCollection.Select(x => new Files
                    {
                        FileUid = client.FileId,
                        FileName = fileCollection[0].Name,
                        Email = client.Email,
                        FileExtension = string.Empty
                    }).ToList<Files>();
                    _fileService.SaveFile(_fileLocationDetail.UserFolder, files, fileCollection, (organization.ClientId).ToString(), client.OldFileName);

                    var fileInfo = (from n in files
                                    select new
                                    {
                                        FileId = n.FileUid,
                                        FileOwnerId = organization.ClientId,
                                        FileName = n.FileName,
                                        FilePath = n.FilePath,
                                        FileExtension = n.FileExtension,
                                        UserTypeId = (int)UserType.Client,
                                        AdminId = _currentSession.CurrentUserDetail.UserId
                                    });

                    DataTable table = Converter.ToDataTable(fileInfo);
                    _db.StartTransaction(IsolationLevel.ReadUncommitted);
                    var taskResult = await _db.BatchInsertUpdateAsync("sp_userfiledetail_Upload", table, true);
                    _db.Commit();
                }

                return organization;
            }
            catch (Exception)
            {
                _db.RollBack();
                throw;
            }
            
        }

        private void ClientValidation(Organization organization)
        {
            if (string.IsNullOrEmpty(organization.Email))
                throw new HiringBellException { UserMessage = "Email id is a mandatory field.", FieldName = nameof(organization.Email), FieldValue = organization.Email.ToString() };

            if (string.IsNullOrEmpty(organization.ClientName))
                throw new HiringBellException { UserMessage = "First Name is a mandatory field.", FieldName = nameof(organization.ClientName), FieldValue = organization.ClientName.ToString() };

            if (string.IsNullOrEmpty(organization.PrimaryPhoneNo) || organization.PrimaryPhoneNo.Contains("."))
                throw new HiringBellException { UserMessage = "Mobile number is a mandatory field.", FieldName = nameof(organization.PrimaryPhoneNo), FieldValue = organization.PrimaryPhoneNo.ToString() };

            var mail = new MailAddress(organization.Email);
            bool isValidEmail = mail.Host.Contains(".");
            if (!isValidEmail)
                throw new HiringBellException { UserMessage = "The email is invalid.", FieldName = nameof(organization.Email), FieldValue = organization.Email.ToString() };
        }

        public DataSet DeactivateClient(Employee employee)
        {
            if (employee == null || employee.EmployeeUid <= 0)
            {
                throw new HiringBellException("Invalid client detail submitted.");
            }

            DbParam[] param = new DbParam[]
            {
                new DbParam(employee.EmployeeMappedClientsUid, typeof(long), "_ClientMappedId"),
                new DbParam(employee.EmployeeUid, typeof(long), "_UserId")
            };

            var resultSet = _db.GetDataset("sp_deactivateOrganization_delandgetall", param);
            return resultSet;
        }
    }
}
