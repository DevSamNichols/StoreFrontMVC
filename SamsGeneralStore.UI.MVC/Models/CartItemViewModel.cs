using SamsGeneralStore.DATA.EF.Models; //Added for access to the Product class

namespace SamsGeneralStore.UI.MVC.Models
{
    public class CartItemViewModel
    {
        public int Qty { get; set; }

        public Product Product { get; set; }
        //The above is an example of a concept called "Containment":
        //This is a use of a complex datatype as a field/property in a class

        //Complex Datatype: Any class with multiple properties (DateTime, Product, ContactViewModel, etc.)
        //Primitive Datatype: A class that stores a single value (int, bool, char, decimal, etc.)

        public CartItemViewModel(int qty, Product product)
        {
            Qty = qty;
            Product = product;
        }
    }
}
