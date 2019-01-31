using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkout_Dev
{
    class SpecialOffer
    {
        public string ItemCode { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal OriginalPrice { get; set; }

        public SpecialOffer(string itemCode, int quantity, decimal totalPrice, decimal originalPrice)
        {
            ItemCode = itemCode;
            Quantity = quantity;
            TotalPrice = totalPrice;
            OriginalPrice = originalPrice;
        }

    }
}
