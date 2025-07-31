public class ReceiptsController : Controller
{
    private readonly AppDbContext _context;

    public ReceiptsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var receipts = await _context.Receipts
            .Include(r => r.Items)
                .ThenInclude(i => i.Resource)
            .Include(r => r.Items)
                .ThenInclude(i => i.Unit)
            .ToListAsync();

        return View(receipts);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Resources = await _context.Resources.Where(r => !r.IsArchived).ToListAsync();
        ViewBag.Units = await _context.Units.Where(u => !u.IsArchived).ToListAsync();
        return View(new Receipt());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Receipt receipt)
    {
        if (await _context.Receipts.AnyAsync(r => r.Number == receipt.Number))
        {
            ModelState.AddModelError("Number", "ƒокумент с таким номером уже существует.");
            return View(receipt);
        }

        foreach (var item in receipt.Items)
        {
            var existing = await _context.StockBalances
                .FirstOrDefaultAsync(b => b.ResourceId == item.ResourceId && b.UnitId == item.UnitId);

            if (existing != null)
                existing.Quantity += item.Quantity;
            else
                _context.StockBalances.Add(new StockBalance
                {
                    ResourceId = item.ResourceId,
                    UnitId = item.UnitId,
                    Quantity = item.Quantity
                });
        }

        _context.Receipts.Add(receipt);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var receipt = await _context.Receipts
            .Include(r => r.Items)
                .ThenInclude(i => i.Resource)
            .Include(r => r.Items)
                .ThenInclude(i => i.Unit)
            .FirstOrDefaultAsync(r => r.Id == id);

        return receipt == null ? NotFound() : View(receipt);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var receipt = await _context.Receipts
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receipt == null)
            return NotFound();

        foreach (var item in receipt.Items)
        {
            var balance = await _context.StockBalances
                .FirstOrDefaultAsync(b => b.ResourceId == item.ResourceId && b.UnitId == item.UnitId);

            if (balance == null || balance.Quantity < item.Quantity)
            {
                TempData["Error"] = "Ќевозможно удалить Ч недостаточно остатков.";
                return RedirectToAction(nameof(Index));
            }

            balance.Quantity -= item.Quantity;
        }

        _context.Receipts.Remove(receipt);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}