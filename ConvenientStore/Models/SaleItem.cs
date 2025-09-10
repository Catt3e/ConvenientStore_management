using System.ComponentModel.DataAnnotations;

namespace ConvenientStore.Models;

public class SaleItem
{
    public int Id { get; set; }
    
    public int SaleId { get; set; }
    public Sale? Sale { get; set; }
    
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public decimal UnitPrice { get; set; }
    
    public decimal TotalPrice => Quantity * UnitPrice;
}