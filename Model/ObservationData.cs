namespace Model
{
    public class ObservationData
    {
        public string Name { get; set; }
        public string weather { get; set; }
        
        public string local_date_time { get; set; }
        public string cloud { get; set; }
        public string aifstime_utc { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double apparent_t { get; set; }
        public double air_temp { get; set; }
        public double dewpt { get; set; }

    }
}