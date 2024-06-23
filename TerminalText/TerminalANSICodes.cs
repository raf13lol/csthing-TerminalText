using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TerminalText
{
    // this https://gist.github.com/fnky/458719343aabd01cfb17a3a4f7296797 page helped out
    internal class TerminalANSICodes
    {
        // important shit
        public const string ANSICodePrefix = "\x1b[";
        public const string ResetAll       = "0";

        // text styles set
        public const string BoldSet          = "1";
        public const string DimSet           = "2";
        public const string ItalicSet        = "3";
        public const string UnderlineSet     = "4";
        public const string BlinkingSet      = "5";
        public const string InvertSet        = "7";
        public const string HiddenSet        = "8";
        public const string StrikeThroughSet = "9";

        // text styles reset
        public const string BoldAndDimReset    = "22";
        public const string ItalicReset        = "23";
        public const string UnderlineReset     = "24";
        public const string BlinkingReset      = "25";
        public const string InvertReset        = "27";
        public const string HiddenReset        = "28";
        public const string StrikeThroughReset = "29";

        // foreground base colors
        public const string ColorBlackFG   = "30";
        public const string ColorRedFG     = "31";
        public const string ColorGreenFG   = "32";
        public const string ColorYellowFG  = "33";
        public const string ColorBlueFG    = "34";
        public const string ColorMagentaFG = "35";
        public const string ColorCyanFG    = "36";
        public const string ColorWhiteFG   = "37";
        public const string ColorDefaultFG = "39";

        // background base colors
        public const string ColorBlackBG   = "40";
        public const string ColorRedBG     = "41";
        public const string ColorGreenBG   = "42";
        public const string ColorYellowBG  = "43";
        public const string ColorBlueBG    = "44";
        public const string ColorMagentaBG = "45";
        public const string ColorCyanBG    = "46";
        public const string ColorWhiteBG   = "47";
        public const string ColorDefaultBG = "49";

        // foreground bright colors
        public const string ColorBrightBlackFG   = "90";
        public const string ColorBrightRedFG     = "91";
        public const string ColorBrightGreenFG   = "92";
        public const string ColorBrightYellowFG  = "93";
        public const string ColorBrightBlueFG    = "94";
        public const string ColorBrightMagentaFG = "95";
        public const string ColorBrightCyanFG    = "96";
        public const string ColorBrightWhiteFG   = "97";

        // background bright colors
        public const string ColorBrightBlackBG   = "100";
        public const string ColorBrightRedBG     = "101";
        public const string ColorBrightGreenBG   = "102";
        public const string ColorBrightYellowBG  = "103";
        public const string ColorBrightBlueBG    = "104";
        public const string ColorBrightMagentaBG = "105";
        public const string ColorBrightCyanBG    = "106";
        public const string ColorBrightWhiteBG   = "107";


        // the 256 colors, https://user-images.githubusercontent.com/12821885/251197668-6d232fa0-f8ad-4cab-8a7a-24b3bb08b481.png helped out with this

        public const string Color256FG = "38;5;COL";
        public const string Color256BG = "48;5;COL";

        private static string ReplaceColorThing(int col, bool bg = false)
        {
            return (bg ? Color256BG : Color256FG).Replace("COL", col.ToString());
        }

        // standard colors
        /* Not needed as these are jsut the consts
        public static string GetStandardColor(bool r, bool g, bool b, bool lighter = false, bool bg = false)
        {
            int redNum   = r ? 1 : 0;
            int greenNum = g ? 2 : 0;
            int blueNum  = b ? 4 : 0;

            return ReplaceColorThing(redNum + greenNum + blueNum + (lighter ? 8 : 0), bg);
        }
        */

        // "rgb666" colors
        public static string GetRGB666Color(int r, int g, int b, bool bg = false)
        {
            if (r >= 6 || g >= 6 || b >= 6)
                throw new Exception("R/G/B of GetRGB666Color cannot be over 5.");
            if (r < 0 || g < 0 || b < 0)
                throw new Exception("R/G/B of GetRGB666Color cannot be under 0");

            return ReplaceColorThing(r * 36 + g * 6 + b + 16, bg);
        }

        // gray colors
        public static string GetGrayColor(int lightness, bool bg = false)
        {
            if (lightness >= 24)
                throw new Exception("lightness of GetGrayColor cannot be over 23.");
            if (lightness < 0)
                throw new Exception("lightness of GetGrayColor cannot be under 0");

            return ReplaceColorThing(lightness + 232, bg);
        }

        // apparently terminals also support 24-bit rgb but i don't want to add that so i'll leave it in as an extra
        public static string GetRGB24Color(int r, int g, int b, bool bg = false)
        {
            if (r >= 256 || g >= 256 || b >= 256)
                throw new Exception("R/G/B of GetRGB24Color cannot be over 255.");
            if (r < 0 || g < 0 || b < 0)
                throw new Exception("R/G/B of GetRGB24Color cannot be under 0");

            return $"{(bg ? "4" : "3")}8;2;{r};{g};{b}";
        }


        public static string BuildANSITag(string[] tags)
        {
            // Stupid idea 😁😁😁
            List<string> cleansedTags = new List<string>();
            foreach (string str in tags)
            {
                if (str.Length > 0)
                    cleansedTags.Add(str);
            }
            return ANSICodePrefix + string.Join(";", cleansedTags) + "m";
        }
        public static string BuildANSITag(string tag, bool addM = true)
        {
            return ANSICodePrefix + tag + (addM ? "m" : "");
        }
    }
}
