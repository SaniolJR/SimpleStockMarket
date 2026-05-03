namespace DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class TransactionRequestDTO
    {
        [Required(ErrorMessage = "Type of transaction is required.")]
        [RegularExpression("^(buy|sell)$", ErrorMessage = "Type must be sell or buy")]
        public string Type { get; set; }
    }
}