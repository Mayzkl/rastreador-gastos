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
public class SummaryController : ControllerBase
{
    private readonly AppDbContext _context;

    public SummaryController(AppDbContext context)
    {
        _context = context;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    // GET: api/summary?month=9&year=2026
    [HttpGet]
    public async Task<ActionResult> GetSummary([FromQuery] int? month, [FromQuery] int? year)
    {
        var userId = GetUserId();

        var query = _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId);

        if (month.HasValue && year.HasValue)
        {
            query = query.Where(t => t.Date.Month == month.Value && t.Date.Year == year.Value);
        }

        var transactions = await query.ToListAsync();

        var totalIncome = transactions
            .Where(t => t.Category.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var totalExpense = transactions
            .Where(t => t.Category.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        var byCategory = transactions
            .GroupBy(t => t.Category.Name)
            .Select(g => new
            {
                Category = g.Key,
                Total = g.Sum(t => t.Amount)
            })
            .OrderByDescending(x => x.Total);

        return Ok(new
        {
            totalIncome,
            totalExpense,
            balance = totalIncome - totalExpense,
            byCategory
        });
    }
}