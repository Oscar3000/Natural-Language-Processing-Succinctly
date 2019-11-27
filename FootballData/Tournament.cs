namespace FootballData
{
    /// <summary>
    /// Tournament
    /// </summary>
    public class Tournament
    {
        public string Name { get; set; }
        public int Year { get; set; }
        public string Gender { get; set; }
        public string FinalScore { get; set; }
        public Country Winner { get; set; }
        public Country RunnerUp { get; set; }

        public string Venue { get; set; }
        public string Attendance { get; set; }

        
    }
}
