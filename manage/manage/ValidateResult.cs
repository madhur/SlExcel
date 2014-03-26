using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace manage
{
    public class ValidateResult
    {
        private bool isValid = true;
        private List<TAB> faultTab = new List<TAB>();

        public bool IsValid
        {
            get { return isValid; }
            set { isValid = value; }
        }



        public List<TAB> FaultTab
        {
            get { return faultTab; }
            set { faultTab = value; }
        }

        public ValidateResult(bool isValid, List<TAB> tab)
        {
            this.isValid = isValid;
            this.faultTab = tab;

        }


        public ValidateResult()
        {

        }
    }

    public class SortTab : IComparer<TAB>
    {
        int IComparer<TAB>.Compare(TAB x, TAB y)
        {
            if (x == y)
                return 0;
            if (x == TAB.OVERVIEW && y == TAB.SCOPE)
                return -1;
            if (x == TAB.OVERVIEW && y == TAB.FINANCIAL)
                return -1;
            if (x == TAB.SCOPE && y == TAB.FINANCIAL)
                return -1;
            if (x == TAB.SCOPE && y == TAB.OVERVIEW)
                return 1;
            if (x == TAB.FINANCIAL && y == TAB.OVERVIEW)
                return 1;
            if (x == TAB.FINANCIAL && y == TAB.SCOPE)
                return 1;
            return 0;
        }
    }
}
