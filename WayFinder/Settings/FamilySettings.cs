using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace WayFinder.Settings
{
    public static class FamilySettings
    {
        // ===================== WALL FAMILIES DETAILS =======================================
        public const string WallSignName = "wallSign"; // family name


        // ====================== PARAMETERS =============================================
        public const string SharedParamGroup = "WayFinder Shared Parameters"; // shared param group name

        private const string RefdRm = "Referenced Room"; // ref'd room instance param name
        private const string SignNum = "Sign Number"; // the sign's tracking number

        // the dictionary containg all shared parameters
        public static Dictionary<string, ForgeTypeId> sharedParams = new Dictionary<string, ForgeTypeId>()
        {
            {RefdRm, SpecTypeId.String.Text},
            {SignNum, SpecTypeId.Number}
        };

        public const string WallSignTagNm = "wayFinderTag"; // the annotation tag name





    }
}
