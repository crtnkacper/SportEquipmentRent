using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportEquipmentRent.Data;
using SportEquipmentRent.Models;
using Microsoft.AspNetCore.Authorization;

namespace SportEquipmentRent.Controllers
{
    public class SportEquipmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SportEquipmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.SportEquipment.Include(x => x.Rentings).ToListAsync());
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var SportEquipment = await _context.SportEquipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (SportEquipment == null)
            {
                return NotFound();
            }

            return View(SportEquipment);
        }
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Units,Name")] SportEquipment SportEquipment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(SportEquipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(SportEquipment);
        }

        [Authorize(Roles = "Admin")]

        // GET: SportEquipments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var SportEquipment = await _context.SportEquipment.FindAsync(id);
            if (SportEquipment == null)
            {
                return NotFound();
            }
            return View(SportEquipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Units,Name")] SportEquipment SportEquipment)
        {
            if (id != SportEquipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(SportEquipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SportEquipmentExists(SportEquipment.Id))
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
            return View(SportEquipment);
        }


        // GET: SportEquipments/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var SportEquipment = await _context.SportEquipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (SportEquipment == null)
            {
                return NotFound();
            }

            return View(SportEquipment);
        }

        // POST: SportEquipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var SportEquipment = await _context.SportEquipment.FindAsync(id);
            if (SportEquipment != null)
            {
                _context.SportEquipment.Remove(SportEquipment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SportEquipmentExists(int id)
        {
            return _context.SportEquipment.Any(e => e.Id == id);
        }
    }
}
