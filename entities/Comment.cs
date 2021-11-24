namespace myStore.entities
{
    public class Comment
    {
        public int comment_id { get; set; }
        public int notebook_id { get; set; }

        public string plus_text { get; set; }
        public string minus_text { get; set; }
        public string review_text { get; set; }

        public short screen_rate { get; set; }
        public short power_rate { get; set; }
        public short work_duration_rate { get; set; }
    }
}
