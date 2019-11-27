namespace FootballData
{
    /// <summary>
    /// Player 
    /// </summary>
    public class Country
    {
        public string FullName { get; set; }

        public string FirstName
        {
            get
            {
                string[] names = FullName.Split(' ');
                return names[0];
            }
        }
        public string LastName
        {
            get
            {
                string[] names = FullName.Split(' ');
                return names[names.Length - 1];
            }
        }

    }
}
