using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkout_Dev
{
    class ShoppingBasketItem
    {
        public string ItemCode { get; set; }
        public int Quantity { get; set; }

        public ShoppingBasketItem(string itemCode, int quantity)
        {
            ItemCode = itemCode;
            Quantity = quantity;
        }

    }
}
