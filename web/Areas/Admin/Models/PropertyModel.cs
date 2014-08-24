using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Areas.Admin.Models
{
    public class PropertyModel 
    {
        public ProductHeaders header { get; set; }
        public List <ProductInformation> ProductInfo { get; set; }
    }

    public class PropertyUpdateModel
    {
        public ProductHeaders header { get; set; }
        public ProductInformation ProductInfo { get; set; }
    }
}
