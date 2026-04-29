namespace Entities;

public class Wallet
{
    public int Id {get; set;}
    public List<WalletStock> Stocks { get; set; } = new();
}