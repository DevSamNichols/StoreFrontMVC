using Microsoft.AspNetCore.Mvc;

using SamsGeneralStore.DATA.EF.Models;//grants access to the context
using Microsoft.AspNetCore.Identity;//grants access to the usermanager
using SamsGeneralStore.UI.MVC.Models;//grants access to the cartitemviewmodel
using Newtonsoft.Json;//easier management of the shopping cart

namespace SamsGeneralStore.UI.MVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        #region Steps to Implement Session-Based Shopping Cart

        /*
        * 1) Register Session in program.cs (builder.Services.AddSession... && app.UseSession())
        * 2) Create the CartItemViewModel class in [ProjName].UI.MVC/Models folder
        * 3) Add the 'Add To Cart' button in the Index and/or Details view of your Products //TODO
        * 4) Create the ShoppingCartController (empty controller -> named ShoppingCartController)
        *      - add using statements
        *          - using GadgetStore.DATA.EF.Models;
        *          - using Microsoft.AspNetCore.Identity;
        *          - using GadgetStore.UI.MVC.Models;
        *          - using Newtonsoft.Json;
        *      - Add props for the GadgetStoreContext && UserManager
        *      - Create a constructor for the controller - assign values to context && usermanager
        *      - Code the AddToCart() action
        *      - Code the Index() action
        *      - Code the Index View
        *          - Start with the basic table structure
        *          - Show the items that are easily accessible (like the properties from the model)
        *          - Calculate/show the lineTotal
        *          - Add the RemoveFromCart <a>
        *      - Code the RemoveFromCart() action
        *          - verify the button for RemoveFromCart in the Index view is coded with the controller/action/id
        *      - Add UpdateCart <form> to the Index View
        *      - Code the UpdateCart() action
        *      - Add Submit Order button to Index View
        *      - Code SubmitOrder() action
        * */

        #endregion

        //FIELDS
        private readonly StoreFrontContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        //CONSTRUCTOR
        public ShoppingCartController(StoreFrontContext context, UserManager<IdentityUser> userManager)
        {
            //ASSIGNMENT
            _context = context;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            //Retrieve the contents from the Session shopping cart (stored as JSON)
            //and convert them to C# via Newtonsoft.Json. After converting to C#, 
            //we can then pass the collection of cart contents back to the View for display.

            //Retrieve the cart contents
            var sessionCart = HttpContext.Session.GetString("cart");

            //Create the shell for the local (C# version) of the cart
            Dictionary<int, CartItemViewModel> shoppingCart = null;

            //Check to see if the session cart was null
            if (sessionCart == null || sessionCart.Count() == 0)
            {
                //The user either hasn't put anything in the cart, or they have removed 
                //all items from the cart. So, set shoppingCart to an empty object
                shoppingCart = new Dictionary<int, CartItemViewModel>();

                ViewBag.Message = "There are no items in your cart.";
            }
            else
            {
                ViewBag.Message = null;

                //Deserialize the cart contents from JSON to C#
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }

            //Send the shopping cart to the View
            return View(shoppingCart);
        }

        public IActionResult AddToCart(int id)
        {
            //Create an empty shell for a LOCAL shopping cart variable
            //Dictionary<key, value>
            //int (key) = ProductID
            //CartItemViewModel (value) = Qty & Product

            Dictionary<int, CartItemViewModel> shoppingCart = null;

            #region Session Notes

            /*
             * Session is a tool available on the server-side that can store information 
             * for a user while they are actively using your site.
             * 
             * Typically, the session lasts for 20 minutes (this can be adjusted in Program.cs).
             * Once the 20 minutes is up, the session variable is disposed.
             * 
             * Values we can store in the Session are limited to: string, int
             *  - Because of this, we'll have to get creative when trying to store complex 
             *    objects (like our CartItemViewModel).
             * 
             * To keep the information separated into properties, we will convert the C# object
             * to a JSON string.
             */

            #endregion


            //Create a local variable that gets the value of the current cart in
            //the session (if one exists)

            var sessionCart = HttpContext.Session.GetString("cart");

            //Check to see if the session object exists
            //If not, instantiate a new local collection
            if (String.IsNullOrEmpty(sessionCart))
            {
                //If the session didn't exist yet (sessionCart was null/empty),
                //instantiate the collection as a new object
                shoppingCart = new Dictionary<int, CartItemViewModel>();
            }
            else
            {
                //Cart already exists -- transfer the cart items from the session into our local cart
                //DeserializeObject() is a method that converts JSON to C#. We MUST tell this method 
                //which C# class to convert the JSON into (here, that will be Dictionary<int, CartItemViewModel>)
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }

            //Add newly selected products to the cart
            //Retrieve the Product from the database
            Product product = _context.Products.Find(id);

            //Instantiate the CartItemViewModel object so we can add it to the cart
            CartItemViewModel civm = new CartItemViewModel(1, product); //Add 1 of the selected product to the cart

            //If the Product was already in the cart, let's increase the quantity by 1
            //Otherwise, add the new item to the cart
            if (shoppingCart.ContainsKey(product.ProductId))
            {
                //Update the Qty
                shoppingCart[product.ProductId].Qty++;
            }
            else
            {
                shoppingCart.Add(product.ProductId, civm);
            }

            //Update the session version of the cart
            //Take the local copy and serialize it as JSON, then assign that value to our session
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);
            HttpContext.Session.SetString("cart", jsonCart);


            return RedirectToAction("Index");
        }


        public IActionResult RemoveFromCart(int id)
        {
            //Retrieve the cart from the session
            var sessionCart = HttpContext.Session.GetString("cart");

            //Convert JSON cart to C#
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //Remove the cart item
            shoppingCart.Remove(id);

            //If there are no remaining items in the cart, remove it from session
            if (shoppingCart.Count == 0)
            {
                HttpContext.Session.Remove("cart");
            }
            //Otherwise, update the session variable with our local cart variable
            else
            {
                string jsonCart = JsonConvert.SerializeObject(shoppingCart);
                HttpContext.Session.SetString("cart", jsonCart);
            }

            return RedirectToAction("Index");
        }


        //The parameters in the below action will receive their information
        //from the <input>s in the <form>. The parameter names MUST match
        //the name attributes of those <input>s. (Spelling matters, casing doesn't)
        public IActionResult UpdateCart(int productId, int qty)
        {
            //Retrieve the cart from the session
            var sessionCart = HttpContext.Session.GetString("cart");

            //Deserialize from JSON to C#
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //Update the Qty for our specific dictionary item (found using its key)
            shoppingCart[productId].Qty = qty;

            //Update the session with the updated cart
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);
            HttpContext.Session.SetString("cart", jsonCart);

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> SubmitOrder()
        {
            #region Planning Out Order Submission

            /*
             * BIG PICTURE PLAN:
             *  - Create Order object, then save to the DB
             *      - UserId
             *      - OrderDate
             *      - ShipToName, ShipCity, ShipState, ShipZip
             *          - This information needs to be pulled from the UserDetails record
             *  - Add the record to _context
             *  - Save DB changes
             *  
             *  - Create OrderProducts object for each item in the cart
             *      - ProductId => available from the cart
             *      - OrderId => available from the Order object
             *      - Qty => available from the cart
             *      - ProductPrice => available from the product, which is in the cart
             *  - Add the record to the _context
             *  - Save DB changes
             * 
             */

            #endregion

            //Retrieve the current user's Id
            //This is a mechanism provided by Identity to retrieve the UserId in the current
            //HttpContext. If you need to retrieve the ID in ANY controller, you can use this
            string? userId = (await _userManager.GetUserAsync(HttpContext.User))?.Id;

            //Retrieve the UserDetails record
            UserDetail ud = _context.UserDetails.Find(userId);

            //Create the Order object and assign values
            Order o = new Order()
            {
                OrderDate = DateTime.Now,
                UserId = userId,
                ShipCity = ud.City,
                ShipToName = ud.FirstName + " " + ud.LastName,
                ShipState = ud.State,
                ShipZip = ud.Zip
            };

            //Add the Order to the _context
            _context.Orders.Add(o);


            //Retrieve the session cart
            var sessionCart = HttpContext.Session.GetString("cart");

            //Convert from JSON to C#
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //Create the OrderProduct object for each item in the cart
            foreach (var item in shoppingCart)
            {
                OrderProduct op = new OrderProduct()
                {
                    OrderId = o.OrderId,
                    ProductId = item.Key,
                    ProductPrice = item.Value.Product.Price,
                    Quantity = (short)item.Value.Qty
                };

                //Only need to add items to an existing entity if the items are a RELATED
                //record (like from a linking table)
                o.OrderProducts.Add(op);
            }

            //Save the changes to the database
            _context.SaveChanges();
            return RedirectToAction("Index", "Orders");

        }


    }
}
