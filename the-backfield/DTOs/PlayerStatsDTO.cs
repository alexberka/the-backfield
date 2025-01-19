using TheBackfield.Models;

namespace TheBackfield.DTOs
{
    public class PlayerStatsDTO
    {
        public int PassAttempts { get; set; }
        public int PassCompletions { get; set; }
        public int PassYards { get; set; }
        public int PassTouchdowns { get; set; }
        public int InterceptionsThrown { get; set; }
        public int Receptions { get; set; }
        public int ReceivingTargets { get; set; }
        public int ReceivingYards { get; set; }
        public int ReceivingTouchdowns { get; set; }
        public int RushAttempts { get; set; }
        public int RushYards { get; set; }
        public int RushTouchdowns { get; set; }
        public int FumblesCommitted { get; set; }
        public int Tackles { get; set; }
        public int SoloTackles { get; set; }
        public int TacklesForLoss { get; set; }
        public int Sacks { get; set; }
        public int InterceptionsReceived { get; set; }
        public int InterceptionReturnYards { get; set; }
        public int InterceptionReturnTouchdowns { get; set; }
        public int FumblesForced { get; set; }
        public int FumblesRecovered { get; set; }
        public int FumbleReturnYards { get; set; }
        public int FumbleReturnTouchdowns { get; set; }
        public int FieldGoalAttempts { get; set; }
        public int FieldGoalsMade { get; set; }
        public int ExtraPointAttempts { get; set; }
        public int ExtraPointsMade { get; set; }
        public int Punts { get; set; }
        public int PuntYards { get; set; }
        public decimal AveragePuntYards
        {
            get
            {
                if (Punts == 0)
                {
                    return 0;
                }
                return PuntYards / Punts;
            }
        }
        public int Kickoffs { get; set; }
        public int KickoffsForTouchbacks { get; set; }
        public int KickoffReturns { get; set; }
        public int KickoffReturnYards { get; set; }
        public int KickoffReturnTouchdowns { get; set; }
        public int PuntReturns { get; set; }
        public int PuntReturnYards { get; set; }
        public int PuntReturnTouchdowns { get; set; }
    }
}
