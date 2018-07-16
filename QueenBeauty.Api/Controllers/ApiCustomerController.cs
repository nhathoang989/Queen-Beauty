// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.OData.Query;
using Newtonsoft.Json.Linq;
using QueenBeauty.Lib.ViewModels;
using Swastika.Cms.Lib;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.ViewModels.Api;
using Swastika.Cms.Lib.ViewModels.BackEnd;
using Swastika.Cms.Lib.ViewModels.FrontEnd;
using Swastika.Cms.Lib.ViewModels.Info;
using Swastika.Domain.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Swastika.Common.Utility.Enums;

namespace QueenBeauty.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/queen-beauty/Customer")]
    public class ApiCustomerController :
        BaseApiController
    {
        public ApiCustomerController()
        {
        }
        #region Get

        // GET api/Customer/id
        [HttpGet]
        [Route("delete/{id}")]
        public async Task<RepositoryResponse<SiocCustomer>> DeleteAsync(string id)
        {
            var getPage = await QueenApiCustomerViewModel.Repository.GetSingleModelAsync(
                model => model.Id == id );
            if (getPage.IsSucceed)
            {

                return await getPage.Data.RemoveModelAsync(true);
            }
            else
            {
                return new RepositoryResponse<SiocCustomer>()
                {
                    IsSucceed = false
                };
            }
        }

        // GET api/pages/id
        [HttpGet]
        [Route("details/{id}")]
        [Route("details")]
        public async Task<JObject> Details(string id = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var beResult = await QueenApiCustomerViewModel.Repository.GetSingleModelAsync(model => model.Id == id ).ConfigureAwait(false);
                return JObject.FromObject(beResult);
            }
            else
            {
                var model = new SiocCustomer();
                RepositoryResponse<QueenApiCustomerViewModel> result = new RepositoryResponse<QueenApiCustomerViewModel>()
                {
                    IsSucceed = true,
                    Data = new QueenApiCustomerViewModel(model)
                    {
                        Specificulture = _lang,
                        Status = SWStatus.Preview,
                    }
                };
                return JObject.FromObject(result);
            }
        }

        #endregion Get

        #region Post

        // POST api/Customer
        //[Authorize]
        [HttpPost, HttpOptions]
        [Route("save")]
        public async Task<JObject> Save([FromBody]QueenApiCustomerViewModel model)
        {
            if (model != null)
            {
                model.Specificulture = _lang;
                var result = await model.SaveModelAsync(true).ConfigureAwait(false);
                if (result.IsSucceed)
                {
                    var getCustomers = await QueenApiCustomerViewModel.Repository.GetModelListAsync();
                    return JObject.FromObject(getCustomers);
                }
                return JObject.FromObject(result);
            }
            var temp = new RepositoryResponse<QueenApiCustomerViewModel>();
            return JObject.FromObject(temp);
        }

        // POST api/Customer
        [HttpPost, HttpOptions]
        [Route("save/{id}")]
        public async Task<RepositoryResponse<SiocCustomer>> SaveFields(string id, [FromBody]List<EntityField> fields)
        {
            if (fields != null)
            {
                var result = new RepositoryResponse<SiocCustomer>() { IsSucceed = true };
                foreach (var property in fields)
                {
                    if (result.IsSucceed)
                    {
                        result = await QueenApiCustomerViewModel.Repository.UpdateFieldsAsync(c => c.Id == id, fields).ConfigureAwait(false);
                    }
                    else
                    {
                        break;
                    }

                }
                return result;
            }
            return new RepositoryResponse<SiocCustomer>();
        }

        // GET api/Customer
        [HttpPost, HttpOptions]
        [Route("list")]
        public async Task<JObject> GetList(
            [FromBody] RequestPaging request)
        {
            ParseRequestPagingDate(request);
            Expression<Func<SiocCustomer, bool>> predicate;
            predicate = model =>
                (string.IsNullOrWhiteSpace(request.Keyword)
                    || model.FullName.Contains(request.Keyword)
                    || model.PhoneNumber.Contains(request.Keyword)
                    || model.Email.Contains(request.Keyword)
                    )
                && (!request.FromDate.HasValue
                    || (model.CreatedDateTime >= request.FromDate.Value)
                )
                && (!request.ToDate.HasValue
                    || (model.CreatedDateTime <= request.ToDate.Value)
                );
            var fedata = await QueenApiCustomerViewModel.Repository.GetModelListByAsync(predicate, request.OrderBy, request.Direction, 
                request.PageSize, request.PageIndex).ConfigureAwait(false);

            return JObject.FromObject(fedata);

        }

        #endregion Post
    }
}
