using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseAppFull.Data;
using WarehouseAppFull.Models;

namespace WarehouseAppFull.Controllers;

public class UnitsController : Controller
{
    private readonly AppDbContext _context;

    public UnitsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(bool includeArchived = false)
    {
        var units = await _context.Units
            .Where(u => includeArchived || !u.IsArchived)
            .ToListAsync();

        return View(units);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Unit unit)
    {
        if (ModelState.IsValid)
        {
            if (await _context.Units.AnyAsync(u => u.Name == unit.Name))
            {
                ModelState.AddModelError("Name", "Единица измерения с таким наименованием уже существует.");
                return View(unit);
            }

            _context.Units.Add(unit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(unit);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var unit = await _context.Units.FindAsync(id);
        return unit == null ? NotFound() : View(unit);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Unit unit)
    {
        if (ModelState.IsValid)
        {
            _context.Units.Update(unit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(unit);
    }

    public async Task<IActionResult> Archive(int id)
    {
        var unit = await _context.Units.FindAsync(id);
        if (unit == null) return NotFound();

        unit.IsArchived = true;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}