namespace CoTuong.Models
{
    public class MoveChess
    {
        public string id { get; set; }
        public int fromi { get; set; }
        public int fromj { get; set; }
        public int toi { get; set; }
        public int toj { get; set; }

        public int top { get; set; }
        public int left { get; set; }

        public Guid roomId { get; set; }
        public int turn { get; set; }
    }
}
