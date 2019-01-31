using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Checkout_Dev
{
    class Program
    {

        static void Main(string[] args)
        {
            // Any classes added to this project are in place of the usual process I would use.
            // I would usually have a set of DBMLs containing stored procedures for data manipulation and object definitons.

            // Program variables used during processing.       
            string input;
            string itemCode;
            int itemQuantity;
            decimal itemCost;
            decimal itemBaseCost;
            ShoppingBasketItem itemToAdd;

            // Shopping basket stores a list of ShoppingBasketItems representing the current shopping basket. 
            List<ShoppingBasketItem> currentBasket = new List<ShoppingBasketItem>();

            // Load the prices. Usually from an external source.
            List<ShoppingItem> prices = new List<ShoppingItem>();
            prices.Add(new ShoppingItem("A99", 0.5m));
            prices.Add(new ShoppingItem("B15", 0.3m));
            prices.Add(new ShoppingItem("C40", 1.8m));
            prices.Add(new ShoppingItem("T23", 0.99m));

            // Load the products. Usually from an external source.
            List<string> products = new List<string>();
            products.Add("A99");
            products.Add("B15");
            products.Add("C40");
            products.Add("T23");

            // Load the original offers. These can be changed via command line. Usually from an external source.
            List<SpecialOffer> specialOffers = new List<SpecialOffer>();
            SpecialOffer offer = new SpecialOffer("A99", 3, 1.30m, 0.5m);
            specialOffers.Add(offer);
            offer = new SpecialOffer("B15", 2, 0.45m, 0.3m);
            specialOffers.Add(offer);

            // Output the start up text.
            Console.WriteLine("Welcome to the shopping basket application.");
            Console.WriteLine("At any time, you can press 'q' to quit, or 'c' to checkout.");
            Console.WriteLine("Press 'o' to update offers or enter an item code to start shopping.");

            do
            {
                input = Console.ReadLine();

                // Check if the user is trying to go to the check out at any time.
                if (input == "c")
                {
                    // Function calculates the total cost of the basket, taking special offers into account.
                    decimal totalPrice = Checkout(currentBasket, prices, specialOffers);

                    // Output total price and then allow user to quit or restart.
                    Console.WriteLine("Your total price is: £" + totalPrice + ". Press 'q' to quit or 'r' to restart the appliation.");
                    string finalStep = Console.ReadLine();

                    while((finalStep != "q") && (finalStep != "r"))
                    {
                        Console.WriteLine("Your total price is: £" + totalPrice + ". Press 'q' to quit or 'r' to restart the appliation.");
                        finalStep = Console.ReadLine();
                    }

                    if (finalStep == "q")
                    {
                        Environment.Exit(0);
                    }
                    else if (finalStep == "r")
                    {
                        var fileName = Assembly.GetExecutingAssembly().Location;
                        System.Diagnostics.Process.Start(fileName);
                    }
                }
                // Check if the user is trying to quit.
                else if (input == "q")
                {
                    Environment.Exit(0);
                }
                // Check if user is trying to change the offers.
                else if (input == "o")
                {
                    // Allow updating of offers.
                    Console.WriteLine("Please enter the item code to update the related offer.");
                    input = Console.ReadLine();

                    // Validate the input until the user enters a valid item code.
                    itemCode = ValidateItemCode(input, products);
                    Console.WriteLine("Please enter the quantity of the item relating to your offer.");
                    input = Console.ReadLine();

                    // Validate the input until the user enters a valid item quantity.
                    itemQuantity = ValidateQuantity(input);
                    Console.WriteLine("Please enter the total cost of the items relating to your offer.");
                    input = Console.ReadLine();

                    // Validate the input until the user enter a valid item cost.
                    itemCost = ValidateCost(input);
                    itemBaseCost = GetItemBaseCost(prices, itemCode);

                    // Update special offers to contain the new offer.
                    specialOffers = UpdateSpecialOffer(itemCode, itemQuantity, itemCost, itemBaseCost, specialOffers);
                    Console.WriteLine("Offers updated. Please enter a product code to begin shopping.");
                }
                // Else deal with new item to add.
                else
                {
                    itemCode = ValidateItemCode(input, products);
                    Console.WriteLine("Please enter your desired quantity.");
                    input = Console.ReadLine();
                    itemQuantity = ValidateQuantity(input);
                    itemToAdd = new ShoppingBasketItem(itemCode, itemQuantity);
                    currentBasket = AddItemToBasket(currentBasket, itemToAdd);
                    Console.WriteLine("Item added to basket. Enter another item code to add another item, press c to go to the check out or press q to quit.");
                }
            } while ((input != "q") && (input != "c"));
        }

        private static List<ShoppingBasketItem> AddItemToBasket(List<ShoppingBasketItem> currentBasket, ShoppingBasketItem itemToAdd)
        {
            bool itemAdded = false;
            // If basket is empty, just add the item.
            if (currentBasket.Count == 0)
            {
                currentBasket.Add(itemToAdd);
                itemAdded = true;
            }
            // If the basket is not empty, check whether the item already exists in the basket.
            else
            {
                foreach(ShoppingBasketItem item in currentBasket)
                {
                    if (item.ItemCode == itemToAdd.ItemCode)
                    {
                        // If we have a match, update the quantity.
                        item.Quantity = item.Quantity + itemToAdd.Quantity;
                        itemAdded = true;
                    }
                }
                // If the item didn't already exist in the basket, add as normal.
                if (!itemAdded)
                {
                    currentBasket.Add(itemToAdd);
                }
            }
            return currentBasket;
        }

        private static decimal Checkout(List<ShoppingBasketItem> basket, List<ShoppingItem> prices, List<SpecialOffer> offers)
        {
            decimal totalPrice = 0;
            decimal priceToAdd = 0;
            bool offerApplied;
            foreach(ShoppingBasketItem currentItem in basket)
            {
                offerApplied = false;
                // Check if item is in the list of offers, return the cost if it is.
                foreach (SpecialOffer offer in offers)
                {
                    if (offer.ItemCode == currentItem.ItemCode)
                    {
                        if (offer.Quantity > currentItem.Quantity)
                        {
                            offerApplied = false;
                        }
                        else
                        {
                            priceToAdd = priceToAdd + CalculateOffer(currentItem.ItemCode, currentItem.Quantity, offer);
                            offerApplied = true;
                        }
                    }
                }
                
                // If no offer was applied, just work out normal cost.
                if (!offerApplied)
                {
                    ShoppingItem foundItem = prices.Find(i => i.ItemCode == currentItem.ItemCode);
                    decimal costOfItems = currentItem.Quantity * foundItem.Cost;
                    priceToAdd = priceToAdd + costOfItems;
                }
                totalPrice = totalPrice + priceToAdd;
            }
            return totalPrice;
        }

        private static decimal CalculateOffer(string itemCode, int quantity, SpecialOffer offer)
        {
            int numberForOffer = offer.Quantity; 
            decimal priceForOffer = offer.TotalPrice; 
            decimal originalPrice = offer.OriginalPrice;
            decimal totalItemCost;
            int remainder;
            int adjustedQuantity;

            if (quantity % numberForOffer == 0)
            {
                // If the numbers divide perfectly, apply the offer.
                totalItemCost = ((quantity / numberForOffer) * priceForOffer);
                return totalItemCost;
            }
            else
            {
                // Apply the offer to as many items as possible, the rest are calculated at a normal cost.
                remainder = (quantity % numberForOffer);
                adjustedQuantity = quantity - remainder;
                totalItemCost = ((quantity / adjustedQuantity) * priceForOffer);
                totalItemCost = totalItemCost + (originalPrice * remainder);
            }
            return totalItemCost;
        }


        private static string ValidateItemCode(string input, List<String> products)
        {
            // Ensure the user enters a valid item code.
            while ((!products.Contains(input)) && (input != "q"))
            {
                Console.WriteLine("Please enter a valid item code.");
                input = Console.ReadLine();
            }

            // Check if the user has quit the application
            if (input == "q")
            {
                // Quit the application.
                return "quit";
            }
            else
            {
                // Item code is valid.
                return input;
            }
        }
      
        private static int ValidateQuantity(string input)
        {
            // Bool variables to ensure the quanity is a valid, positive integer.
            uint parse;
            bool b1 = uint.TryParse(input, out parse);
            bool b2 = input != "q";
            bool b3 = input != "0";

            while (!b1 || !b2 || !b3)
            {
                Console.WriteLine("Please enter a valid quantity.");
                input = Console.ReadLine();
                b1 = uint.TryParse(input, out parse);
                b2 = input != "q";
                b3 = input != "0";
            }

            // Check if the input is 'q'.
            if (input == "q")
            {
                // Quit the application.
                return 0;
            }
            else
            {
                // Quantity is valid.
                return Convert.ToInt32(input);
            }
        }

        private static decimal ValidateCost(string input)
        {
            // Bool variables to ensure the cost is a positive decimal.
            decimal parse;
            bool b1 = decimal.TryParse(input, out parse);
            bool b2 = input != "q";
            bool b3 = input != "0";
            bool b4 = parse > 0;

            while (!b1 || !b2 || !b3 || !b4)
            {
                Console.WriteLine("Please enter a valid cost.");
                input = Console.ReadLine();
                b1 = decimal.TryParse(input, out parse);
                b2 = input != "q";
                b3 = input != "0";
                b4 = parse > 0;
            }

            // Check if the input is 'q'.
            if (input == "q")
            {
                // Quit the application.
                return 0;
            }
            else
            {
                // Quantity is valid.
                return Convert.ToDecimal(input);
            }
        }

        protected static List<SpecialOffer> UpdateSpecialOffer(string itemCode, int itemQuantity, decimal itemCost, decimal itemBaseCost, List<SpecialOffer> specialOffers)
        {
            bool passed = false;
            bool replaceExisting = false;
            SpecialOffer newOffer = new SpecialOffer(itemCode, itemQuantity, itemCost, itemBaseCost);
            SpecialOffer oldOffer = null;
            foreach (SpecialOffer offer in specialOffers)
            {
                if (offer.ItemCode == itemCode)
                {
                    oldOffer = offer;
                    replaceExisting = true;
                }
            }

            if (!replaceExisting)
            {
                // No offer existed, so just add new one.
                specialOffers.Add(newOffer);
            }
            else
            {
                // Offer already existed, so replace the old one with the new one.
                specialOffers.Remove(oldOffer);
                specialOffers.Add(newOffer);
            }
            return specialOffers;
        }

        protected static decimal GetItemBaseCost(List<ShoppingItem> prices, string itemCode)
        {
            // 
            bool found = false;
            decimal baseCost = 0;
            while (!found)
            {
                foreach (ShoppingItem item in prices)
                {
                    if (item.ItemCode == itemCode)
                    {
                        baseCost = item.Cost;
                        found = true;
                    }
                }
            }
            return baseCost;
        }

    }
}
