using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamsGeneralStore.DATA.EF.Models;

namespace SamsGeneralStore.UI.MVC.Controllers
{
    public class StockStatusController : Controller
    {
        private readonly StoreFrontContext _context;

        public StockStatusController(StoreFrontContext context)
        {
            _context = context;
        }

        // GET: StockStatus
        public async Task<IActionResult> Index()
        {
              return _context.StockStatuses != null ? 
                          View(await _context.StockStatuses.ToListAsync()) :
                          Problem("Entity set 'StoreFrontContext.StockStatuses'  is null.");
        }

        // GET: StockStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StockStatuses == null)
            {
                return NotFound();
            }

            var stockStatus = await _context.StockStatuses
                .FirstOrDefaultAsync(m => m.StockStatusId == id);
            if (stockStatus == null)
            {
                return NotFound();
            }

            return View(stockStatus);
        }

        // GET: StockStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StockStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StockStatusId,StockStatusName")] StockStatus stockStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stockStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stockStatus);
        }

        // GET: StockStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StockStatuses == null)
            {
                return NotFound();
            }

            var stockStatus = await _context.StockStatuses.FindAsync(id);
            if (stockStatus == null)
            {
                return NotFound();
            }
            return View(stockStatus);
        }

        // POST: StockStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StockStatusId,StockStatusName")] StockStatus stockStatus)
        {
            if (id != stockStatus.StockStatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockStatusExists(stockStatus.StockStatusId))
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
            return View(stockStatus);
        }

        // GET: StockStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StockStatuses == null)
            {
                return NotFound();
            }

            var stockStatus = await _context.StockStatuses
                .FirstOrDefaultAsync(m => m.StockStatusId == id);
            if (stockStatus == null)
            {
                return NotFound();
            }

            return View(stockStatus);
        }

        // POST: StockStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StockStatuses == null)
            {
                return Problem("Entity set 'StoreFrontContext.StockStatuses'  is null.");
            }
            var stockStatus = await _context.StockStatuses.FindAsync(id);
            if (stockStatus != null)
            {
                _context.StockStatuses.Remove(stockStatus);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockStatusExists(int id)
        {
          return (_context.StockStatuses?.Any(e => e.StockStatusId == id)).GetValueOrDefault();
        }
    }
}
