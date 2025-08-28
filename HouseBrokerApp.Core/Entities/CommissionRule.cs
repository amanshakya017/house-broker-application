namespace HouseBrokerApp.Core.Entities
{
    public class CommissionRule : BaseEntity<Guid>
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal Rate { get; set; } 
    }
}
