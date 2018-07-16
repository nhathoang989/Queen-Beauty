// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.Data.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swastika.Cms.Lib;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.Services;
using Swastika.Domain.Core.ViewModels;
using Swastika.Domain.Data.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Swastika.Common.Utility.Enums;

namespace QueenBeauty.Lib.ViewModels
{
    public class QueenApiProductViewModel
       : ViewModelBase<SiocCmsContext, SiocProduct, QueenApiProductViewModel>
    {
        #region Properties

        #region Models

        [JsonProperty("id")]
        public string Id { get; set; }

        [Required]
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("priceUnit")]
        public string PriceUnit { get; set; }
        
        #endregion Models

        #region Views


        #endregion Views

        #endregion Properties

        #region Contructors

        public QueenApiProductViewModel() : base()
        {
        }

        public QueenApiProductViewModel(SiocProduct model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
        {
        }

        #endregion Contructors

        #region Expands

        public static async Task<RepositoryResponse<PaginationModel<QueenApiProductViewModel>>> GetModelListByCategoryAsync(
            int categoryId, string specificulture
            , string orderByPropertyName, OrderByDirection direction
            , int? pageSize = 1, int? pageIndex = 0
            , SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            SiocCmsContext context = _context ?? new SiocCmsContext();
            var transaction = _transaction ?? context.Database.BeginTransaction();
            try
            {
                var query = context.SiocCategoryProduct.Include(ac => ac.SiocProduct)
                    .Where(ac =>
                    ac.CategoryId == categoryId && ac.Specificulture == specificulture
                    && ac.Status == (int)SWStatus.Published).Select(ac => ac.SiocProduct);
                PaginationModel<QueenApiProductViewModel> result = await Repository.ParsePagingQueryAsync(
                    query, orderByPropertyName
                    , direction,
                    pageSize, pageIndex, context, transaction
                    );
                return new RepositoryResponse<PaginationModel<QueenApiProductViewModel>>()
                {
                    IsSucceed = true,
                    Data = result
                };
            }
            catch (Exception ex) // TODO: Add more specific exeption types instead of Exception only
            {
                Repository.LogErrorMessage(ex);
                if (_transaction == null)
                {
                    //if current transaction is root transaction
                    transaction.Rollback();
                }

                return new RepositoryResponse<PaginationModel<QueenApiProductViewModel>>()
                {
                    IsSucceed = false,
                    Data = null,
                    Exception = ex
                };
            }
            finally
            {
                if (_context == null)
                {
                    //if current Context is Root
                    context.Dispose();
                }
            }
        }

        #region Sync

        public static RepositoryResponse<PaginationModel<QueenApiProductViewModel>> GetModelListByCategory(
           int categoryId, string specificulture
           , string orderByPropertyName, OrderByDirection direction
           , int? pageSize = 1, int? pageIndex = 0
           , SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            SiocCmsContext context = _context ?? new SiocCmsContext();
            var transaction = _transaction ?? context.Database.BeginTransaction();
            try
            {
                var query = context.SiocCategoryProduct.Include(ac => ac.SiocProduct)
                    .Where(ac =>
                    ac.CategoryId == categoryId && ac.Specificulture == specificulture
                    && ac.Status == (int)SWStatus.Published).Select(ac => ac.SiocProduct);
                PaginationModel<QueenApiProductViewModel> result = Repository.ParsePagingQuery(
                    query, orderByPropertyName
                    , direction,
                    pageSize, pageIndex, context, transaction
                    );
                return new RepositoryResponse<PaginationModel<QueenApiProductViewModel>>()
                {
                    IsSucceed = true,
                    Data = result
                };
            }
            catch (Exception ex) // TODO: Add more specific exeption types instead of Exception only
            {
                Repository.LogErrorMessage(ex);
                if (_transaction == null)
                {
                    //if current transaction is root transaction
                    transaction.Rollback();
                }

                return new RepositoryResponse<PaginationModel<QueenApiProductViewModel>>()
                {
                    IsSucceed = false,
                    Data = null,
                    Exception = ex
                };
            }
            finally
            {
                if (_context == null)
                {
                    //if current Context is Root
                    context.Dispose();
                }
            }
        }

        public static RepositoryResponse<PaginationModel<QueenApiProductViewModel>> GetModelListByModule(
          int ModuleId, string specificulture
          , string orderByPropertyName, OrderByDirection direction
          , int? pageSize = 1, int? pageIndex = 0
          , SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            SiocCmsContext context = _context ?? new SiocCmsContext();
            var transaction = _transaction ?? context.Database.BeginTransaction();
            try
            {
                var query = context.SiocModuleProduct.Include(ac => ac.SiocProduct)
                    .Where(ac =>
                    ac.ModuleId == ModuleId && ac.Specificulture == specificulture
                    && (ac.Status == (int)SWStatus.Published || ac.Status == (int)SWStatus.Preview)).Select(ac => ac.SiocProduct);
                PaginationModel<QueenApiProductViewModel> result = Repository.ParsePagingQuery(
                    query, orderByPropertyName
                    , direction,
                    pageSize, pageIndex, context, transaction
                    );
                return new RepositoryResponse<PaginationModel<QueenApiProductViewModel>>()
                {
                    IsSucceed = true,
                    Data = result
                };
            }
            catch (Exception ex) // TODO: Add more specific exeption types instead of Exception only
            {
                Repository.LogErrorMessage(ex);
                if (_transaction == null)
                {
                    //if current transaction is root transaction
                    transaction.Rollback();
                }

                return new RepositoryResponse<PaginationModel<QueenApiProductViewModel>>()
                {
                    IsSucceed = false,
                    Data = null,
                    Exception = ex
                };
            }
            finally
            {
                if (_context == null)
                {
                    //if current Context is Root
                    context.Dispose();
                }
            }
        }

        #endregion Sync

        #endregion Expands
    }
}
