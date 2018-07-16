// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.ViewModels.Info;
using Swastika.Domain.Data.ViewModels;

namespace QueenBeauty.Lib.ViewModels.Navigations
{
    public class QueenNavCategoryProductViewModel
        : ViewModelBase<SiocCmsContext, SiocCategoryProduct, QueenNavCategoryProductViewModel>
    {
        public QueenNavCategoryProductViewModel(SiocCategoryProduct model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
            : base(model, _context, _transaction)
        {
        }

        public QueenNavCategoryProductViewModel() : base()
        {
        }
        [JsonProperty("productId")]
        public string ProductId { get; set; }
        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }

        [JsonProperty("isActived")]
        public bool IsActived { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonIgnore]
        public new bool IsValid { get; set; }
        #region Views
        [JsonProperty("product")]
        public QueenApiProductViewModel Product { get; set; }

        #endregion Views

        #region overrides

        public override void ExpandView(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var getProduct = QueenApiProductViewModel.Repository.GetSingleModel(p => p.Id == ProductId && p.Specificulture == Specificulture
                , _context: _context, _transaction: _transaction
            );
            if (getProduct.IsSucceed)
            {
                Product = getProduct.Data;
            }
        }

        #region Async

        #endregion Async

        #endregion overrides
    }
}
