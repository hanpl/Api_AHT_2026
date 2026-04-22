namespace AHTAPI.Models
{
    public class FlightLinks
    {
        public string Aircraft { get; set; }
        public string ACType { get; set; }
        public string Transit24_ARR { get; set; }
        public string ArrFlight { get; set; }
        public string? ArrConfig { get; set; }
        public string ArrCallsign { get; set; }
        public DateTime? STA { get; set; }
        public string ArrType { get; set; }
        public string From { get; set; }
        public string Qual_ARR { get; set; }
        public string Status_ARR { get; set; }
        public string Remark_ARR { get; set; }
        public DateTime? MCAT { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? ChocksOn { get; set; }
        public int? EstDeliveryTime { get; set; }   // reuses Totalpax field
        public string? ArrPax { get; set; }
        public string Carousel { get; set; }
        public string ARR_Comments { get; set; }
        public string ArrStand { get; set; }
        public string ArrCodeShares { get; set; }

        // Departure fields
        public int? DepartureId { get; set; }
        public string ACDepType { get; set; }
        public string Transit24_DEP { get; set; }
        public string DepFlight { get; set; }
        public string? DepConfig { get; set; }
        public string DepCallsign { get; set; }
        public DateTime? STD { get; set; }
        public string DepType { get; set; }
        public string To { get; set; }
        public string Qual_DEP { get; set; }
        public string Status_DEP { get; set; }
        public string Remark_DEP { get; set; }
        public DateTime? MCDT { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ChocksOff { get; set; }
        public DateTime? ATD { get; set; }
        public string? DepPax { get; set; }
        public string Gate { get; set; }
        public string Counters { get; set; }
        public string DEP_Comments { get; set; }
        public string DepStand { get; set; }
        public string DepCodeShares { get; set; }
        public string Chutes { get; set; }
    }
}
