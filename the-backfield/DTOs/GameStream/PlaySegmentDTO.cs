namespace TheBackfield.DTOs.GameStream
{
    public class PlaySegmentDTO
    {
        public int Index { get; set; }
        public int? FieldStart { get; set; }
        public int? FieldEnd { get; set; }
        public int TeamId { get; set; }
        public string SegmentText { get; set; } = "";
    }
}
