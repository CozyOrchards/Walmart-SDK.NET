using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walmart.SDK.Entities
{
    public class Product : WalmartEntity
    {
        public string ItemId { get; set; }
        public string ParentItemId { get; set; }
        public string Name { get; set; }
        public double MSRP { get; set; }
        public double SalePrice { get; set; }
        public string Upc { get; set; }
        public string CategoryPath { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string BrandName { get; set; }
        public string ThumbnailImage { get; set; }
        public string MediumImage { get; set; }
        public string LargeImage { get; set; }
        public string ProductTrackingUrl { get; set; }
        public bool NinetySevenCentShipping { get; set; }
        public decimal StandardShipRate { get; set; }
        public decimal TwoThreeDayShipRate { get; set; }
        public decimal OvernightShipRate { get; set; }
        public bool Marketplace { get; set; }
        public bool ShipToStore { get; set; }
        public bool FreeShipToStore { get; set; }
        public string ModelNumber { get; set; }
        public string ProductUrl { get; set; }
        public decimal CustomerRating { get; set; }
        public string CategoryNode { get; set; }
        public bool Bundle { get; set; }
        public bool Clearance { get; set; }
        public bool PreOrder { get; set; }
        public string Stock { get; set; }
        public bool AvailableOnline { get; set; }
    }
}
