﻿using BottomhalfCore.DatabaseLayer.Common.Code;
using BottomhalfCore.Services.Code;
using DocMaker.PdfService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ModalLayer.Modal;
using ModalLayer.Modal.Profile;
using Newtonsoft.Json;
using ServiceLayer.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer.Code
{
    public class UserService : IUserService
    {
        private readonly IDb _db;
        private readonly IFileService _fileService;
        private readonly FileLocationDetail _fileLocationDetail;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IFileMaker _fileMaker;

        public UserService(IDb db, IFileService fileService, FileLocationDetail fileLocationDetail, IHostingEnvironment hostingEnvironment, IFileMaker fileMaker)
        {
            _db = db;
            _fileService = fileService;
            _fileLocationDetail = fileLocationDetail;
            _fileMaker = fileMaker;
            _hostingEnvironment = hostingEnvironment;
        }

        public string UpdateProfile(ProfessionalUser professionalUser, int IsProfileImageRequest = 0)
        {
            var result = string.Empty;
            var professionalUserDetail = JsonConvert.SerializeObject(professionalUser);
            DbParam[] dbParams = new DbParam[]
            {
                new DbParam(professionalUser.UserId, typeof(long), "_UserId"),
                new DbParam(professionalUser.FileId, typeof(long), "_FileId"),
                new DbParam(professionalUser.Mobile_Number, typeof(string), "_Mobile"),
                new DbParam(professionalUser.Email, typeof(string), "_Email"),
                new DbParam(professionalUser.FirstName, typeof(string), "_FirstName"),
                new DbParam(professionalUser.LastName, typeof(string), "_LastName"),
                new DbParam(professionalUser.Date_Of_Application, typeof(DateTime), "_Date_Of_Application"),
                new DbParam(professionalUser.Total_Experience_In_Months, typeof(int), "_Total_Experience_In_Months"),
                new DbParam(professionalUser.Salary_Package, typeof(double), "_Salary_Package"),
                new DbParam(professionalUser.Notice_Period, typeof(int), "_Notice_Period"),
                new DbParam(professionalUser.Expeceted_CTC, typeof(double), "_Expeceted_CTC"),
                new DbParam(professionalUser.Current_Location, typeof(string), "_Current_Location"),
                new DbParam(JsonConvert.SerializeObject(professionalUser.Preferred_Locations), typeof(string), "_Preferred_Location"),
                new DbParam(professionalUserDetail, typeof(string), "_ProfessionalDetail_Json"),
                new DbParam(IsProfileImageRequest, typeof(int), "_IsProfileImageRequest")
            };

            result = _db.ExecuteNonQuery("sp_professionaldetail_insetupdate", dbParams, true);
            return result;
        }

        public string UploadUserInfo(string userId, ProfessionalUser professionalUser, IFormFileCollection FileCollection)
        {
            if (string.IsNullOrEmpty(professionalUser.Email))
            {
                throw new HiringBellException("Email id is required field.");
            }

            int IsProfileImageRequest = 0;
            List<Files> files = new List<Files>();
            Files file = new Files();
            if (FileCollection.Count > 0)
            {
                IsProfileImageRequest = 1;
                Parallel.ForEach(FileCollection, x =>
                {
                    files.Add(new Files
                    {
                        FileName = x.Name,
                        FilePath = string.Empty,
                        Email = professionalUser.Email
                    });
                });

                _fileService.SaveFile(_fileLocationDetail.UserFolder, files, FileCollection, userId);
                file = files.Single();
            }

            var result = this.UpdateProfile(professionalUser, IsProfileImageRequest);
            if (string.IsNullOrEmpty(result))
            {
                _fileService.DeleteFiles(files);
                throw new HiringBellException("Fail to update user info.");
            }
            return result;
        }

        public ProfileDetail GetUserDetail(long userId)
        {
            if (userId <= 0)
                throw new HiringBellException { UserMessage = "Invalid User Id", FieldName = nameof(userId), FieldValue = userId.ToString() };

            ProfileDetail profileDetail = new ProfileDetail();
            DbParam[] param = new DbParam[]
            {
               new DbParam(userId, typeof(long), "_UserId"),
               new DbParam(null, typeof(string), "_Mobile"),
               new DbParam(null, typeof(string), "_Email")
            };

            var Result = _db.GetDataset("sp_professionaldetail_filter", param);

            if (Result.Tables.Count == 0)
            {
                throw new HiringBellException("Fail to get record.");
            }
            else
            {
                profileDetail.profileDetail = Converter.ToType<FileDetail>(Result.Tables[1]);
                string jsonData = Convert.ToString(Result.Tables[0].Rows[0][0]);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    profileDetail.professionalUser = JsonConvert.DeserializeObject<ProfessionalUser>(jsonData);
                }
                else
                {
                    throw new HiringBellException("Fail to get record.");
                }
            }
            return profileDetail;
        }

        public string GenerateResume(long userId)
        {
            if (userId <= 0)
                throw new HiringBellException { UserMessage = "Invalid User Id", FieldName = nameof(userId), FieldValue = userId.ToString() };

            var value = string.Empty;
            ProfileDetail profileDetail = new ProfileDetail();
            DbParam[] param = new DbParam[]
            {
               new DbParam(userId, typeof(long), "_UserId"),
               new DbParam(null, typeof(string), "_Mobile"),
               new DbParam(null, typeof(string), "_Email")
            };

            var Result = _db.GetDataset("sp_professionaldetail_filter", param);

            if (Result.Tables.Count == 0)
            {
                throw new HiringBellException("Fail to get record.");
            }
            else
            {
                profileDetail.profileDetail = Converter.ToType<FileDetail>(Result.Tables[1]);
                string jsonData = Convert.ToString(Result.Tables[0].Rows[0][0]);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    profileDetail.professionalUser = JsonConvert.DeserializeObject<ProfessionalUser>(jsonData);
                }
                else
                {
                    throw new HiringBellException("Fail to get record.");
                }

                string rootPath = _hostingEnvironment.ContentRootPath;
                string templatePath = Path.Combine(rootPath,
                    _fileLocationDetail.Location,
                    Path.Combine(_fileLocationDetail.resumePath.ToArray()),
                    _fileLocationDetail.resumeTemplate
                );
            }
                
            return value;
        }
    }
}
