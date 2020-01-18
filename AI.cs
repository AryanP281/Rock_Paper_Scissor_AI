using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameDevelopmentUtilities;
using System.IO;

namespace Rock_Paper_ScissorAI
{
    class AI
    {
        #region Fields

        private CustomDictionary<string, bool> states = new CustomDictionary<string, bool>(); //The states of the AI

        private int score; //The score of the AI

        private string playerName; //The name of the player playing against the AI

        private Move[] moves = new Move[3];

        private CustomDictionary<MoveVector2, List<Move>> playerMoveHistory = new CustomDictionary<MoveVector2, List<Move>>(); //The ways in which the player responded to various move combinations
        private Move playerMove;
        private Move aiMove;
        private MoveVector2 playerAiMoveCombination;

        private byte roundCtr; //Keeps track of the round

        #endregion

        #region Constructor
        public AI()
        {
            InitializeAI();

        }
        #endregion

        #region Properties

        public string PlayerName
        {
            get { return playerName; }
            set { playerName = value; ReadPlayerData(@"Past Player Data\" + playerName.ToUpper() + ".txt"); }
        }

        public Move PlayerMove
        {
            get { return playerMove; }
            set { playerMove = value; if (playerAiMoveCombination == null) { playerAiMoveCombination = new MoveVector2(playerMove, aiMove); };}
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        #endregion

        #region Fundamental Methods

        public Move PlayMove()
        {
            if (states["Study"])
            {
                if (roundCtr <= 6)
                {
                    roundCtr++; //Incrementing the round counter
                    List<MoveVector2> keys = playerMoveHistory.ReturnKeys();

                    if (keys.Count != 0)
                    {
                        for(int a = 0;a < moves.Length;a++)
                        {
                            if(moves[a] == keys[keys.Count - 1].YComponent)
                            {
                                if(a == 2)
                                {
                                    aiMove = moves[0];
                                    return aiMove;
                                }
                                else
                                {
                                    aiMove = moves[a + 1];
                                    return aiMove;
                                }
                            }
                        }

                        roundCtr++; //Incrementing the rounds counter

                    }
                    else
                    {
                        aiMove = moves[0];
                        return aiMove;
                    }
                }
                else
                {
                    aiMove = moves[0];
                    states["Study"] = false;
                    states["Make Predictions"] = true;

                    return aiMove;
                }
            }
            else if(states["Make Predictions"])
            {
                aiMove = PredictPlayerMove();
                return aiMove;
            }

            return new Move();
        }

        public void RecieveRoundResults(Move playerMove)
        {
            if(playerAiMoveCombination != null)
            {
                List<MoveVector2> keys = playerMoveHistory.ReturnKeys();

                foreach(MoveVector2 combination in keys) //Adding the move to the player's move history
                {
                    if(combination.Equals(playerAiMoveCombination))
                    {
                        playerMoveHistory[combination].Add(playerMove);
                    }
                }

                playerAiMoveCombination = new MoveVector2(playerMove, aiMove); //Updating the combination vector
                bool combinationAlreadyExists = false;

                foreach(MoveVector2 combination in keys) //Checking whether the combination already exists in the moves list
                {
                    if(combination.Equals(playerAiMoveCombination))
                    {
                        combinationAlreadyExists = true;
                        break;
                    }
                }

                if(!combinationAlreadyExists) //Adding the combination to the list, as it does not exist in the list
                { 
                    playerMoveHistory.Add(playerAiMoveCombination, new List<Move>());
                }

            }
            else
            {
                playerAiMoveCombination = new MoveVector2(playerMove, aiMove);
                playerMoveHistory.Add(playerAiMoveCombination, new List<Move>());
            }

        }

        public void SavePlayerData()
        {
            string currentFileName = @"Past Player Data\" + playerName.ToUpper() + ".txt";

            if (!Directory.Exists(@"Past Player Data")) //Creating a new folder if it doesnt already exist
            {
                Directory.CreateDirectory(@"Past Player Data");
            }

            if(File.Exists(currentFileName)) //Checking whether the players file exists
            {
                StreamReader reader = new StreamReader(currentFileName);
                List<MoveVector2> combinations = playerMoveHistory.ReturnKeys();
                int lineCount = 0;

                //Updating the existing combinations
                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    lineCount++;
                    MoveVector2 combination = ReturnCombination(line);

                    //Updating the existing combinations
                    if(!combination.Equals(MoveVector2.EmptyVector))
                    {
                        reader.Close();
                        if(ListContainsCombination(combinations, combination))
                        {
                            List<string> lines = ListOfLines(currentFileName);
                             string lineContents = "";

                            for(int a = 0;a < lines.Count;a++)
                            {
                                if(line == lines[a])
                                {
                                    lineContents = ReadFromLine(currentFileName, a + 2);

                                    foreach(MoveVector2 existingCombinations in combinations)
                                    {
                                        if(combination.Equals(existingCombinations))
                                        {
                                            List<Move> moves = playerMoveHistory[existingCombinations];

                                            foreach(Move move in moves)
                                            {
                                                lineContents += move.MoveName;
                                            }
                                        }
                                    }
                                }
                            }

                            WriteOnLine(currentFileName, lineCount + 2, lineContents);
                            break;
                        }
                    }
                }

                reader.Close();


                //Adding new combinations to the file
                List<string> linesInFile = ListOfLines(currentFileName);
                foreach(MoveVector2 combination in combinations)
                {
                    bool combinationExists = false;

                    for(int a = 0;a < linesInFile.Count;a++)
                    {
                        if(combination.Equals(ReturnCombination(linesInFile[a])))
                        {
                            combinationExists = false;
                            break;
                        }
                    }

                    if(!combinationExists)
                    {
                        string textCombination = "(" + combination.XComponent.MoveName + "," + combination.YComponent.MoveName + ")";
                        WriteOnLine(currentFileName, linesInFile.Count + 2, textCombination);
                        linesInFile = ListOfLines(currentFileName); //Updating the list of lines

                    }
                }

            }
            else
            {
                //Creating a new file and writing the players information to it
                StreamWriter writer = new StreamWriter(currentFileName);
                List<MoveVector2> keys = playerMoveHistory.ReturnKeys();

                foreach(MoveVector2 combination in keys)
                {
                    writer.WriteLine("");
                    writer.WriteLine("(" + combination.XComponent.MoveName + "," + combination.YComponent.MoveName + ")");
                    writer.WriteLine("/");

                    for(int a = 0;a < playerMoveHistory[combination].Count;a++)
                    {
                        writer.Write(playerMoveHistory[combination].ElementAt(a).MoveName + " ");
                    }

                    writer.WriteLine("");
                }

                writer.Close();
            }
        }

        #endregion

        #region Helper Methods

        private void InitializeAI()
        {
            //Initializing AI states
            states.Add("Study", true);
            states.Add("Make Predictions", false);
            states.Add("Use past knowledge", false);

            //Initializing Moves
            roundCtr = 1;
            moves[0] = Move.Rock;
            moves[1] = Move.Paper;
            moves[2] = Move.Scissor;

        }

        private Move PredictPlayerMove()
        {
            bool moveCombinationFound = false;
            List<MoveVector2> combinations = playerMoveHistory.ReturnKeys();
            List<Move> moveList = new List<Move>();

            foreach(MoveVector2 combination in combinations)
            {
                if(combination.Equals(playerAiMoveCombination))
                {
                    if (playerMoveHistory[combination].Count > 0)
                    {
                        moveList = playerMoveHistory[combination];
                        moveCombinationFound = true;
                    }

                    break;
                }
            }

            if(!moveCombinationFound)
            {
                List<Move> moveHistory = new List<Move>();
                foreach(MoveVector2 combination in combinations)
                {
                    for(int a = 0;a < playerMoveHistory[combination].Count;a++)
                    {
                        moveHistory.Add(playerMoveHistory[combination].ElementAt(a));
                    }
                }

                aiMove = ReturnMaxOccurringMove(moveHistory).SuperiorMove;
                return aiMove;
            }
            else
            {
                Move maxOccuringMove = ReturnMaxOccurringMove(moveList);

                return maxOccuringMove.SuperiorMove;
            }

            return new Move();
        }

        private Move ReturnMaxOccurringMove(List<Move> moves)
        {
            int[] moveOccurences = new int[moves.Count];
            int maxOccurence = 0;

            foreach(Move move in moves)
            {
                for(int a = 0;a < moves.Count;a++)
                {
                    if(Move.MoveEquals(move, moves[a]))
                    {
                        moveOccurences[a]++;
                    }
                }
            }

            for (int a = 0; a < moveOccurences.Length;a++)
            {
                if(moveOccurences[a] > moveOccurences[maxOccurence])
                {
                    maxOccurence = a;
                }
            }

            return (moves[maxOccurence]);
            
        }

        private MoveVector2 ReturnCombination(string line)
        {
            MoveVector2 combination = MoveVector2.EmptyVector;

            if (line.Length > 0)
            {
                if (line[0] == '(')
                {
                    string playerMoveName = "";
                    Move playerMove = new Move();
                    bool playerMoveInputRecieved = false;
                    string aiMoveName = "";
                    Move aiMove = new Move();

                    for (int a = 1; a < line.Length; a++)
                    {
                        if (line[a] != ',' && !playerMoveInputRecieved)
                        {
                            playerMoveName += line[a];
                        }
                        else
                        {
                            playerMoveInputRecieved = true;
                            if (line[a + 1] != ')')
                            {
                                aiMoveName += line[a + 1];
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    switch (playerMoveName.ToUpper())
                    {
                        case "ROCK": playerMove = Move.Rock; break;
                        case "PAPER": playerMove = Move.Paper; break;
                        case "SCISSOR": playerMove = Move.Scissor; break;
                    }

                    switch (aiMoveName.ToUpper())
                    {
                        case "ROCK": aiMove = Move.Rock; break;
                        case "PAPER": aiMove = Move.Paper; break;
                        case "SCISSOR": aiMove = Move.Scissor; break;
                    }

                    combination = new MoveVector2(playerMove, aiMove);

                }
            }

            return combination;
        }

        private List<Move> ReturnMoves(string line)
        {
            List<string> moveNames = new List<string>();
            List<Move> moves = new List<Move>();

            if(line.Length > 0)
            {
                string moveName = "";

                for(int charCtr = 0;charCtr < line.Length;charCtr++)
                {
                    if (line[charCtr] != ' ')
                    {
                        moveName += line[charCtr];
                    }
                    else
                    {
                        moveNames.Add(moveName);
                        moveName = "";
                    }
                }
            }

            foreach(string moveName in moveNames)
            {
                switch(moveName.ToUpper())
                {
                    case "ROCK": moves.Add(Move.Rock); break;
                    case "PAPER": moves.Add(Move.Paper); break;
                    case "SCISSOR": moves.Add(Move.Scissor); break;
                }
            }

            return moves;
        }

        private List<string> ListOfLines(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            List<string> lines = new List<string>();

            while(!reader.EndOfStream)
            {
                lines.Add(reader.ReadLine());
            }

            reader.Close();

            return lines;
        }

        private void WriteOnLine(string fileName, int lineNumber, string textToAdd)
        {
            List<string> lines = ListOfLines(fileName);
            
            for (int lineCount = 0;lineCount < lines.Count; lineCount++) //Adding the provided text to the line
            { 
                if(lineCount == lineNumber)
                {
                    lines[lineCount - 1] = textToAdd;
                    break;
                }
            }

            StreamWriter writer = new StreamWriter(fileName);

            for(int a = 0;a < lines.Count;a++) //Writing to the file
            {
                writer.WriteLine(lines[a]);
            }

            writer.Close();

        }

        private string ReadFromLine(string fileName, int lineNumber)
        {
            string line = "";
            bool lineFound = false;
            List<string> lines = ListOfLines(fileName);

            for (int a = 0; a < lines.Count;a++)
            {
                if(a == lineNumber)
                {
                    line = lines[a];
                    lineFound = true;
                    break;
                }
            }

            if(lineFound)
            {
                return line;
            }

            throw new Exception("Line Does Not Exist ");
        }

        private bool ListContainsCombination(List<MoveVector2> list, MoveVector2 combination)
        {
            foreach(MoveVector2 vec in list)
            {
                if(combination.Equals(vec))
                {
                    return true;
                }
            }

            return false;
        }

        private void ReadPlayerData(string filePath)
        {
            if (File.Exists(filePath))
            {
                StreamReader reader = new StreamReader(filePath);
                List<string> lines = ListOfLines(filePath);

                for (int lineCtr = 0; lineCtr < lines.Count; lineCtr++)
                {
                    MoveVector2 combination = ReturnCombination(lines[lineCtr]);

                    if (!combination.Equals(MoveVector2.EmptyVector))
                    {
                        playerMoveHistory.Add(combination, ReturnMoves(lines[lineCtr + 2]));
                    }
                }

                reader.Close();

                playerAiMoveCombination = playerMoveHistory.ReturnKeys()[0];
                //Skipping the study state and switching to predicting player moves
                states["Study"] = false;
                states["Make Predictions"] = true;
            }
        }

        #endregion

    }
}

