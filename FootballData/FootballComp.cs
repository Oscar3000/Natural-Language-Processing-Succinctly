using NaturalLanguageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FootballData
{
    public static class FootballComp
    {
        private static List<Tournament> FootballResults;
        private static Random rnd;
        private static string lastVerb;
        private static string Gender;
        private static int TournamentYear;
        private static string Tournament;
        private static string Country;

        static FootballComp()
        {
            FootballResults = new List<Tournament>();
            LoadDataSet();
            rnd = new Random();
            lastVerb = "WON";
            Gender = "B";
        }

        public static string GetResponse(List<string> Words_, List<string> Tags_)
        {
            string ans_ = "";
            string GuessYear = "LAST";
            string curWord = "";
            for(int x =0; x < Tags_.Count; x++)
            {
                curWord = Words_[x].Trim().ToLower();
                if(Tags_[x] == "YEAR")
                {
                    TournamentYear = Convert.ToInt16(Words_[x]);
                    GuessYear = "";
                }
                if(Tags_[x] == "EVENT")
                {
                    Tournament = Words_[x];
                    if (Tournament.Contains("WORLD")) Tournament = "WORLDCUP";
                    if (Tournament.Contains("EURO")) Tournament = "UEFAEUROSCUP";
                }
                if (Tags_[x].StartsWith("VB"))
                {
                    lastVerb = Words_[x].ToUpper();
                }
                if(Tags_[x] == "COUNTRY")
                {
                    Country = Words_[x].ToUpper();
                }
                if ("|first|earliest|".IndexOf(curWord) >= 0)
                {
                    GuessYear = "FIRST";
                }
                if (Tags_[x] == "RB" && curWord == "most")
                {
                    lastVerb = "MOST:" + lastVerb;
                }
            }
            if (GuessYear == "FIRST")
            {
                TournamentYear = 0;
            }
            if (GuessYear == "LAST")
            {
                TournamentYear = 9999;
            }

            //Answer
            ans_ = Answer(Words_,Tags_);
            return ans_.Length < 1 ? GetRandomReply() : ans_;
        }

        private static string Answer(List<string> Words_, List<string> Tags_)
        {
            string answer = ""; 
            //Simple Questions (who won or who lost)
            if (lastVerb == "WON")
            {
                answer = WhoWon(Tournament, TournamentYear, "M");

            }
            if (lastVerb == "LOST")
            {
                answer = WhoLost(Tournament, TournamentYear, "M");
            }

            if(lastVerb == "HOSTED" || lastVerb == "PLAYED")
            {
                answer = WhoHosted(Tournament, TournamentYear, "M");
            }

            if (lastVerb.StartsWith("MOST"))
            {
                string[] keys_ = lastVerb.Split(':');
                if (keys_.Length == 1)
                {
                    lastVerb = "WON";
                }
                else
                {
                    lastVerb = keys_[1];
                }
                if (lastVerb == "WON")
                {
                    answer = MostWins(Tournament, "M");
                }
                if (lastVerb == "LOST")
                {
                    answer = MostLosses(Tournament, "M");
                }

            }

            return answer;
        }

        public static string WhoHosted(string Tournament, int Year, string Gender)
        {
            string[] PossibleReplies = {
                      "The Competition {0} was played in {1}",
                      "The {0} was hosted in {1}"
                    };
            string ans_ = "";
            Tournament Results_ = GetResults(Tournament, Year, Gender);

            if (Results_ != null)
            {
                string GenderText = "men";
                if (Gender == "F") { GenderText = "women"; }
                int reply = rnd.Next(1, PossibleReplies.Length);

                ans_ = string.Format(PossibleReplies[reply],
                       Results_.Name,
                       Results_.Host.FullName);
            }
            return ans_;
        } 

        public static string WhoWon(string Tournament, int Year, string Gender)
        {
            string[] PossibleReplies = {
                      "{0} was the {1}'s winner",
                      "{0} won the {1}'s",
                      "{0} won on the {1}'s side",
                      "{0} defeated {2} in the {1}'s draw",
                      "{0} won in {3} sets over {2}",
                      "{0} won in a {3} score over {2} with {5} people in attendance at {4}."
                    };
            string ans_ = "";
            Tournament Results_ = GetResults(Tournament, Year, Gender);
            if (Results_ != null)
            {
                string GenderText = "men";
                if (Gender == "F") { GenderText = "women"; }
                int reply = rnd.Next(1, PossibleReplies.Length) - 1;
                
                reply = 5;


                ans_ = string.Format(PossibleReplies[reply],
                       Results_.Winner.FullName,
                       GenderText,
                       Results_.RunnerUp.FullName,
                       Results_.FinalScore,
                       Results_.Venue,
                       Results_.Attendance);
            }
            return ans_;
        }

        public static string WhoLost(string Tournament, int Year, string Gender)
        {
            string[] PossibleReplies = {
                      "{1} lost the {0} in {2}.",
                      "{1} lost in {2} to {3}",
                      "{1} lost in the final {4}",
                      "{0} lost {1} to {2} in {3}.",
                      "{0} was victorious over {1} with a scoreline of {2}."
                    };
            string ans_ = "";
            Tournament Results_ = GetResults(Tournament, Year, Gender);
            if (Results_ != null)
            {
                string GenderText = "men";
                if (Gender == "F") { GenderText = "women"; }

                int reply = rnd.Next(1, PossibleReplies.Length) - 1;
                if (reply == 4)
                {
                    ans_ = string.Format(PossibleReplies[reply], Results_.Winner.FullName,
                            Results_.RunnerUp.FullName, Results_.FinalScore);
                }
                else if(reply == 3)
                {
                    ans_ = string.Format(PossibleReplies[reply], Results_.RunnerUp.FullName, 
                        Results_.FinalScore, Results_.Winner.FullName,Results_.Venue);
                }else
                {
                    ans_ = string.Format(PossibleReplies[reply], Tournament,
                        Results_.RunnerUp.FullName,Results_.Venue,Results_.Winner.FullName,Results_.FinalScore);
                }
            }
            return ans_;
        }

        private static string GetRandomReply()
        {
            string[] PossibleReplies = {
                      "I don't know",
                      "Could you be more specific with your question?",
                      //"can you tell me the year?",
                      "sorry I have no clue right now",
                      "sorry, I don't understand. could you repeat the questions with more clarity?"
                    };

            int reply = rnd.Next(1, PossibleReplies.Length) - 1;

            return PossibleReplies[reply];
        }

        public static string FinalScore(string Tournament, int Year, string Gender)
        {
            string ans_ = "";
            Tournament Results_ = GetResults(Tournament, Year, Gender);
            if (Results_ != null)
            {
                ans_ = Results_.FinalScore;
            }
            return ans_;
        }

        static Tournament GetResults(string Tournament, int Year, string Gender)
        {
            try
            {
                if (Year < 1)
                {
                    Tournament FirstOne = FootballResults.Where(x => x.Name.ToLower() == Tournament.ToLower()).OrderBy(x => x.Year).First();
                    Year = FirstOne.Year;
                }
                if (Year >= 9999)
                {
                    Tournament LastOne = FootballResults.Where(x => x.Name.ToLower() == Tournament.ToLower()).OrderBy(x => x.Year).Last();
                    Year = LastOne.Year;
                }


                Tournament Fnd = FootballResults.FirstOrDefault(x => x.Year == Year && x.Name == Tournament && x.Gender == Gender);
                return Fnd;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        static public string MostWins(string Tournament, string Gender)
        {
            string Winningest = "";
            try
            {
                var ans_ = FootballResults.Where(x => x.Name.ToLower() == Tournament.ToLower() && x.Gender == Gender).
                          GroupBy(x => x.Winner.FullName, (key, values) => new { Country = key, Count = values.Count() }).OrderByDescending(y => y.Count);
                var MostWins = ans_.FirstOrDefault();
                if (MostWins != null)
                {
                    Winningest = MostWins.Country + " has won the " + Tournament + " " + MostWins.Count + " times";
                }
            }
            catch(Exception ex)
            {
                Winningest = "";
            }
            
            return Winningest;
        }
        static public string MostLosses(string Tournament, string Gender)
        {
            string Losses = "";
            try
            {
                var ans_ = FootballResults.Where(x => x.Name.ToLower() == Tournament.ToLower() && x.Gender == Gender).
                          GroupBy(x => x.RunnerUp.FullName, (key, values) => new { Country = key, Count = values.Count() }).OrderByDescending(y => y.Count);
                var MostLosses = ans_.FirstOrDefault();
                if (MostLosses != null)
                {
                    Losses = MostLosses.Country + " has lost the most. " + MostLosses.Count + " times to be exact.";
                }
            }
            catch (Exception ex)
            {
                Losses = "";
            }
            
            return Losses;
        }

        public static void LoadDataSet()
        {
            if (FootballResults.Count >= 1) return;

            FootballResults = new List<Tournament>();
            List<string> Countries = new List<string>();
            string[] lines = System.IO.File.ReadAllLines(@"../../../FootballData/Football.txt");
            for(int i=0; i < lines.Length; i++)
            {
                string[] currentData = lines[i].Split('|');
                Tournament currentTournament = new Tournament
                {
                    Name = currentData[0].ToUpper().Trim(),
                    Gender = currentData[1].ToUpper().Trim(),
                    Year = Convert.ToInt16(currentData[2]),
                    FinalScore = FinalScore(currentData),
                    Winner = new Country(),
                    RunnerUp = new Country(),
                    Host = new Country(),
                    Venue = currentData[7].ToUpper().Trim(),
                    Attendance = currentData[8].ToUpper().Trim()
                };

                currentTournament.Winner.FullName = currentData[3].ToUpper().Trim();
                currentTournament.RunnerUp.FullName = currentData[5].ToUpper().Trim();
                currentTournament.Host.FullName = currentData[7].ToUpper().Split(',')[1].Trim();

                FootballResults.Add(currentTournament);
                Countries.Add(currentTournament.Winner.FullName);
                Countries.Add(currentTournament.RunnerUp.FullName);
            }

            var uniqueCountries = Countries.Distinct();
            foreach(var country in uniqueCountries)
            {
                Entities.NamedEntities.Add(country, "COUNTRY");
            }
        }

        private static string FinalScore(string[] data)
        {
            if (data[9].ToUpper().Trim() != "*")
                return $"{data[4].ToUpper().Trim()}-{data[6].ToUpper().Trim()} after 120 mins. Penalty scores were {data[9].ToUpper().Trim()}";
            else
                return $"{data[4].ToUpper().Trim()}-{data[6].ToUpper().Trim()}";
        }
    }
}
