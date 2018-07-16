using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.ViewModels.Info;
using Swastika.Domain.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueenBeauty.Lib.ViewModels
{
    public class QueenApiOrderItemViewModel : ViewModelBase<SiocCmsContext, SiocOrderItem, QueenApiOrderItemViewModel>
    {
        #region Properties

        #region Models

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("orderId")]
        public int OrderId { get; set; }
        [JsonProperty("productId")]
        public string ProductId { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        [JsonProperty("price")]
        public double Price { get; set; }
        [JsonProperty("priceUnit")]
        public string PriceUnit { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonIgnore]
        [JsonProperty("storeId")]
        public int StoreId { get; set; }

        #endregion

        #region Views

        public QueenApiProductViewModel Product { get; set; }

        #endregion

        #endregion

        #region Contructors

        public QueenApiOrderItemViewModel() : base()
        {
        }

        public QueenApiOrderItemViewModel(SiocOrderItem model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
            : base(model, _context, _transaction)
        {
        }

        #endregion Contructors

        #region Overrides

        public override void Validate(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            base.Validate(_context, _transaction);
            if (IsValid)
            {
                if (!QueenApiOrderViewModel.Repository.CheckIsExists(o => o.Id == OrderId, _context, _transaction))
                {
                    Errors.Add("Invalid Order");
                    IsValid = false;
                }
                if (!QueenApiProductViewModel.Repository.CheckIsExists(p => p.Id == ProductId && p.Specificulture == Specificulture, _context, _transaction))
                {
                    Errors.Add("Invalid Product");
                    IsValid = false;
                }
                if (QueenApiOrderItemViewModel.Repository.CheckIsExists(i => i.ProductId == ProductId
                    && i.OrderId == OrderId 
                    && i.Specificulture == Specificulture, _context, _transaction))
                {
                    Errors.Add("Invalid Item");
                    IsValid = false;
                }
            }
        }

        public override SiocOrderItem ParseModel(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var product = QueenApiProductViewModel.Repository.GetSingleModel(p => p.Id == ProductId && p.Specificulture == Specificulture).Data;
            Price = product?.Price ?? 0;
            Quantity = 1;
            PriceUnit = product?.PriceUnit;
            if (Id == 0)
            {
                Id = QueenApiOrderItemViewModel.Repository.Max(o => o.Id, _context, _transaction).Data + 1;
            }
            return base.ParseModel(_context, _transaction);
        }

        public override void ExpandView(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            Product = QueenApiProductViewModel.Repository.GetSingleModel(p => p.Id == ProductId && p.Specificulture == Specificulture, _context, _transaction).Data;
        }
        #endregion Overrides
    }
}
