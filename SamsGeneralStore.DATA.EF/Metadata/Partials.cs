using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamsGeneralStore.DATA.EF.Models//.Metadata
{
    #region ProductType
    [ModelMetadataType(typeof(ProductTypeMetadata))]
    public partial class ProductType { }
    #endregion

    #region StockStatus
    [ModelMetadataType(typeof(StockStatusMetadata))]
    public partial class StockStatus { }
    #endregion

    #region Orders
    [ModelMetadataType(typeof(OrderMetadata))]
    public partial class Order { }
    #endregion

    #region Products
    [ModelMetadataType(typeof(ProductMetadata))]
    public partial class Product 
    {
        [NotMapped]
        public IFormFile? ProductImage { get; set; }
    }
    #endregion

    #region Manufacturers
    [ModelMetadataType(typeof(ManufacturerMetadata))]
    public partial class Manufacturer { }
    #endregion

    #region UserDetails
    [ModelMetadataType(typeof(UserDetailMetadata))]
    public partial class UserDetail 
    {
        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
    #endregion

    #region OrderProducts
    [ModelMetadataType(typeof(OrderProductMetadata))]
    public partial class OrderProduct { }
    #endregion

}//End Namespace
