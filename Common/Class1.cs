using System.Text.Json.Serialization;

namespace Common
{
    public enum CardType : byte // Используем byte для экономии памяти
    {
        Number = 0,
        Skip = 1,
        Reverse = 2,
        DrawTwo = 3,
        Wild = 4,
        WildDrawFour = 5
    }

    public enum CardColor : byte // Используем byte для экономии памяти
    {
        Red = 0,
        Green = 1,
        Blue = 2,
        Yellow = 3,
        
    }

    public enum CardValue : byte // Используем byte для экономии памяти
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Skip = 10,
        Reverse = 11,
        DrawTwo = 12,
        Wild = 13,
        WildDrawFour = 14
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
        public CardColor Color { get; set; } // Изменено на CardColor
        public CardValue Value { get; set; } // Изменено на CardValue
        public CardType Type { get; set; }

        [JsonConstructor]
        public Card(int id = 1, CardColor color = CardColor.Red, CardValue value = CardValue.Zero, CardType? type = null)
        {
            Id = id;

            if (type.HasValue)
            {
                Type = type.Value;

                if (Type == CardType.Wild || Type == CardType.WildDrawFour)
                {
                    Color = color;
                    Value = value; // Для диких карт значение может быть не определено
                }
                else
                {
                    Color = color;
                    Value = value;
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
