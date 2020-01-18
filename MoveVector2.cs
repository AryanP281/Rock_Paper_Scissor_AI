using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock_Paper_ScissorAI
{
    class MoveVector2
    {
        #region Fields

        private Move xComponent;
        private Move yComponent;

        #endregion

        #region Constructor

        public MoveVector2(Move xComponent, Move yComponent)
        {
            this.xComponent = xComponent;
            this.yComponent = yComponent;
        }

        #endregion

        #region Properties

        public Move XComponent
        {
            get { return xComponent; }
            set { xComponent = value; }
        }

        public Move YComponent
        {
            get { return yComponent; }
            set { yComponent = value; }
        }

        static public MoveVector2 EmptyVector
        {
            get { return new MoveVector2(Move.Empty, Move.Empty); }
        }

        #endregion

        #region Methods

        public bool Equals(MoveVector2 vec1)
        {
            if(Move.MoveEquals(vec1.xComponent, xComponent) && Move.MoveEquals(vec1.yComponent, yComponent))
            {
                return true;
            }

            return false;
        }

        #endregion

    }
}

