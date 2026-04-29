namespace Entities;
public class WalletStock
{
    public int Id { get; set; }
    public int WalletId { get; set; }
    public int StockId { get; set; }
    public int Quantity { get; set; }
    
    public required Wallet Wallet { get; set; }
    public required Stock Stock { get; set; }
}