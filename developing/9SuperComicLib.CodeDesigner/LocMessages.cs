using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperComicLib.DataObject;

namespace SuperComicLib.CodeDesigner
{
    [Config("localization/locmsg")]
    public static class LocMessages
    {
        [MarkAsName("isnull")]
        public static string Format0_IsNull; // {0} 이(가) null 이었습니다
        [MarkAsName("invalid")]
        public static string Format0_Invalid; // {0} 은(는) 유효하지 않습니다
    }
}
