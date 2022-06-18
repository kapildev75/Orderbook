namespace ICAP.Orderbook.Interfaces
{
    public interface IPrice
    {
        int PriceId { get; set; }

        string Description { get; set; }

        double Price { get; set; }
    }
}
