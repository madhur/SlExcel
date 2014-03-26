using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace excel_create
{
  
        public class MyItem
        {
            public String AIM_ID { get; set; }
            public String AIM_NAME { get; set; }

            public String Content
            {
                get
                {
                    return AIM_NAME;
                }
                set
                {
                    AIM_NAME = value;
                }
            }

            public override string ToString()
            {
                return AIM_NAME;
            }



        }

}
