using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.ViewModels.Api;
using Swastika.Domain.Core.ViewModels;
using Swastika.Domain.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static QueenBeauty.Lib.QueenConstants;

namespace QueenBeauty.Lib.ViewModels
{
    public class QueenApiOrderViewModel
        : ViewModelBase<SiocCmsContext, SiocOrder, QueenApiOrderViewModel>
    {
        #region Properties

        #region Models

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonIgnore]
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }
        [JsonIgnore]
        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }
        [JsonIgnore]
        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }
        [JsonIgnore]
        [JsonProperty("storeId")]
        public int StoreId { get; set; }

        #endregion

        #region View
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("items")]
        public List<QueenApiOrderItemViewModel> Items { get; set; }

        [JsonProperty("customer")]
        public ApiCustomerViewModel Customer { get; set; }

        [JsonProperty("comments")]
        public List<QueenApiCommentViewModel> Comments{ get; set; }

        [JsonProperty("status")]
        public new int Status { get; set; }

        #endregion

        #endregion

        #region Contructors

        public QueenApiOrderViewModel() : base()
        {
        }

        public QueenApiOrderViewModel(SiocOrder model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
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
                var getCustomer = ApiCustomerViewModel.Repository.GetSingleModel(c => c.PhoneNumber == PhoneNumber, _context, _transaction);
                
                if (!getCustomer.IsSucceed)
                {
                    IsValid = false;
                    Errors.Add("Invalid Customer");
                }
                else
                {
                    CustomerId = getCustomer.Data.Id;
                   
                }
                var checkOrder = ApiOrderViewModel.Repository.CheckIsExists(c => c.CustomerId == CustomerId && c.Status == (int)OrderStatus.Waiting && c.CreatedDateTime.Date == DateTime.UtcNow.Date);
                if (checkOrder)
                {
                    IsValid = false;
                    Errors.Add("Order already existed");
                }



            }
        }

        public override SiocOrder ParseModel(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            if (Id == 0)
            {
                Id = QueenApiOrderViewModel.Repository.Max(o => o.Id, _context, _transaction).Data + 1;
                CreatedDateTime = DateTime.UtcNow;
            }
            return base.ParseModel(_context, _transaction);
        }
        public override void ExpandView(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var getCustomer = ApiCustomerViewModel.Repository.GetSingleModel(c => c.Id == CustomerId);
            Customer = getCustomer.Data;
            PhoneNumber = Customer.PhoneNumber;
            var getItems = QueenApiOrderItemViewModel.Repository.GetModelListBy(i => i.OrderId == Id && i.Specificulture== Specificulture, _context, _transaction);
            Items = getItems.Data;
            var getComments = QueenApiCommentViewModel.Repository.GetModelListBy(i => i.OrderId == Id && i.Specificulture == Specificulture, _context, _transaction);
            Comments = getComments.Data;
        }

        public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(SiocOrder parent, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var result = new RepositoryResponse<bool> { IsSucceed = true };
            foreach (var item in Items)
            {
                var saveItem = await item.SaveModelAsync(false, _context, _transaction);
                result.IsSucceed = result.IsSucceed && saveItem.IsSucceed;
                if (!saveItem.IsSucceed)
                {
                    result.Errors.AddRange(saveItem.Errors);
                }
            }
            return result;
        }

        public override RepositoryResponse<bool> SaveSubModels(SiocOrder parent, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var result = new RepositoryResponse<bool> { IsSucceed = true };
            foreach (var item in Items)
            {
                if (item.Id == 0)
                {

                    var saveItem = item.SaveModel(false, _context, _transaction);
                    result.IsSucceed = result.IsSucceed && saveItem.IsSucceed;
                    if (!saveItem.IsSucceed)
                    {
                        result.Errors.AddRange(saveItem.Errors);
                    }
                }
            }
            return result;
        }
        #endregion Overrides
    }
}
