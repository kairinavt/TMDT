
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Models;


[Table("InvoiceDetail")]
public class InvoiceDetail
{
  
    public int InvoiceId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public short Quantity { get; set; }
   
}
