namespace Entities;

public class AuditLog
{
    public int Id {get; set;}
    public TransactionType Type {get; set;}
    public required Wallet UsedWallet {get; set;}
    public required Stock UsedStock {get; set;}
    public DateTime TransactionDate {get; set;}
}