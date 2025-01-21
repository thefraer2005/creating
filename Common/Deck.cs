using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class WinnerInfo
    {
        public string WinnerNickname { get; set; }
        public int TotalPoints { get; set; }
    }
    public class Deck
    {
        public List<Card> Cards { get; private set; }

        public Deck()
        {
            Cards = new List<Card>();
            InitializeDeck();
        }

        private void InitializeDeck()
        {

            var colors = new[] { "Red"};
            int cardId = 1;
            Random random = new Random();

            foreach (var color in colors)
            {

                Cards.Add(new Card(cardId++, color, "0"));


                for (int i = 1; i <= 9; i++)
                {
                    Cards.Add(new Card(cardId++, color, i.ToString()));
                    Cards.Add(new Card(cardId++, color, i.ToString()));
                }

              Cards.Add(new Card(cardId++, color, null, CardType.Skip));
                Cards.Add(new Card(cardId++, color, null, CardType.Skip));
                Cards.Add(new Card(cardId++, color, null, CardType.Reverse));
                Cards.Add(new Card(cardId++, color, null, CardType.Reverse));
                Cards.Add(new Card(cardId++, color, null, CardType.DrawTwo)); 
                Cards.Add(new Card(cardId++, color, null, CardType.DrawTwo));
               
            }

            
            for (int i = 0; i < 4; i++)
            {
                string randomColor = colors[random.Next(colors.Length)]; // Выбор случайного цвета
                Cards.Add(new Card(cardId++, randomColor, null, CardType.Wild));
            }

            for (int i = 0; i < 4; i++)
            {
                string randomColor = colors[random.Next(colors.Length)]; // Выбор случайного цвета
                Cards.Add(new Card(cardId++, randomColor, null, CardType.WildDrawFour));
            }
        }







        public void Shuffle()
        {
            Random rng = new Random();
            int n = Cards.Count;

            for (int i = n - 1; i > 0; i--)
            {
                int k = rng.Next(i + 1); 
                                        
                var temp = Cards[i];
                Cards[i] = Cards[k];
                Cards[k] = temp;
            }
        }


        public Card Draw()
        {
            if (Cards.Count == 0) throw new InvalidOperationException("Колода пуста.");
            var card = Cards[0];
            Cards.RemoveAt(0);
            return card;
        }
    }
}
