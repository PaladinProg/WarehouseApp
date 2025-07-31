public class ResourcesController : Controller
{
    private readonly AppDbContext _context;

    public ResourcesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(bool includeArchived = false)
    {
        var resources = await _context.Resources
            .Where(r => includeArchived || !r.IsArchived)
            .ToListAsync();
        return View(resources);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Resource resource)
    {
        if (ModelState.IsValid)
        {
            if (await _context.Resources.AnyAsync(r => r.Name == resource.Name))
            {
                ModelState.AddModelError("Name", "Ресурс с таким наименованием уже существует.");
                return View(resource);
            }

            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(resource);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var resource = await _context.Resources.FindAsync(id);
        return resource == null ? NotFound() : View(resource);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Resource resource)
    {
        if (ModelState.IsValid)
        {
            _context.Resources.Update(resource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(resource);
    }

    public async Task<IActionResult> Archive(int id)
    {
        var resource = await _context.Resources.FindAsync(id);
        if (resource == null) return NotFound();

        resource.IsArchived = true;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}