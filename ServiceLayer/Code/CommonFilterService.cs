﻿using BottomhalfCore.DatabaseLayer.Common.Code;
using BottomhalfCore.Services.Code;
using ModalLayer.Modal;
using System;
using System.Collections.Generic;

namespace ServiceLayer.Code
{
    public class CommonFilterService
    {
        private readonly IDb _db;
        //private readonly CurrentSession _currentSession;
        public CommonFilterService(IDb db)
        {
            _db = db;
            // _currentSession = currentSession;
        }

        public List<T> GetResult<T>(FilterModel filterModel, string ProcedureName)
            where T : new()
        {
            return GetList<T>(filterModel, ProcedureName);
        }

        public List<T> GetMultiResult<T, I>(FilterModel filterModel, string ProcedureName)
            where T : new()
        {
            return GetList<T>(filterModel, ProcedureName);
        }

        private List<T> GetList<T>(FilterModel filterModel, string ProcedureName)
            where T : new()
        {
            DbParam[] dbParams = new DbParam[]
            {
                new DbParam(filterModel.SearchString, typeof(string), "_searchString"),
                new DbParam(filterModel.SortBy, typeof(string), "_sortBy"),
                new DbParam(filterModel.PageIndex, typeof(int), "_pageIndex"),
                new DbParam(filterModel.PageSize, typeof(int), "_pageSize")
            };

            List<T> filterResult = default;
            var Result = _db.GetDataset(ProcedureName, dbParams);
            if (Result.Tables.Count > 0 && Result.Tables[0].Rows.Count > 0)
            {
                filterResult = Converter.ToList<T>(Result.Tables[0]);
            }
            return filterResult;
        }
    }
}
