using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skelbimu_sistema.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ProductState State { get; set; }

        public ProductPaymentType PaymentType { get; set; }

        public double Price { get; set; } = 0.0;

        public string ImageUrl { get; set; } = string.Empty;

        public string StartDate { get; set; } // šito nėra klasių diagramose

        public string EndDate { get; set; } // šito nėra klasių diagramose

        public Category Category { get; set; }

        [ForeignKey("UserId")]
        public User Seller { get; set; }
    }

    public enum ProductState
    {
        Active = 0,
        Reserved = 1,
        Suspended = 2,
        Closed = 3
    }

    public enum ProductPaymentType
    {
        Cash = 0,
        Card = 1,
        Transaction = 2
    }
}
