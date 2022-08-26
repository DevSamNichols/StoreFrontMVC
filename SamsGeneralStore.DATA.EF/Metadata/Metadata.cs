using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;//Added for access to required, display, etc...

namespace SamsGeneralStore.DATA.EF//.Metadata
{
    #region ProductType

    public class ProductTypeMetadata
    {
        //since this a PK, we should never see it in a view, or have to enter a value when creating/editing.
        //For those reasons, we will not need to apply any metadata to a PK
        public int ProductTypeID { get; set; }

        [Required(ErrorMessage = "* Product Type Name is required")]
        [StringLength(100)]
        [Display(Name = "Type")]
        public string ProductTypeName { get; set; } = null!;
    }

    #endregion

    #region StockStatus

    public class StockStatusMetadata
    {
        public int StockStatusID { get; set; }

        [Required(ErrorMessage = "* Stock Status is required")]
        [StringLength(50)]
        [Display(Name = "Stock Status")]
        public string StockStatusName { get; set; } = null!;
    }

    #endregion

    #region Orders

    public class OrderMetadata
    {
        public int OrderID { get; set; }

        //no metadata needed for FKs - as they are represented in a View by a dropdown list
        public string UserID { get; set; } = null!;

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]//0:d => MM/dd/yyyy
        [Display(Name = "Order Date")]
        [Required]
        public DateTime OrderDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Ship To")]
        [Required]
        public string ShipToName { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "City")]
        [Required]
        public string ShipCity { get; set; } = null!;

        [StringLength(2)]
        [Display(Name = "State")]
        public string? ShipState { get; set; }

        [StringLength(5)]
        [Display(Name = "Zip")]
        [Required]
        [DataType(DataType.PostalCode)]
        public string ShipZip { get; set; } = null!;
    }

    #endregion

    #region Products

    public class ProductMetadata
    {
        //PK
        public int ProductID { get; set; }

        [StringLength(200)]
        [DisplayFormat(NullDisplayText = "N/A")]
        public string ? Description { get; set; }

        //FK
        public int ProductTypeID { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "0:c")]
        [Range(0, (double)decimal.MaxValue)]
        public decimal? Price { get; set; }

        [StringLength(100)]
        [Display(Name = "Product Name")]
        [Required]
        public string ProductName { get; set; } = null!;


        [DisplayFormat(NullDisplayText = "N/A")]
        [Display(Name = "Products Sold")]
        public int ? ProductsSold { get; set; }

        //FK
        public int ManufacturerID { get; set; }

        //FK
        public int StockStatusID { get; set; }

        [DisplayFormat(NullDisplayText = "N/A")]
        [StringLength(75)]
        [Display(Name = "Image")]
        public string ? Image { get; set; }

    }

    #endregion

    #region Manufacturers

    public class ManufacturerMetadata
    {
        //PK
        public int ManufacturerID { get; set; }

        [StringLength(100)]
        [Display(Name = "Manufacturer")]
        [Required]
        public string ManufacturerName { get; set; } = null!;
    }

    #endregion

    #region UserDetails

    public class UserDetailMetadata
    {
        //PK
        public string UserId { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; } = null!;

        [StringLength(150)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(2)]
        public string? State { get; set; }

        [StringLength(5)]
        [DataType(DataType.PostalCode)]
        public string? Zip { get; set; }

        [StringLength(24)]
        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }
    }

    #endregion

    #region OrderProducts

    public class OrderProductMetadata
    {
        public int OrderProductID { get; set; }

        public int ProductID { get; set; }

        public int OrderID { get; set; }

        [DisplayFormat(NullDisplayText = "N/A")]
        public int? Quantity { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "0:c")]
        [Display(Name = "Product Price")]
        [Range(0, (double)decimal.MaxValue)]
        public decimal? ProductPrice { get; set; }
    }

    #endregion
}
