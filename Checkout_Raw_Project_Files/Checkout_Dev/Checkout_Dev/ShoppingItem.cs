using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkout_Dev
{
    class ShoppingItem
    {
        public string ItemCode { get; set; }
        public decimal Cost { get; set; }

        public ShoppingItem(string itemCode, decimal cost)
        {
            ItemCode = itemCode;
            Cost = cost;
        }

    }
}
