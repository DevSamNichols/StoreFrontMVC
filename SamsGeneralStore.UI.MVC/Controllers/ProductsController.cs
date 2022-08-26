using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamsGeneralStore.DATA.EF.Models;

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
        public async Task<IActionResult> Index()
        {
            var storeFrontContext = _context.Products.Include(p => p.Manufacturer).Include(p => p.ProductType).Include(p => p.StockStatus);
            return View(await storeFrontContext.ToListAsync());
        }


        //Created a separate action that returns the same results as Index, but in the View
        //we will use a tiled layout instead of a table
        public async Task<IActionResult> TiledProducts(string searchTerm, int productTypeID = 0, int page = 1)
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

            if (productTypeID != 0)
            {
                products = products.Where(p => p.ProductTypeId == productTypeID).ToList();

                //Repopuplate the dropdown with the current category that is selected
                ViewData["ProductTypeID"] = new SelectList(_context.ProductTypes, "ProductTypeID", "ProductTypeName", productTypeID);

                //Update the ViewBag variable
                ViewBag.Category = productTypeID;
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
                .Include(p => p.StockStatus)
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
            ViewData["StockStatusId"] = new SelectList(_context.StockStatuses, "StockStatusId", "StockStatusName");
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
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "ManufacturerName", product.ManufacturerId);
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName", product.ProductTypeId);
            ViewData["StockStatusId"] = new SelectList(_context.StockStatuses, "StockStatusId", "StockStatusName", product.StockStatusId);
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
            ViewData["StockStatusId"] = new SelectList(_context.StockStatuses, "StockStatusId", "StockStatusName", product.StockStatusId);
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
            ViewData["StockStatusId"] = new SelectList(_context.StockStatuses, "StockStatusId", "StockStatusName", product.StockStatusId);
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
                .Include(p => p.StockStatus)
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
