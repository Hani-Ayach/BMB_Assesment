namespace BMBAssesmentOT.Web
{
    public class Order
    {
        public int? Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid ClientId { get; set; }
        public Guid? ClientName { get; set; }

    }
}
