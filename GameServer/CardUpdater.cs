using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace UnoServer
{
    public class CardUpdater
    {

      

        public CardUpdater()
        {
           
        }

        public string HandleSkipCard(string turnPlayer, List<Player> players)
        {
            Console.WriteLine($"Игрок {turnPlayer} пропускает ход следующего игрока."); 

            int currentPlayerIndex = players.FindIndex(player => player.Nickname == turnPlayer);

           

            int nextPlayerIndex = (currentPlayerIndex + 2) % players.Count;
            turnPlayer = players[nextPlayerIndex].Nickname;

            return turnPlayer;


        }
        public ( List<Card>, int) NotUno(string turnPlayer, List<Player> players, int addCards, Deck deck)
        {
            int currentPlayerIndex = players.FindIndex(player => player.Nickname == turnPlayer);
            int previousPlayerIndex = (currentPlayerIndex - 1 + players.Count) % players.Count;
            Player previousPlayer = players[previousPlayerIndex];

            List<Card> updatedHand = AddCardsToPlayer(previousPlayer, addCards, deck);

           
            Console.WriteLine($"Игрок {turnPlayer} заставляет предыдущего игрока взять {addCards} карты.");
            return ( updatedHand, previousPlayerIndex);
        }

        public (string , List<Card> ,int ) HandleDraw(string turnPlayer, List<Player> players,int addCards,Deck deck)
        {
            int currentPlayerIndex = players.FindIndex(player => player.Nickname == turnPlayer);
            int nextPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            int nextTurnIndex = (currentPlayerIndex + 2) % players.Count;
            turnPlayer = players[nextTurnIndex].Nickname;
            Player nextPlayer = players[nextPlayerIndex];
            List<Card> updatedHand = AddCardsToPlayer(nextPlayer, addCards,deck);

            Console.WriteLine($"Игрок {turnPlayer} заставляет  взять 4 карты.");
            return (turnPlayer, updatedHand, nextPlayerIndex);
        }

        public string HandleNumber(string turnPlayer,List<Player> players)
        {
            Console.WriteLine($"Игрок {turnPlayer} goes."); 

            int currentPlayerIndex = players.FindIndex(player => player.Nickname == turnPlayer);

           

            int nextPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            turnPlayer = players[nextPlayerIndex].Nickname;
           

            return turnPlayer;
        }
        private List<Card> AddCardsToPlayer(Player player, int numberOfCards,Deck deck)
        {
            List<Card> playerHand  = player.Hand;

            for (int i = 0; i < numberOfCards; i++)
            {

                Card drawnCard = deck.Draw(); 

                player.Hand.Add(drawnCard); 

            }

            return playerHand; 
        }


        public (List<Player> players,string turnPlayer) HandleReverse(string turnPlayer, List<Player> players)
        {
            if (players.Count <= 2)
            {
                return (players,turnPlayer);
            }

            var reversedPlayers = new List<Player>(players);
            reversedPlayers.Reverse();


            int currentIndex = reversedPlayers.FindIndex(p => p.Nickname == turnPlayer);
            string nextTurnPlayer = reversedPlayers[(currentIndex + 1) % reversedPlayers.Count].Nickname;

            return (reversedPlayers, nextTurnPlayer);
        }




    }
}
