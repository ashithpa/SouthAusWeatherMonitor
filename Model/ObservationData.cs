namespace Model
{
    public class ObservationData
    {
        public string Name { get; set; }
        public string HistoryProduct { get; set; }
        public string LocalDateTime { get; set; }
        public string LocalDateTimeFull { get; set; }
        public string AifstimeUtc { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double ApparentT { get; set; }
        public double air_temp { get; set; }
        public double apparent_t { get; set; }
        public double dewpt { get; set; }

    }
}