using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skelbimu_sistema.Models
{
    public class Wish
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string SearchKeyWords { get; set; } = string.Empty;

        public WishPaymentType PaymentMethod { get; set; }

        public double PriceHigh { get; set; } = 0.0;
        public double PriceLow { get; set; } = 0.0;

        public Category Category { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public User User { get; set; }
    }


    public enum WishPaymentType
    {
        All = 0,
        Cash = 1,
        Card = 2,
        Transaction = 3,
        Cash_And_Card = 4,
        Cash_And_Transaction = 5,
        Card_And_Transaction = 6   
    }
}
