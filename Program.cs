using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameDevelopmentUtilities;

namespace Rock_Paper_ScissorAI
{
    class Program
    {

        static private AI ai;

        static private bool gameOn; //Tells whether the game is being played
        static private int roundsPlayed; //The number of rounds played
        static private int playerScore; //The score of the player

        static public Move playerMove;

        static void Main(string[] args)
        {
            Initialize(); //Initializing the console

            //Receiving player details
            Console.Write("Enter your name : ");
            ai.PlayerName = Console.ReadLine();
            Console.WriteLine("{0} are you ready to play a game of Rock,Paper,Scissors with me ?\n[Y]. Yes \n[N]. No", ai.PlayerName);
            DetermineWhetherThePlayerWantsToPlay();

            if(gameOn)
            {
                while(gameOn)
                {
                    ManageGame();
                }
            }
            else
            {
                Console.WriteLine("\nOk, maybe next time :( \n");
            }

        }

        static private void Initialize()
        {
           //Initializing Console
            Console.Title = "Rock,Paper,Scissor";
            Console.ForegroundColor = ConsoleColor.Green;

            //Initializing AI
            ai = new AI();

        }

        static private void DetermineWhetherThePlayerWantsToPlay()
        {
            bool validInputReceived = false;

            switch (Console.ReadLine())
            {
                case "Y":
                case "y": gameOn = true; validInputReceived = true ; break;
                case "N":
                case "n": gameOn = false; validInputReceived = true; break;
                default: Console.WriteLine("I don't understand what your are trying to say !! Try again.."); validInputReceived = false; break;
            }

            if(!validInputReceived)
            {
                DetermineWhetherThePlayerWantsToPlay();
            }

        }

        static private void ManageGame()
        {
            
            playerMove = GetPlayerInput();
            Move aiMove = ai.PlayMove();

            SelectWinner(playerMove, aiMove);
        }

        static private Move GetPlayerInput()
        {
            //Variables 
            string playerInput = ""; //The input provided by the player
            bool validInput = true;

            //Working

            Console.WriteLine("Rock,Paper,Scissor!!!\n[R]. Rock\n[P]. Paper\n[S]. Scissor\n[E]. Quit");
            playerInput = Console.ReadLine(); //Getting the player input
            roundsPlayed++; //Indicating that a new round has been played

            switch(playerInput.ToUpper())
            {
                case "R": return Move.Rock; break;
                case "P": return Move.Paper; break;
                case "S": return Move.Scissor; break;
                case "E": Quit(); break;
                default: Console.WriteLine("InvalidInput.. Try Again"); validInput = false; break;
            }

            if(!validInput)
            {
                Move playerMove = GetPlayerInput();
                return playerMove;
            }

            roundsPlayed++;

            return new Move();

        }

        static private void SelectWinner(Move playerMove, Move aiMove)
        {
           bool playerWon = false; //Tells whether the player won the round

            if(Move.MoveEquals(aiMove, playerMove.SuperiorMove))
            {
                ai.Score++; //Increasing the score of the AI
                Console.WriteLine("YES!!!! I won!!\nAI's Move : {0} [{1} Points]\nPlayer's Move : {2} [{3} Points]\n", aiMove.MoveName, ai.Score,playerMove.MoveName, playerScore);
            }
            else if(Move.MoveEquals(playerMove, aiMove.SuperiorMove))
            {
                playerScore++; //Increasing the score of the player
                Console.WriteLine("Oh...! You won and i lost :(\nPlayer's Move : {0} [{1} Points]\nAI's Move : {2} [{3} Points]\n", playerMove.MoveName, playerScore, aiMove.MoveName, ai.Score);
            }
            else if(Move.MoveEquals(playerMove, aiMove))
            {
                Console.WriteLine("Its a tie\nPlayer's Move : {0} [{1} Points]\nAI's Move : {2} [{3} Points]\n", playerMove.MoveName, playerScore, aiMove.MoveName, ai.Score);
            }

            ai.RecieveRoundResults(playerMove); //Sending the reseults of the round to the AI

            if (roundsPlayed > 5)
            {
                Console.Clear();
                roundsPlayed = 0;
            }
        }

        static private void Quit()
        {
            ai.SavePlayerData();
            Environment.Exit(0);
        }
    }
}

