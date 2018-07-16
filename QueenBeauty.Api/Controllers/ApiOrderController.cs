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
using static QueenBeauty.Lib.QueenConstants;
using static Swastika.Common.Utility.Enums;

namespace QueenBeauty.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/{culture}/queen-beauty/order")]
    public class ApiOrderController :
        BaseApiController
    {
        public ApiOrderController()
        {
        }
        #region Get

        // GET api/Order/id
        [HttpGet]
        [Route("delete/{id}")]
        public async Task<RepositoryResponse<SiocOrder>> DeleteAsync(int id)
        {
            var getPage = await QueenApiOrderViewModel.Repository.GetSingleModelAsync(
                model => model.Id == id && model.Specificulture == _lang);
            if (getPage.IsSucceed)
            {

                return await getPage.Data.RemoveModelAsync(true);
            }
            else
            {
                return new RepositoryResponse<SiocOrder>()
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
                var beResult = await QueenApiOrderViewModel.Repository.GetSingleModelAsync(model => model.Id == id && model.Specificulture == _lang).ConfigureAwait(false);
                return JObject.FromObject(beResult);
            }
            else
            {
                var model = new SiocOrder();
                RepositoryResponse<QueenApiOrderViewModel> result = new RepositoryResponse<QueenApiOrderViewModel>()
                {
                    IsSucceed = true,
                    Data = new QueenApiOrderViewModel(model)
                    {
                        Specificulture = _lang,
                        Status = (int)OrderStatus.Waiting,
                    }
                };
                return JObject.FromObject(result);
            }
        }

        #endregion Get

        #region Post

        // POST api/Order
        //[Authorize]
        [HttpPost, HttpOptions]
        [Route("save")]
        [Route("book")]
        public async Task<JObject> Save([FromBody]QueenApiOrderViewModel model)
        {
            if (model != null)
            {
                model.Specificulture = _lang;
                var result = await model.SaveModelAsync(true).ConfigureAwait(false);
                if (result.IsSucceed)
                {
                    var getOrders = await QueenApiOrderViewModel.Repository.GetModelListByAsync(
                        o => o.Status == (int)OrderStatus.Waiting && o.CreatedDateTime.Date == DateTime.UtcNow.Date.Date);
                    return JObject.FromObject(getOrders);
                }
                return JObject.FromObject(result);
            }
            var temp = new RepositoryResponse<QueenApiOrderViewModel>();
            return JObject.FromObject(temp);
        }

        // POST api/Order
        [HttpPost, HttpOptions]
        [Route("save/{id}")]
        public async Task<RepositoryResponse<SiocOrder>> SaveFields(int id, [FromBody]List<EntityField> fields)
        {
            if (fields != null)
            {
                var result = new RepositoryResponse<SiocOrder>() { IsSucceed = true };
                foreach (var property in fields)
                {
                    if (result.IsSucceed)
                    {
                        result = await QueenApiOrderViewModel.Repository.UpdateFieldsAsync(c => c.Id == id && c.Specificulture == _lang, fields).ConfigureAwait(false);
                    }
                    else
                    {
                        break;
                    }

                }
                return result;
            }
            return new RepositoryResponse<SiocOrder>();
        }

        // GET api/Order
        [HttpPost, HttpOptions]
        [Route("list")]
        public async Task<JObject> GetList(
            [FromBody] RequestPaging request)
        {
            int.TryParse(request.Key, out int status);
            if (!request.FromDate.HasValue)
            {
                request.FromDate = DateTime.Now;
            }
            ParseRequestPagingDate(request);
            Expression<Func<SiocOrder, bool>> predicate;
            predicate = model =>
                model.Specificulture == _lang
                 && model.Status == status
                && (string.IsNullOrWhiteSpace(request.Keyword)
                    || (model.Customer.FullName.Contains(request.Keyword)
                    || model.Customer.PhoneNumber.Contains(request.Keyword))
                    )
                && (
                    (!request.FromDate.HasValue)
                    || (model.CreatedDateTime >= request.FromDate.Value)
                )
                && (
                    (!request.ToDate.HasValue)
                    || (model.CreatedDateTime <= request.ToDate.Value)
                );
            var fedata = await QueenApiOrderViewModel.Repository.GetModelListByAsync(predicate, request.OrderBy, request.Direction, request.PageSize, request.PageIndex).ConfigureAwait(false);

            return JObject.FromObject(fedata);

        }

        #endregion Post
    }
}
