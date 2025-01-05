using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportEquipmentRent.Data;
using SportEquipmentRent.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SportEquipmentRent.Controllers
{
    [Authorize]

    public class RentingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public string? CurrentUserId
        {
            get
            {
                return User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
        }
        public RentingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Renting.Include(x => x.User).Include(x => x.SportEquipment).Where(u => u.UserId == CurrentUserId || User.IsInRole("Admin") ).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var renting = await _context.Renting
                .FirstOrDefaultAsync(m => m.Id == id);
            if (renting == null)
            {
                return NotFound();
            }
            if (renting.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier) && !User.IsInRole("Admin"))
            {
                throw new Exception();
            }
            return View(renting);
        }
        public IActionResult Create(int? eqId = null)
        {
            ViewData["SportEquipment"] = new SelectList(_context.SportEquipment, "Id", "Name");
            Renting session = new Renting { UserId = CurrentUserId };
            if (eqId != null)
            {
                session.SportEquipmentId = eqId.Value;
            }
            session.End = DateTime.UtcNow;
            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,SportEquipmentId,End")] Renting renting)
        {
            ViewData["SportEquipment"] = new SelectList(_context.SportEquipment, "Id", "Name");

            if (DateTime.Now.Date > renting.End.Date)
            {
                ModelState.AddModelError("End", "Data zakończenia wypożyczenia musi być późniejsza niż wczoraj");
            }
            renting.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            SportEquipment equipment = _context.SportEquipment.Include(s => s.Rentings).FirstOrDefault(x => x.Id == renting.SportEquipmentId);
            if(equipment.AvaibleUnits() == 0)
            {
                ModelState.AddModelError("SportEquipmentId", "Brak dostępnych sztuk");

            }
            if (ModelState.IsValid)
            {
                DateTime startDate = renting.Start;
                DateTime endDate = renting.End;
                renting.Start = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
                renting.End = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

                _context.Add(renting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(renting);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["SportEquipment"] = new SelectList(_context.SportEquipment.Include(x => x.Rentings).Where(r => r.AvaibleUnits() > 0), "Id", "Name");

            if (id == null)
            {
                return NotFound();
            }

            var renting = await _context.Renting.FindAsync(id);
            if (renting == null)
            {
                return NotFound();
            }
            if(renting.UserId !=  CurrentUserId)
            {
                throw new Exception();
            }
            return View(renting);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,SportEquipmentId,End")] Renting renting)
        {
            if (id != renting.Id)
            {
                return NotFound();
            }
            if (renting.UserId != CurrentUserId && !User.IsInRole("Admin") )
            {
                throw new Exception();
            }
            if (DateTime.Now.Date > renting.End.Date)
            {
                ModelState.AddModelError("End", "Data zakończenia wypożyczenia musi być późniejsza niż wczoraj");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(renting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentingExists(renting.Id))
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
            return View(renting);
        }

        // GET: Sessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var renting = await _context.Renting.Include(x => x.SportEquipment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (renting == null)
            {
                return NotFound();
            }
            if (renting.UserId != CurrentUserId && !User.IsInRole("Admin"))
            {
                throw new Exception();
            }
            return View(renting);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var renting = await _context.Renting.FindAsync(id);
            if (renting != null)
            {
                _context.Renting.Remove(renting);
            }
            if (renting.UserId != CurrentUserId && !User.IsInRole("Admin"))
            {
                throw new Exception();
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentingExists(int id)
        {
            return _context.Renting.Any(e => e.Id == id);
        }
    }
}
