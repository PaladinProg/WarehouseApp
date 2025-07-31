using System.ComponentModel.DataAnnotations;

namespace WarehouseAppFull.Models;

public class Receipt
{
	public int Id { get; set; }

	[Required]
	public string Number { get; set; } = null!;

	public DateTime Date { get; set; } = DateTime.Now;

	public List<ReceiptItem> Items { get; set; } = new();
}