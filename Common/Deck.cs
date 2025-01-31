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

            var colors = new[] { CardColor.Red };
            int cardId = 1;
            Random random = new Random();

            foreach (var color in colors)
            {

                Cards.Add(new Card(cardId++, color, CardValue.Zero));

                // Добавляем карты с значениями от 1 до 9
                for (int i = 1; i <= 9; i++)
                {
                    Cards.Add(new Card(cardId++, color, (CardValue)i)); // Используем перечисление CardValue
                    Cards.Add(new Card(cardId++, color, (CardValue)i)); // Используем перечисление CardValue
                    // Используем перечисление CardValue
                    // Cards.Add(new Card(cardId++, color, null, CardType.DrawTwo)); // Добавляем карты DrawTwo
                }
               /* Cards.Add(new Card(cardId++, color, CardValue.Skip, CardType.Skip));
                Cards.Add(new Card(cardId++, color, CardValue.Skip, CardType.Skip));
                Cards.Add(new Card(cardId++, color, CardValue.Reverse, CardType.Reverse));
                Cards.Add(new Card(cardId++, color, CardValue.Reverse, CardType.Reverse));
                Cards.Add(new Card(cardId++, color, CardValue.DrawTwo, CardType.DrawTwo));
               */
            }


           /*for (int i = 0; i < 8; i++)
            {
                CardColor randomColor = colors[random.Next(colors.Length)]; // Выбор случайного цвета
                Cards.Add(new Card(cardId++, randomColor, CardValue.Wild, CardType.Wild)); // Используем значение Wild
            }

            for (int i = 0; i < 8; i++)
            {
                CardColor randomColor = colors[random.Next(colors.Length)]; // Выбор случайного цвета
                Cards.Add(new Card(cardId++, randomColor, CardValue.WildDrawFour, CardType.WildDrawFour)); // Используем значение WildDrawFour
            }*/




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
