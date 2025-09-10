using System.ComponentModel.DataAnnotations;

namespace ConvenientStore.Models;

public class Sale
{
    public int Id { get; set; }
    
    public DateTime SaleDate { get; set; } = DateTime.Now;
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
    public decimal TotalAmount { get; set; }
    
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;
    
    public PaymentMethod PaymentMethod { get; set; }
    
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}

public enum PaymentMethod
{
    Cash,
    CreditCard,
    DebitCard,
    MobilePayment
}