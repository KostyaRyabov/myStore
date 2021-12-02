
namespace myStore.entities
{
    public class Producer : Accessored<Producer>
    {
        public int producer_id { get; set; }
        public string name { get; set; }
    }
}
