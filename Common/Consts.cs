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
    public class GlobalConsts
    {
        public   const  String   ROLE_QUERY="<View><Query><FieldRef Name='Title' /><OrderBy> <FieldRef Name='Title'/></OrderBy></Query></View>";
        public  const  String   ROLE_LIST="RoleFamily";
        public  const String TITLE_COLUMN = "Title";
        public  const String NAME_FIELD = "Name";
        public const String ROLEFAMILY_COLUMN = "RoleFamily";
        public const String USER_MISSING_MSG = "The user does not exist or is not unique.";
        public const String USER_MISSING_MSG_NEW = "One or more FTE Contributors do not exist";

        public const String NO_RESULTS_FOUND = "No results found";

    }
}
