namespace AHTAPI.Models
{
    public class FlightForBelt
    {
        public string? LineCode { get; set; }
        public string? Number { get; set; }
        public string? City { get; set; }
        public DateTime Schedule { get; set; }
        public DateTime Estimated { get; set; }
        public DateTime Time {  get; set; }
        public string? Status { get; set; }
        public string? Remark { get; set; }
        public string? Belt {  get; set; }
        public string? Stand { get; set; }
        public string? Pvmd { get; set; }
    }
}
