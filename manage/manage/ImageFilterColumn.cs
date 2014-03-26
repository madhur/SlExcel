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
using Telerik.Windows.Controls;

namespace manage
{
    public class ImageFilterColumn : GridViewImageColumn
    {
        /// <summary>
        /// Gets the filtering display function.
        /// </summary>
        /// <value>The filtering display function.</value>
        /// <remarks>This function is used by the filtering control distinct values list.
        /// It accepts a raw data value and returns what will become the content of the
        /// distinct value checkbox.</remarks>
        protected override Func<object, object> FilteringDisplayFunc
        {
            get { return ImageFilterColumn.ConvertUriStringToString; }
        }

        public static object ConvertUriStringToString(object uriString)
        {
            // var image = new Image();
            // image.Source = new BitmapImage(new Uri(uriString.ToString(), UriKind.Relative));
            // return image;

            String uri = uriString as String;

            if (uri.Contains(Consts.IMG_VAL_1))
                return Consts.FIL_VAL_1;

            if (uri.Contains(Consts.IMG_VAL_2) || uri.Contains(Consts.IMG_VAL_8))
                return Consts.FIL_VAL_2;

            if (uri.Contains(Consts.IMG_VAL_3) || uri.Contains(Consts.IMG_VAL_9))
                return Consts.FIL_VAL_3;

            if (uri.Contains(Consts.IMG_VAL_4))
                return Consts.FIL_VAL_4;

            if (uri.Contains(Consts.IMG_VAL_5))
                return Consts.FIL_VAL_5;

            if (uri.Contains(Consts.IMG_VAL_6))
                return Consts.FIL_VAL_6;

            if (uri.Contains(Consts.IMG_VAL_7))
                return Consts.FIL_VAL_7;

            //if (uri.Contains(Consts.IMG_VAL_8))
            //    return Consts.FIL_VAL_8;

            //if (uri.Contains(Consts.IMG_VAL_9))
            //    return Consts.FIL_VAL_9;

            return "";
        }
    }
}
