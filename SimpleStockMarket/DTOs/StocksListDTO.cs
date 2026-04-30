using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class StocksListDTO
{
    [Required] 
    public List<StockItemDTO> Stocks { get; set; } = new();
}

public class StockItemDTO
{
    [Required(ErrorMessage = "Stock name needed")]
    public required string Name { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative!")]
    public int Quantity { get; set; }
}