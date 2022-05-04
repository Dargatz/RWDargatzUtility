using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using RWDargatzUtility.MRFParser.ValueTypes;

namespace RWDargatzUtility.MRFParser
{
    public class FileParser
    {
        #region Markers and Help
        private const char _labelMarker = '=';
        private const char _sectionStart = '{';
        private const char _sectionEnd = '}';
        private const char _separator = ',';
        private const string _commentStarter = "//";
        private const string _dataStart = "[";
        private const string _dataEnd = "]";

        private enum LineType
        {
            SectionStart,
            SectionEnd,
            LabeledLine,
            Data,
            Comment,
            Nothing,
            Unknown
        }

        #endregion

        public string Section { get; private set; }

        public List<FileParser> Subsections { get; private set; }

        public string Name { get; private set; }

        public FileParser(string path)
        {
            Section = GetFileString(path);
            Name = "File";

            Subsections = createSubSections();
        }
        
        public FileParser(string body, string name)
        {
            Section = body;
            Name = name;

            Subsections = createSubSections();
        }

        public static string GetFileString(string path)
        {
            StreamReader reader = new StreamReader(path, true);
            string temp = reader.ReadToEnd();
            reader.Close();
            return temp;
        }


        #region Find Functions

        #region Row Find Functions
        private string findLabeledLine(string key)
        {
            key = key.ToUpper();

            foreach (string line in Search())
            {
                if (isLabeledLine(line) && GetStringLabel(line) == key)
                {
                    return line;
                }
            }

            return null;
        }

        public string[] FindLabeledLines(string key = null)
        {
            if (key != null) key = key.ToUpper();

            var array = new string[countLabeledLines(key)];
            if (array.Length == 0) return array;
            int index = 0;
            LineType lType;

            foreach (string line in Search())
            {
                lType = GetLineType(line);
                if (lType == LineType.LabeledLine && (key == null || GetStringLabel(line) == key))
                {
                    array[index] = line;
                    index++;
                }
            }
            return array;
        }

        //Strings
        public string FindOptionalString(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) return null;
            return GetLabeledString(temp);
        }

