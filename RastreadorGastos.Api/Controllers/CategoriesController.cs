using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RastreadorGastos.Api.Data;
using RastreadorGastos.Api.Models;

namespace RastreadorGastos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        var userId = GetUserId();
        return await _context.Categories.Where(c => c.UserId == userId).ToListAsync();
    }

    // GET: api/categories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var userId = GetUserId();
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
        {
            return NotFound();
        }

        return category;
    }

    // POST: api/categories
    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(Category category)
    {
        category.UserId = GetUserId();
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    // PUT: api/categories/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, Category category)
    {
        var userId = GetUserId();
        var existing = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (existing == null)
        {
            return NotFound();
        }

        existing.Name = category.Name;
        existing.Type = category.Type;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/categories/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var userId = GetUserId();
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}