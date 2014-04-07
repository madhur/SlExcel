using Microsoft.SharePoint.Client;
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

namespace Common
{
    public class Utils
    {
        public static string checkClaimsUser(String userName)
        {
            string decodedName;

            if (userName.Contains("|"))
            {
                string[] splitUserName = userName.Split(new Char[] { '|' }, StringSplitOptions.None);

                if (splitUserName.Length > 0)
                    decodedName = splitUserName[1];
                else
                    decodedName = userName;
            }
            else
            {
                decodedName = userName;
            }


            return decodedName.ToLower();

        }

        public static String GetSiteUrl()
        {
            return ClientContext.Current.Url;
        }
    }
}
