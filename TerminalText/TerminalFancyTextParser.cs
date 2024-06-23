using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TerminalText
{
    // needed for "<cf-red>Hi <cf-blu>USER</cf-blu>, are you Good?</cf-red>"
    internal class TerminalFancyTextParser
    {
        enum TerminalFancyTextParserStackThing
        {
            None,
            BoldDim,
            ColorFG,
            ColorBG
        }

        public static string OpeningTagCharacter = "<";
        public static string ClosingTagCharacter = ">";
        public static string EndingTagCharacter  = "/";
        public static string EscapeTagCharacter  = "\\";

        // {tag name, [set, reset], stack bullshit}
        public static Dictionary<string, dynamic[]> Tags = new Dictionary<string, dynamic[]>(){
            // text styles
            { "b"     , [TerminalANSICodes.BoldSet         , TerminalANSICodes.BoldAndDimReset   , TerminalFancyTextParserStackThing.BoldDim] }, 
            { "dim"   , [TerminalANSICodes.DimSet          , TerminalANSICodes.BoldAndDimReset   , TerminalFancyTextParserStackThing.BoldDim] }, 
            { "i"     , [TerminalANSICodes.ItalicSet       , TerminalANSICodes.ItalicReset       , TerminalFancyTextParserStackThing.None   ] },
            { "u"     , [TerminalANSICodes.UnderlineSet    , TerminalANSICodes.UnderlineReset    , TerminalFancyTextParserStackThing.None   ] },
            { "blink" , [TerminalANSICodes.BlinkingSet     , TerminalANSICodes.BlinkingReset     , TerminalFancyTextParserStackThing.None   ] },
            { "inv"   , [TerminalANSICodes.InvertSet       , TerminalANSICodes.InvertReset       , TerminalFancyTextParserStackThing.None   ] },
            { "hid"   , [TerminalANSICodes.HiddenSet       , TerminalANSICodes.HiddenReset       , TerminalFancyTextParserStackThing.None   ] },
            { "strike", [TerminalANSICodes.StrikeThroughSet, TerminalANSICodes.StrikeThroughReset, TerminalFancyTextParserStackThing.None   ] },


            // foreground base colors
            { "cf-blck", [TerminalANSICodes.ColorBlackFG  , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "cf-red" , [TerminalANSICodes.ColorRedFG    , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "cf-grn" , [TerminalANSICodes.ColorGreenFG  , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "cf-yel" , [TerminalANSICodes.ColorYellowFG , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "cf-blu" , [TerminalANSICodes.ColorBlueFG   , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "cf-mag" , [TerminalANSICodes.ColorMagentaFG, TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "cf-cyan", [TerminalANSICodes.ColorCyanFG   , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "cf-whit", [TerminalANSICodes.ColorWhiteFG  , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "cf-def" , [TerminalANSICodes.ColorDefaultFG, TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] }, // uh... really? is this really needed?

            // background base colors
            { "cb-blck", [TerminalANSICodes.ColorBlackBG  , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "cb-red" , [TerminalANSICodes.ColorRedBG    , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "cb-grn" , [TerminalANSICodes.ColorGreenBG  , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "cb-yel" , [TerminalANSICodes.ColorYellowBG , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "cb-blu" , [TerminalANSICodes.ColorBlueBG   , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "cb-mag" , [TerminalANSICodes.ColorMagentaBG, TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "cb-cyan", [TerminalANSICodes.ColorCyanBG   , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "cb-whit", [TerminalANSICodes.ColorWhiteBG  , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "cb-def" , [TerminalANSICodes.ColorDefaultBG, TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] }, // uh... really? is this really needed? (part2)


            // foreground Bright colors
            { "bcf-blck", [TerminalANSICodes.ColorBrightBlackFG  , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "bcf-red" , [TerminalANSICodes.ColorBrightRedFG    , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "bcf-grn" , [TerminalANSICodes.ColorBrightGreenFG  , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "bcf-yel" , [TerminalANSICodes.ColorBrightYellowFG , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "bcf-blu" , [TerminalANSICodes.ColorBrightBlueFG   , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "bcf-mag" , [TerminalANSICodes.ColorBrightMagentaFG, TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "bcf-cyan", [TerminalANSICodes.ColorBrightCyanFG   , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },
            { "bcf-whit", [TerminalANSICodes.ColorBrightWhiteFG  , TerminalANSICodes.ColorDefaultFG, TerminalFancyTextParserStackThing.ColorFG] },

            // background Bright colors
            { "bcb-blck", [TerminalANSICodes.ColorBrightBlackBG  , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "bcb-red" , [TerminalANSICodes.ColorBrightRedBG    , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "bcb-grn" , [TerminalANSICodes.ColorBrightGreenBG  , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "bcb-yel" , [TerminalANSICodes.ColorBrightYellowBG , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "bcb-blu" , [TerminalANSICodes.ColorBrightBlueBG   , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "bcb-mag" , [TerminalANSICodes.ColorBrightMagentaBG, TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "bcb-cyan", [TerminalANSICodes.ColorBrightCyanBG   , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },
            { "bcb-whit", [TerminalANSICodes.ColorBrightWhiteBG  , TerminalANSICodes.ColorDefaultBG, TerminalFancyTextParserStackThing.ColorBG] },

            // there's some special tags that are defined via code (256 colors)
            // except this one which requires a lot of extra shit
            { "reset", [TerminalANSICodes.ResetAll, TerminalANSICodes.ResetAll, TerminalFancyTextParserStackThing.None] }
        };

        public static string ParseTextWithANSICodes(string text, bool sanitize = false)
        {
            // bold, dim, italy, underline, blink, rev, hid, strike, fgcol, bgcol
            string[] globalANSICodes = new string[10]{"", "", "", "", "", "", "", "", "", ""};
            List<string> boldDimANSICodes = new List<string>();
            List<string> colorFGANSICodes = new List<string>();
            List<string> colorBGANSICodes = new List<string>();

            string returnString = "";
            int stringIndex = 0;

            while (true)
            {
                int nextIndex = text.IndexOf(OpeningTagCharacter, stringIndex);

                // what the fuck ?????? why must i do ToCharArray[0]??? plus these escapes are Opposited
                bool escapedOpenTag = nextIndex == 0 
                                      || (nextIndex > 0 && text[nextIndex - 1] != EscapeTagCharacter.ToCharArray()[0]);
                if (!escapedOpenTag)
                {
                    string erm = text.Substring(stringIndex, text.Length - stringIndex);
                    // uhh hm ah 
                    returnString += erm
                                    .Replace(EscapeTagCharacter + OpeningTagCharacter, OpeningTagCharacter)
                                    .Replace(EscapeTagCharacter + ClosingTagCharacter, ClosingTagCharacter);
                    if (nextIndex == -1)
                        break;
                    stringIndex = nextIndex + 1;
                    continue;
                }

                // uh basically just add text
                // hm uhhh ahhh err part 2
                returnString += text.Substring(stringIndex, nextIndex - stringIndex)
                                .Replace(EscapeTagCharacter + OpeningTagCharacter, OpeningTagCharacter)
                                .Replace(EscapeTagCharacter + ClosingTagCharacter, ClosingTagCharacter);
                stringIndex = nextIndex + 1;

                int closingIndex = text.IndexOf(ClosingTagCharacter, stringIndex);
                // Read above
                bool escapedCloseTag = closingIndex == 0 
                                       || (closingIndex > 0 && text[closingIndex - 1] != EscapeTagCharacter.ToCharArray()[0]);
                if (!escapedCloseTag)
                    continue;

                string tag = text.Substring(stringIndex, closingIndex - stringIndex).ToLower();
                bool endTag = tag[0] == EndingTagCharacter.ToCharArray()[0];
                if (endTag)
                    tag = text.Substring(stringIndex + 1, closingIndex - stringIndex - 1).ToLower();

                stringIndex = closingIndex + 1;

                // sort shit out
                dynamic[]? tagInfo;
                bool tagExist = Tags.TryGetValue(tag, out tagInfo);
                tagInfo ??= ["null", "null", TerminalFancyTextParserStackThing.None];

                if ((!tagExist && !tag.StartsWith("cc")) || sanitize)
                    continue;

                returnString += UnderstandTag(tag, tagInfo, endTag, ref globalANSICodes, ref boldDimANSICodes,
                                              ref colorFGANSICodes, ref colorBGANSICodes);
            }

            return returnString;
        }

        static string UnderstandTag(string tag, dynamic[] tagInfo, bool endingTag,
                                    ref string[] globalANSICodes, ref List<string> boldDimANSICodes,
                                    ref List<string> colorFGANSICodes, ref List<string> colorBGANSICodes)
        {
            if (!endingTag)
            {
                // cc = custom color
                if (tag.StartsWith("cc"))
                {
                    bool bg;
                    string codeAnsi = ParseCustomColor(tag, out bg);

                    globalANSICodes[bg ? 9 : 8] = codeAnsi;
                    (bg ? colorBGANSICodes : colorFGANSICodes).Add(codeAnsi);
                    return TerminalANSICodes.BuildANSITag(codeAnsi);
                }

                int globalIndex = -1;
                // sort out that reset bs
                switch ((TerminalFancyTextParserStackThing)tagInfo[2])
                {
                    case TerminalFancyTextParserStackThing.None:
                        switch (tag)
                        {
                            case "i"     : globalIndex = 2; break;
                            case "u"     : globalIndex = 3; break;
                            case "blink" : globalIndex = 4; break;
                            case "inv"   : globalIndex = 5; break;
                            case "hid"   : globalIndex = 6; break;
                            case "strike": globalIndex = 7; break;
                        }
                        break;
                    // whilst we're here
                    case TerminalFancyTextParserStackThing.BoldDim:
                        globalIndex = tag == "b" ? 0 : 1;
                        boldDimANSICodes.Add(tagInfo[0]);
                        break;
                    case TerminalFancyTextParserStackThing.ColorFG:
                        globalIndex = 8;
                        colorFGANSICodes.Add(tagInfo[0]);
                        break;
                    case TerminalFancyTextParserStackThing.ColorBG:
                        globalIndex = 9;
                        colorBGANSICodes.Add(tagInfo[0]);
                        break;
                }
                if (globalIndex != -1)
                    globalANSICodes[globalIndex] = tagInfo[0];
                return TerminalANSICodes.BuildANSITag(tagInfo[0]);
            }

            if (tag == "reset")
                return TerminalANSICodes.BuildANSITag(globalANSICodes);


            string ansiCode;
            if (tag.StartsWith("cc"))
            {
                bool bg;
                // then we can just leave it
                tagInfo[0] = ParseCustomColor(tag, out bg);
                tagInfo[2] = bg ? TerminalFancyTextParserStackThing.ColorBG : TerminalFancyTextParserStackThing.ColorFG;
                ansiCode = TerminalANSICodes.BuildANSITag(bg ? TerminalANSICodes.ColorDefaultBG : TerminalANSICodes.ColorDefaultFG);
            }
            else
                ansiCode = TerminalANSICodes.BuildANSITag(tagInfo[1]);
            // great, time for this stacking bullshit 😁
            switch ((TerminalFancyTextParserStackThing)tagInfo[2])
            {
                case TerminalFancyTextParserStackThing.None:
                    break;
                case TerminalFancyTextParserStackThing.BoldDim:
                    string ret = DoLastRemoveThing(boldDimANSICodes, tagInfo[0]);
                    if (ret != "")
                        ansiCode += TerminalANSICodes.BuildANSITag(ret);
                    break;
                case TerminalFancyTextParserStackThing.ColorFG:
                    string ret2 = DoLastRemoveThing(colorFGANSICodes, tagInfo[0]);
                    if (ret2 != "")
                        ansiCode = TerminalANSICodes.BuildANSITag(ret2);
                    break;
                case TerminalFancyTextParserStackThing.ColorBG:
                    string ret3 = DoLastRemoveThing(colorBGANSICodes, tagInfo[0]);
                    if (ret3 != "")
                        ansiCode = TerminalANSICodes.BuildANSITag(ret3);
                    break;
            }

            return ansiCode;
        }

        static string DoLastRemoveThing(List<string> thing, string endTagThing)
        {
            if (thing.Count != 0)
            {
                int ding = thing.LastIndexOf(endTagThing);
                if (ding != -1)
                    thing.RemoveAt(ding);
                return thing.Count == 0 ? "" : thing[thing.Count - 1];
            } 
            return "";
        }

        // yea uh i think it's better i do this
        static string ParseCustomColor(string tag, out bool bg)
        {
            // here's the tags
            // ccf-RGB (0-6)
            // ccb-RGB (0-6)
            // ccgf-GRAY (0-23)
            // ccgb-GRAY (0-23)
            // cc24f-R-G-B (0-255)
            // cc24b-R-G-B (0-255)
            string[] splitbyDash = tag.Split("-");
            bg = splitbyDash[0].Last() == "b".ToCharArray()[0];
            string codeOfTheAnsi;
            // idk man
            try
            {
                if (splitbyDash.Length >= 4 && splitbyDash[0].Substring(0, 4) == "cc24") 
                {
                    if (splitbyDash.Length != 4)
                        throw new Exception($"Invalid amount of dashes in the tag of `{tag}`. There should only be 3 dashes.");
                    codeOfTheAnsi = TerminalANSICodes.GetRGB24Color(Convert.ToInt32(splitbyDash[1]), Convert.ToInt32(splitbyDash[2]), 
                                                                    Convert.ToInt32(splitbyDash[3]), bg);
                }
                else 
                {
                    if (splitbyDash.Length != 2)
                        throw new Exception($"Invalid amount of dashes in the tag of `{tag}`. There should only be 1 dash.");
                        
                    if (splitbyDash[0].Substring(0, 3) == "ccg")
                        codeOfTheAnsi = TerminalANSICodes.GetGrayColor(Convert.ToInt32(splitbyDash[1]), bg);
                    else
                        codeOfTheAnsi = TerminalANSICodes.GetRGB666Color(Convert.ToInt32(splitbyDash[1][0].ToString()), 
                                                                         Convert.ToInt32(splitbyDash[1][1].ToString()), 
                                                                         Convert.ToInt32(splitbyDash[1][2].ToString()), bg);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Couldn't handle and error'ed on the tag of `{tag}`. Error message: {e.Message}.");
            }

            // then uh.. yeah should work?
            return codeOfTheAnsi;
        }
    }

        
}
