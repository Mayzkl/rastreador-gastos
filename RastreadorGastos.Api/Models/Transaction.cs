using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RastreadorGastos.Api.Models;

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public string UserId { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    [ValidateNever]
    public Category Category { get; set; } = null!;
}