        public string FindRequiredString(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) throw new LabelNotFoundException("Could not find label: " + key);
            return GetLabeledString(temp);
        }

        public string[] FindOptionalStringArray(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) return null;
            return getLabeledStrings(temp);
        }

        public string[] FindRequiredStringArray(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) throw new LabelNotFoundException("Could not find label: " + key);
            return getLabeledStrings(temp);
        }

        //Ints
        public MRFInteger? FindOptionalInt(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) return null;
            return getLabeledInt(temp);
        }

        public MRFInteger FindRequiredInt(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) throw new LabelNotFoundException("Could not find label: " + key);
            return getLabeledInt(temp);
        }

        public MRFInteger[] FindOptionalIntArray(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) return null;
            return getLabeledInts(temp);
        }

        public MRFInteger[] FindRequiredIntArray(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) throw new LabelNotFoundException("Could not find label: " + key);
            return getLabeledInts(temp);
        }

        //Doubles
        public double? FindOptionalDouble(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) return null;
            return getLabeledDouble(temp);
        }

        public double FindRequiredDouble(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) throw new LabelNotFoundException("Could not find label: " + key);
            return getLabeledDouble(temp);
        }

        public double[] FindOptionalDoubleArray(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) return null;
            return getLabeledDoubles(temp);
        }

        public double[] FindRequiredDoubleArray(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) throw new LabelNotFoundException("Could not find label: " + key);
            return getLabeledDoubles(temp);
        }

        //Bools
        public bool? FindOptionalBool(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) return null;
            return getLabeledBool(temp);
        }

        public bool FindRequiredBool(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) throw new LabelNotFoundException("Could not find label: " + key);
            return getLabeledBool(temp);
        }

        public bool[] FindOptionalBoolArray(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) return null;
            return getLabeledBools(temp);
        }

        public bool[] FindRequiredBoolArray(string key)
        {
            string temp = findLabeledLine(key);
            if (temp == null) throw new LabelNotFoundException("Could not find label: " + key);
            return getLabeledBools(temp);
        }
        #endregion

        #region Section find functions

        public FileParser[] FindSections(string key)
        {
            return Subsections.Where(s => s.Name == key.ToUpper()).ToArray();
        }

        public FileParser FindSection(string key)
        {
            return Subsections.Where(s => s.Name == key.ToUpper()).FirstOrDefault();
        }

        private List<FileParser> createSubSections()
        {
            var list = new List<FileParser>();
            string currentName = "";
            string tempSection = "";
            bool inSection = false;
            int depth = 0;

            foreach (string line in Search(true))
            {
                if (isSectionStart(line))
                {
                    depth++;
                    if (depth == 1)
                    {
                        inSection = true;
                        currentName = getSectionKey(line);
                    }
                }
                else if (isSectionEnd(line))
                {
                    depth--;
                    if (inSection && depth == 0)
                    {
                        inSection = false;
                        list.Add(new FileParser(tempSection, currentName));
                        tempSection = "";
                    }

                }

                if (inSection && (depth > 1 || !isSectionStart(line)))
                {
                    tempSection += line + '\n';
                }
            }

            return list;
        }


        #endregion

        #region Data find functions
        public MRFInteger[][] FindIntData(string key)
        {
            var section = FindSection(key);

            //var array = createEmptyArray(section);
            var list = new List<MRFInteger[]>();
            int index = 0;

            foreach (string line in section.Search())
            {
                if (GetLineType(line) == LineType.Data)
                {
                    list.Add(stringsToInts(processDataToArray(line)));
                    //array[index] = stringsToInts(processDataToArray(line));
                    index++;
                }
            }
            return list.ToArray();
        }

        public List<string> FindDataLines()
        {
            List<string> dataLines = new List<string>();

            foreach (string line in Search())
            {
                if (GetLineType(line) == LineType.Data)
                {
                    dataLines.Add(line);
                }
            }
            return dataLines;
        }
        #endregion

        #endregion

        #region Counting Functions
        private int countLabeledLines(string key = null)
        {
            int counter = 0;
            foreach (string line in Search())
            {
                if (isLabeledLine(line) && (key == null || GetStringLabel(line) == key)) counter++;
            }
            return counter;
        }

        private int countSections(string key = null)
        {
            int counter = 0;
            foreach (string line in Search())
            {
                if (isSectionStart(line) && (key == null || getSectionKey(line) == key)) counter++;
            }
            return counter;
        }


        #endregion

        #region Utility Functions

        public IEnumerable<string> Search(bool includeSubsections = false)
        {
            int depth = 0;
            LineType currentLineType;

            StringReader reader = new StringReader(Section);
            string currentLine = reader.ReadLine();

            while (currentLine != null)
            {
                currentLineType = GetLineType(currentLine);

                if (currentLineType == LineType.SectionStart) depth++;
                else if (currentLineType == LineType.SectionEnd) depth--;

                if (depth == 0 || includeSubsections)
                {
                    if (IsFunctionallyRelevant(currentLineType)) yield return currentLine.Trim();
                }

                currentLine = reader.ReadLine();
            }
        }

        public IEnumerable<string> NonCommentLines()
        {
            StringReader reader = new StringReader(Section);
            string currentLine = reader.ReadLine();
            LineType currentLineType;
            while (currentLine != null)
            {
                currentLineType = GetLineType(currentLine);

                if (currentLine != "" && currentLineType != LineType.Comment)
                {
                    yield return currentLine.Trim();
                }
            }
        }
        #endregion

        #region Static Utility Functions

        //Functions used to determine what syntax each line is
        #region Line Types
        private static LineType GetLineType(string line)
        {
            line = line.Trim();
            if (line == null || line == "") return LineType.Nothing;
            if (isComment(line)) return LineType.Comment;
            if (isSectionStart(line)) return LineType.SectionStart;
            if (isSectionEnd(line)) return LineType.SectionEnd;
            if (isLabeledLine(line)) return LineType.LabeledLine;
            if (isData(line)) return LineType.Data;
            else return LineType.Unknown;
        }

        private static bool IsFunctionallyRelevant(LineType type)
        {
            if (type == LineType.Unknown) return false;
            if (type == LineType.Comment) return false;
            if (type == LineType.Nothing) return false;
            return true;
        }

        private static bool isComment(string line)
        {
            if (line.StartsWith(_commentStarter)) return true;
            return false;
        }

        private static bool isSectionStart(string line)
        {
            if (line.Contains(_sectionStart)) return true;
            return false;
        }

        private static bool isSectionEnd(string line)
        {
            if (line.Contains(_sectionEnd)) return true;
            return false;
        }

        private static bool isLabeledLine(string line)
        {
            if (line.Contains(_labelMarker)) return true;
            return false;
        }

        private static bool isData(string line)
        {
            if (line.StartsWith(_dataStart) && line.EndsWith(_dataEnd)) return true;
            return false;
        }

        #endregion

        //Functions used to parse values out of a single line
        #region Get Functions

        //Strings
        public static string GetLabeledString(string line)
        {
            if (line == null) return null;
            return line.Substring(line.IndexOf(_labelMarker) + 1);
        }

        public static string GetStringLabel(string line)
        {
            return line.Substring(0, line.IndexOf(_labelMarker)).ToUpper();
        }

        public static int GetLabeledInt(string line)
        {
            return Int32.Parse(GetLabeledString(line));
        }

        private static string getSectionKey(string line)
        {
            return (line.Substring(0, line.IndexOf(_sectionStart))).Trim().ToUpper();
        }

        private static string[] getLabeledStrings(string line, char delim = _separator)
        {
            return getStrings(GetLabeledString(line), delim);
        }

        private static string[] getStrings(string line, char delim = _separator)
        {
            return getDelimitedStrings(line, delim);
        }

        //Ints
        private static MRFInteger getLabeledInt(string line)
        {
            return new MRFInteger(GetLabeledString(line));
        }

        public static int getIntLabel(string line)
        {
            return Int32.Parse(GetStringLabel(line));
        }

        private static MRFInteger[] getLabeledInts(string line, char delim = _separator)
        {
            return GetIntArray(GetLabeledString(line), delim);
        }

        public static MRFInteger[] GetIntArray(string line, char delim = _separator)
        {
            return stringsToInts(getDelimitedStrings(line, delim));
        }

        //Doubles
        private static double getLabeledDouble(string line)
        {
            return Double.Parse(GetLabeledString(line));
        }

        private static double[] getLabeledDoubles(string line, char delim = _separator)
        {
            return getDoubles(GetLabeledString(line), delim);
        }

        private static double[] getDoubles(string line, char delim = _separator)
        {
            return stringsToDoubles(getDelimitedStrings(line, delim));
        }

        //Bools
        private static bool getLabeledBool(string line)
        {
            return BoolParser(GetLabeledString(line));
        }

        private static bool[] getLabeledBools(string line, char delim = _separator)
        {
            return getBools(GetLabeledString(line), delim);
        }

        private static bool[] getBools(string line, char delim = _separator)
        {
            return stringsToBools(getDelimitedStrings(line, delim));
        }

        private static string[] getDelimitedStrings(string line, char delim)
        {
            int count = line.Count(f => f == delim) + 1;

            if (count == 1) return new string[] { line };
            string[] returnArray = new string[count];

            int start = 0;
            int next = line.IndexOf(delim);

            returnArray[0] = line.Substring(start, next);
            start = next;

            for (int i = 1; i < count; i++)
            {
                next = line.IndexOf(delim, start + 1);
                if (next == -1)
                {
                    returnArray[i] = line.Substring(start + 1);
                }
                else
                {
                    returnArray[i] = line.Substring(start + 1, next - start - 1);
                }

                start = next;
            }
            return returnArray;
        }

        #endregion

        //For extracting values from arrays of multiple types
        #region Extract Functions
        public static string ExtractString(string line, int index, char delim = _separator)
        {
            if (line.Contains(_labelMarker)) line = GetLabeledString(line);
            string[] parts = getStrings(line, delim);
            return parts[index];
        }

        public static MRFInteger ExtractInt(string line, int index, char delim = _separator)
        {
            return new MRFInteger(ExtractString(line, index, delim));
        }

        public static double ExtractDouble(string line, int index, char delim = _separator)
        {
            return Double.Parse(ExtractString(line, index, delim));
        }

        public static bool ExtractBool(string line, int index, char delim = _separator)
        {
            return BoolParser(ExtractString(line, index, delim));
        }
        #endregion

        #endregion

        #region Misc
        private static MRFInteger[] stringsToInts(string[] strings)
        {
            MRFInteger[] returnVal = new MRFInteger[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                returnVal[i] = new MRFInteger(strings[i]);
            }
            return returnVal;
        }

        private static double[] stringsToDoubles(string[] strings)
        {
            double[] returnVal = new double[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                returnVal[i] = Double.Parse(strings[i]);
            }
            return returnVal;
        }

        private static bool[] stringsToBools(string[] strings)
        {
            bool[] returnVal = new bool[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                returnVal[i] = BoolParser(strings[i]);
            }
            return returnVal;
        }

        private static bool BoolParser(string l)
        {
            string line = l.ToUpper();
            if (line == "T" || line == "TRUE" || line == "Y" || line == "YES") return true;
            return false;
        }

        private static int[][] createEmptyArray(FileParser dataSection)
        {
            int count = 0;
            foreach (string line in dataSection.Search())
            {
                if (GetLineType(line) == LineType.Data)
                {
                    count++;
                }
            }
            return new int[count][];
        }

        public static string[] processDataToArray(string line)
        {
            return getStrings(line.Substring(1, line.Length - 2));
        }
        #endregion
    }

    public class LabelNotFoundException : Exception
    {
        public LabelNotFoundException(string message) : base(message)
        {

        }
    }

    public class ObjectBuildException : Exception
    {
        public ObjectBuildException(string message) : base(message)
        {

        }
    }
}
