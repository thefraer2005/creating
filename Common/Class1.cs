using System.Text.Json.Serialization;

namespace Common
{
    public enum CardType
    {
        Number,
        Skip,
        Reverse,
        DrawTwo,
        Wild,
        WildDrawFour
    }

    public class GameStartInfo
    {

        public Dictionary<string, List<Card>> PlayerHands { get; set; }
        public Card FirstCard { get; set; }
        public string FirstPlayerNickname { get; set; }
        public GameStartInfo()
        {
            PlayerHands = new Dictionary<string, List<Card>>();
            FirstCard = new Card();
            FirstPlayerNickname = string.Empty;
        }
    }


    public class Card
    {
        public int Id { get; set; } 
        public string Color { get; set; }
        public string Value { get; set; }
        public CardType Type { get; set; }

        [JsonConstructor]
        public Card(int id=1, string color = null, string value = null, CardType? type = null)
        {
            Id = id; 

            if (type.HasValue)
            {
                Type = type.Value;

                if (Type == CardType.Wild || Type == CardType.WildDrawFour)
                {
                    Color = color ;
                    Value = value ?? Type.ToString(); 
                }
                else
                {
                    Color = color; 
                    Value = value ?? Type.ToString();
                }
            }
            else
            {
                Type = CardType.Number;
                Color = color;
                Value = value;
            }
        }
        public override bool Equals(object obj)
        {
            if (obj is Card otherCard)
            {
                return this.Id == otherCard.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode(); 
        }
        public override string ToString()
        {
            return Type == CardType.Number ? $"{Color} {Value}" : $"{Color} {Value} ({Type})";
        }
    }


    public static class Protocol
    {
        public const string Connect = "CONNECT";
        public const string Disconnect = "DISCONNECT";
        public const string StartGame = "START_GAME";
        public const string GiveCard = "GIVE_CARD";
        public const string PlayCard = "PLAY_CARD";
        public const string WildCard = "WILD_CARD";
        public const string UNO= "UNO";
        public const string Round= "Round";
        public const string NewRound= "New_Round";
        public const string Victory= "VICTORY";
        public const string UpdateGameState = "UPDATE_GAME_STATE";
    }
}
