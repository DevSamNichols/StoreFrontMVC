using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamsGeneralStore.DATA.EF.Models;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using SamsGeneralStore.UI.MVC.Utilities;

using X.PagedList; //Grants access to PagedList

namespace SamsGeneralStore.UI.MVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly StoreFrontContext _context;
        //added prop below for access to the wwwroot folder
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(StoreFrontContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;//added
        }

        // GET: Products
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var storeFrontContext = _context.Products.Include(p => p.Manufacturer).Include(p => p.ProductType).Include(p => p.StockStatus);
            return View(await storeFrontContext.ToListAsync());
        }


        //Created a separate action that returns the same results as Index, but in the View
        //we will use a tiled layout instead of a table
        public async Task<IActionResult> TiledProducts(string searchTerm, int productTypeId = 0, int page = 1)
        {
            //Create a pageSize variable
            int pageSize = 6;


            //For search, filter, paging, etc. we want products in a List
            var products = _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.Manufacturer)
                .Include(p => p.OrderProducts).ToList();


            #region Optional Category Filter

            //Create a ViewData object to send a list of Categories to the View
            //(This is similar to what you see in the Products/Create())
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName");

            //Create a ViewBag variable to persist the selected category
            ViewBag.ProductType = 0;

            if (productTypeId != 0)
            {
                products = products.Where(p => p.ProductTypeId == productTypeId).ToList();

                //Repopuplate the dropdown with the current category that is selected
                ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName", productTypeId);

                //Update the ViewBag variable
                ViewBag.Category = productTypeId;
            }

            #endregion


            #region Optional Search Filter

            if (!String.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p =>
                    p.ProductName.ToLower().Contains(searchTerm.ToLower()) ||
                    p.Manufacturer.ManufacturerName.ToLower().Contains(searchTerm.ToLower()) ||
                    p.ProductType.ProductTypeName.ToLower().Contains(searchTerm.ToLower()) ||
                    p.Description.ToLower().Contains(searchTerm.ToLower())).ToList();

                //ViewBag for total # of results
                ViewBag.NbrResults = products.Count;

                //ViewBag to persist the term searched
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                ViewBag.NbrResults = null;
                ViewBag.SearchTerm = null;
            }

            #endregion


            //return View(await products.ToListAsync());
            //return View(products);
            return View(products.ToPagedList(page, pageSize));
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Manufacturer)
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "ManufacturerName");
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Description,ProductTypeId,Price,ProductsSold,ManufacturerId,StockStatusId,Image")] Product product)
        {
            if (ModelState.IsValid)
            {

                #region File Upload


                //#region File Upload - CREATE
                ////Check to see if a file was uploaded
                //if (product.Image != null)
                //{
                //    //Check the file type 
                //    //- retrieve the extension of the uploaded file
                //    string ext = Path.GetExtension(product.Image.FileName);

                //    //- Create a list of valid extensions to check against
                //    string[] validExts = { ".jpeg", ".jpg", ".gif", ".png" };

                //    //- verify the uploaded file has an extension matching one of the extensions in the list above
                //    //- AND verify file size will work with our .NET app
                //    if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4_194_303)//underscores don't change the number, they just make it easier to read
                //    {
                //        //Generate a unique filename
                //        product.ProductImage = Guid.NewGuid() + ext;

                //        //Save the file to the web server (here, saving to wwwroot/images)
                //        //To access wwwroot, add a property to the controller for the _webHostEnvironment (see the top of this class for our example)
                //        //Retrieve the path to wwwroot
                //        string webRootPath = _webHostEnvironment.WebRootPath;
                //        //variable for the full image path --> this is where we will save the image
                //        string fullImagePath = webRootPath + "/images/";

                //        //Create a MemoryStream to read the image into the server memory
                //        using (var memoryStream = new MemoryStream())
                //        {
                //            await product.Image.CopyToAsync(memoryStream);//transfer file from the request to server memory
                //            using (var img = Image.FromStream(memoryStream))//add a using statement for the Image class (using System.Drawing)
                //            {
                //                //now, send the image to the ImageUtility for resizing and thumbnail creation
                //                //items needed for the ImageUtility.ResizeImage()
                //                //1) (int) maximum image size
                //                //2) (int) maximum thumbnail image size
                //                //3) (string) full path where the file will be saved
                //                //4) (Image) an image
                //                //5) (string) filename
                //                int maxImageSize = 500;//in pixels
                //                int maxThumbSize = 100;

                //                ImageUtility.ResizeImage(fullImagePath, product.Image, img, maxImageSize, maxThumbSize);
                //                //myFile.Save("path/to/folder", "filename"); - how to save something that's NOT an image

                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    //If no image was uploaded, assign a default filename
                //    //Will also need to download a default image and name it 'noimage.png' -> copy it to the /images folder
                //    product.Image = "noimage.png";
                //}

                //#endregion
                #endregion

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "ManufacturerName", product.ManufacturerId);
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName", product.ProductTypeId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "ManufacturerName", product.ManufacturerId);
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName", product.ProductTypeId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,Description,ProductTypeId,Price,ProductsSold,ManufacturerId,StockStatusId,Image")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                #region EDIT File Upload
                //retain old image file name so we can delete if a new file was uploaded
                //string oldImageName = product.Image;

                ////Check if the user uploaded a file
                //if (product.Image != null)
                //{
                //    //get the file's extension
                //    string ext = Path.GetExtension(product.Image.FileName);

                //    //list valid extensions
                //    string[] validExts = { ".jpeg", ".jpg", ".png", ".gif" };

                //    //check the file's extension against the list of valid extensions
                //    if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4_194_303)
                //    {
                //        //generate a unique file name
                //        product.ProductImage = Guid.NewGuid() + ext;
                //        //build our file path to save the image
                //        string webRootPath = _webHostEnvironment.WebRootPath;
                //        string fullPath = webRootPath + "/images/";

                //        //Delete the old image
                //        if (oldImageName != "noimage.png")
                //        {
                //            ImageUtility.Delete(fullPath, oldImageName);
                //        }

                //        //Save the new image to webroot
                //        using (var memoryStream = new MemoryStream())
                //        {
                //            await product.Image.CopyToAsync(memoryStream);
                //            using (var img = Image.FromStream(memoryStream))
                //            {
                //                int maxImageSize = 500;
                //                int maxThumbSize = 100;
                //                ImageUtility.ResizeImage(fullPath, product.Image, img, maxImageSize, maxThumbSize);
                //            }
                //        }

                //    }
                //}
                #endregion

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "ManufacturerName", product.ManufacturerId);
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName", product.ProductTypeId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Manufacturer)
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'StoreFrontContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
