using Microsoft.Data.OData.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using QueenBeauty.Lib.ViewModels.Navigations;
using Swastika.Cms.Lib;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.Services;
using Swastika.Cms.Lib.ViewModels.Navigation;
using Swastika.Domain.Core.ViewModels;
using Swastika.Domain.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Swastika.Cms.Lib.SWCmsConstants;

namespace QueenBeauty.Lib.ViewModels
{
    public class QueenApiCategoryViewModel
       : ViewModelBase<SiocCmsContext, SiocCategory, QueenApiCategoryViewModel>
    {
        #region Properties

        #region Models

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("excerpt")]
        public string Excerpt { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonIgnore]
        public new bool IsValid { get; set; }
        #endregion Models

        #region Views


        [JsonProperty("products")]
        public List<QueenApiProductViewModel> Products { get; set; } = new List<QueenApiProductViewModel>();

        #endregion Views

        #endregion Properties

        #region Contructors

        public QueenApiCategoryViewModel() : base()
        {
        }

        public QueenApiCategoryViewModel(SiocCategory model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
        {
        }

        #endregion Contructors

        #region Overrides

        public override void ExpandView(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            GetSubProducts(_context, _transaction);
        }

        public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(QueenApiCategoryViewModel view, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            RepositoryResponse<bool> result = new RepositoryResponse<bool>() { IsSucceed = true };
            if (result.IsSucceed)
            {
                var removeResult = await QueenNavCategoryProductViewModel.Repository.RemoveListModelAsync(n => n.CategoryId == Id && n.Specificulture == Specificulture, _context, _transaction);
                result.IsSucceed = result.IsSucceed && removeResult.IsSucceed;
                if (!result.IsSucceed)
                {
                    result.Errors.AddRange(removeResult.Errors);
                    result.Exception = removeResult.Exception;
                }
            }
            return result;
        }

        #endregion Overrides

        #region Expands

        #region Sync


        private void GetSubProducts(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var getProducts = QueenNavCategoryProductViewModel.Repository.GetModelListBy(
               m => m.CategoryId == Id && m.Specificulture == Specificulture
           , SWCmsConstants.Default.OrderBy, OrderByDirection.Ascending
           , null, null
               , _context: _context, _transaction: _transaction
               );
            if (getProducts.IsSucceed)
            {
                foreach (var item in getProducts.Data.Items)
                {
                    Products.Add(item.Product);
                }
            }
        }

        #endregion Sync

        #endregion Expands
    }
}
