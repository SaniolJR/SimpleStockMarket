using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class WalletDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "WalletID must by possitive number (ID > 0).")]
    public int walletID { get; set; }
}