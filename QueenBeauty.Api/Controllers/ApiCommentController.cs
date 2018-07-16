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
    [Route("api/{culture}/queen-beauty/comment")]
    public class ApiCommentController :
        BaseApiController
    {
        public ApiCommentController()
        {
        }
        #region Get

        // GET api/Comment/id
        [HttpGet]
        [Route("delete/{id}")]
        public async Task<RepositoryResponse<SiocComment>> DeleteAsync(Guid id)
        {
            var getPage = await QueenApiCommentViewModel.Repository.GetSingleModelAsync(
                model => model.Id == id && model.Specificulture == _lang);
            if (getPage.IsSucceed)
            {

                return await getPage.Data.RemoveModelAsync(true);
            }
            else
            {
                return new RepositoryResponse<SiocComment>()
                {
                    IsSucceed = false
                };
            }
        }

        // GET api/pages/id
        [HttpGet]
        [Route("details/{id}")]
        [Route("details")]
        public async Task<JObject> Details(Guid? id)
        {
            if (id.HasValue)
            {
                var beResult = await QueenApiCommentViewModel.Repository.GetSingleModelAsync(model => model.Id == id && model.Specificulture == _lang).ConfigureAwait(false);
                return JObject.FromObject(beResult);
            }
            else
            {
                var model = new SiocComment();
                RepositoryResponse<QueenApiCommentViewModel> result = new RepositoryResponse<QueenApiCommentViewModel>()
                {
                    IsSucceed = true,
                    Data = new QueenApiCommentViewModel(model)
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

        // POST api/Comment
        //[Authorize]
        [HttpPost, HttpOptions]
        [Route("save")]
        public async Task<RepositoryResponse<QueenApiCommentViewModel>> Save([FromBody]QueenApiCommentViewModel model)
        {
            if (model != null)
            {
                model.Specificulture = _lang;
                var result = await model.SaveModelAsync(true).ConfigureAwait(false);
                return result;
            }
            return new RepositoryResponse<QueenApiCommentViewModel>();
        }

        // POST api/Comment
        [HttpPost, HttpOptions]
        [Route("save/{id}")]
        public async Task<RepositoryResponse<SiocComment>> SaveFields(Guid id, [FromBody]List<EntityField> fields)
        {
            if (fields != null)
            {
                var result = new RepositoryResponse<SiocComment>() { IsSucceed = true };
                foreach (var property in fields)
                {
                    if (result.IsSucceed)
                    {
                        result = await QueenApiCommentViewModel.Repository.UpdateFieldsAsync(c => c.Id == id && c.Specificulture == _lang, fields).ConfigureAwait(false);
                    }
                    else
                    {
                        break;
                    }

                }
                return result;
            }
            return new RepositoryResponse<SiocComment>();
        }

        // GET api/Comment
        [HttpPost, HttpOptions]
        [Route("list")]
        public async Task<JObject> GetList(
            [FromBody] RequestPaging request)
        {
            ParseRequestPagingDate(request);
            Expression<Func<SiocComment, bool>> predicate;
            predicate = model =>
                model.Specificulture == _lang
                && (string.IsNullOrWhiteSpace(request.Keyword)
                    || model.Content.Contains(request.Keyword))
                && (!request.FromDate.HasValue
                    || (model.CreatedDateTime >= request.FromDate.Value)
                )
                && (!request.ToDate.HasValue
                    || (model.CreatedDateTime <= request.ToDate.Value)
                );
            var fedata = await QueenApiCommentViewModel.Repository.GetModelListByAsync(predicate, request.OrderBy, request.Direction, request.PageSize, request.PageIndex).ConfigureAwait(false);

            return JObject.FromObject(fedata);

        }

        #endregion Post
    }
}
