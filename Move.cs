using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock_Paper_ScissorAI
{
    class Move
    {
        private string name;
        private string superiorMove;
        private string inferiorMove;

        public Move()
        {

        }
        private Move(string moveName)
        {
            name = moveName;

            InitializeName();
        }

        public string MoveName
        {
            get { return name; }
            set { name = value; InitializeName(); }
        }

        public Move SuperiorMove
        {
            get { return (new Move(superiorMove)); }
        }

        public Move InferiorMove
        {
            get { return (new Move(inferiorMove)); }
        }

       static public Move Rock
        {
            get { return (new Move("Rock")); }
        }

        static public Move Paper
        {
            get { return (new Move("Paper")); }
        }

        static public Move Scissor
        {
            get { return (new Move("Scissor")); }
        }

        static public Move Empty
        {
            get { return (new Move("Empty"));}
        }

        private void InitializeName()
        {
            
            switch(name.ToUpper())
            {
                case "ROCK": superiorMove = "Paper"; inferiorMove = "Scissor"; break;
                case "PAPER": superiorMove = "Scissor"; inferiorMove = "Rock"; break;
                case "SCISSOR" : superiorMove = "Rock"; inferiorMove = "Paper"; break;
            }

        }

        static public bool MoveEquals(Move move1, Move move2)
        {
            if(move1.name.ToUpper() == move2.name.ToUpper())
            {
                return true;
            }

            return false;
        }

    }

}

