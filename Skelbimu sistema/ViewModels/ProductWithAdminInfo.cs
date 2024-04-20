namespace Skelbimu_sistema.ViewModels
{
    public class ProductWithAdminInfo
    {
        public Product Product { get; set; }
        public List<Report> Reports { get; set; }
        public Suspension? Suspension { get; set; }
    }
}
