namespace AHTAPI.Models
{
    public class AHT_StandsInformation
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? Distance { get; set; }
        public string? Ip {  get; set; }
        public int RollOn { get; set; }
        public int RollOff { get; set; }
        public int ReloadPage { get; set; }
        public string? En { get; set; }
        public string? Vn { get; set; }
        public string? Kr { get; set; }
        public string? Cn { get; set; }
        public string? ConnectionId { get; set; }
        public string? Handler { get; set; }
        public string? EnTitle { get; set; }
        public string? VnTitle { get; set; }
        public string? KrTitle { get; set; }
        public string? CnTitle { get; set; }
    }
}
