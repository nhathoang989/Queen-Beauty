using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Swastika.Cms.Lib.Models.Cms;
using Swastika.Cms.Lib.ViewModels.Api;
using Swastika.Cms.Lib.ViewModels.Info;
using Swastika.Domain.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QueenBeauty.Lib.ViewModels
{
    public class QueenApiCustomerViewModel
       : ViewModelBase<SiocCmsContext, SiocCustomer, QueenApiCustomerViewModel>
    {
        #region Properties

        #region Models

        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("isAgreeNotified")]
        public string IsAgreeNotified { get; set; }
        [JsonProperty("fullName")]
        public string FullName { get; set; }
        [Required]
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty("birthday")]
        public DateTime? BirthDay { get; set; }
        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        #endregion

        #region Views
        //[JsonProperty("orders")]
        //public List<InfoOrderViewModel> Orders { get; set; }
        [JsonIgnore]
        [JsonProperty("specificulture")]
        public new string Specificulture { get; set; }
        #endregion

        #endregion

        #region Contructors

        public QueenApiCustomerViewModel() : base()
        {
        }

        public QueenApiCustomerViewModel(SiocCustomer model, SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
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
                IsValid = !ApiCustomerViewModel.Repository.CheckIsExists(c => c.PhoneNumber == PhoneNumber, _context, _transaction);
                if (!IsValid)
                {
                    Errors.Add("This phone number already existed");
                }
            }
        }
        public override SiocCustomer ParseModel(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.NewGuid().ToString();
                CreatedDateTime = DateTime.UtcNow;
            }
            return base.ParseModel(_context, _transaction);
        }

        public override void ExpandView(SiocCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            //Orders = InfoOrderViewModel.Repository.GetModelListBy(o => o.CustomerId == Id, _context, _transaction).Data;
        }
        #endregion Overrides
    }
}
