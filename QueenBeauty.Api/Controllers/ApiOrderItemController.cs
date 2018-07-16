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
    [Route("api/{culture}/queen-beauty/order-item")]
    public class ApiOrderItemController :
        BaseApiController
    {
        public ApiOrderItemController()
        {
        }
        #region Get

        // GET api/OrderItem/id
        [HttpGet]
        [Route("delete/{id}")]
        public async Task<RepositoryResponse<SiocOrderItem>> DeleteAsync(int id)
        {
            var getPage = await QueenApiOrderItemViewModel.Repository.GetSingleModelAsync(
                model => model.Id == id && model.Specificulture == _lang);
            if (getPage.IsSucceed)
            {

                return await getPage.Data.RemoveModelAsync(true);
            }
            else
            {
                return new RepositoryResponse<SiocOrderItem>()
                {
                    IsSucceed = false
                };
            }
        }

        // GET api/pages/id
        [HttpGet]
        [Route("details/{id}")]
        [Route("details")]
        public async Task<JObject> Details(int? id)
        {
            if (id.HasValue)
            {
                var beResult = await QueenApiOrderItemViewModel.Repository.GetSingleModelAsync(model => model.Id == id && model.Specificulture == _lang).ConfigureAwait(false);
                return JObject.FromObject(beResult);
            }
            else
            {
                var model = new SiocOrderItem();
                RepositoryResponse<QueenApiOrderItemViewModel> result = new RepositoryResponse<QueenApiOrderItemViewModel>()
                {
                    IsSucceed = true,
                    Data = new QueenApiOrderItemViewModel(model)
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

        // POST api/OrderItem
        //[Authorize]
        [HttpPost, HttpOptions]
        [Route("save")]
        public async Task<RepositoryResponse<QueenApiOrderItemViewModel>> Save([FromBody]QueenApiOrderItemViewModel model)
        {
            if (model != null)
            {
                model.Specificulture = _lang;
                var result = await model.SaveModelAsync(true).ConfigureAwait(false);
                return result;
            }
            return new RepositoryResponse<QueenApiOrderItemViewModel>();
        }

        // POST api/OrderItem
        [HttpPost, HttpOptions]
        [Route("save/{id}")]
        public async Task<RepositoryResponse<SiocOrderItem>> SaveFields(int id, [FromBody]List<EntityField> fields)
        {
            if (fields != null)
            {
                var result = new RepositoryResponse<SiocOrderItem>() { IsSucceed = true };
                foreach (var property in fields)
                {
                    if (result.IsSucceed)
                    {
                        result = await QueenApiOrderItemViewModel.Repository.UpdateFieldsAsync(c => c.Id == id && c.Specificulture == _lang, fields).ConfigureAwait(false);
                    }
                    else
                    {
                        break;
                    }

                }
                return result;
            }
            return new RepositoryResponse<SiocOrderItem>();
        }

        // GET api/OrderItem
        [HttpPost, HttpOptions]
        [Route("list")]
        public async Task<JObject> GetList(
            [FromBody] RequestPaging request)
        {
            ParseRequestPagingDate(request);
            Expression<Func<SiocOrderItem, bool>> predicate;
            predicate = model =>
                model.Specificulture == _lang
                && (string.IsNullOrWhiteSpace(request.Keyword)
                    || (model.Description.Contains(request.Keyword)
                    ));
            var data = await QueenApiOrderItemViewModel.Repository.GetModelListByAsync(predicate, request.OrderBy, request.Direction, request.PageSize, request.PageIndex).ConfigureAwait(false);

            return JObject.FromObject(data);

        }

        #endregion Post
    }
}
