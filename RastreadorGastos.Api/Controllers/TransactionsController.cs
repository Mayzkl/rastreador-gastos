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
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransactionsController(AppDbContext context)
    {
        _context = context;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    // GET: api/transactions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
    {
        var userId = GetUserId();
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    // GET: api/transactions/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetTransaction(int id)
    {
        var userId = GetUserId();
        var transaction = await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (transaction == null)
        {
            return NotFound();
        }

        return transaction;
    }

    // POST: api/transactions
    [HttpPost]
    public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction)
    {
        var userId = GetUserId();

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == transaction.CategoryId && c.UserId == userId);
        if (!categoryExists)
        {
            return BadRequest("La categoría especificada no existe o no te pertenece.");
        }

        transaction.UserId = userId;
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
    }

    // PUT: api/transactions/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(int id, Transaction transaction)
    {
        var userId = GetUserId();
        var existing = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (existing == null)
        {
            return NotFound();
        }

        existing.Amount = transaction.Amount;
        existing.Date = transaction.Date;
        existing.Description = transaction.Description;
        existing.CategoryId = transaction.CategoryId;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/transactions/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var userId = GetUserId();
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (transaction == null)
        {
            return NotFound();
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}