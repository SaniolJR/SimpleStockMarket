using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class StockAndWalletDTO
{
    [Required(ErrorMessage = "Stock name is required.")]
    [RegularExpression(@"^.*\S.*$", ErrorMessage = "Stock name cannot contains only whitespaces.")]
    public string stockName { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "WalletID must by possitive number (ID > 0).")]
    public int walletID { get; set; }
}