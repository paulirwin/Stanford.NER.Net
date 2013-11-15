using Stanford.NER.Net.Ling;
using Stanford.NER.Net.Math;
using Stanford.NER.Net.Support;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public static class StringUtils
    {
        public static readonly String[] EMPTY_STRING_ARRAY = new string[0];
        private static readonly string PROP = @"prop";
        private static readonly string PROPS = @"props";
        private static readonly string PROPERTIES = @"properties";
        private static readonly string ARGS = @"args";
        private static readonly string ARGUMENTS = @"arguments";

        public static bool Find(string str, string regex)
        {
            return Regex.IsMatch(str, regex);
        }

        public static bool ContainsIgnoreCase(ICollection<String> c, string s)
        {
            foreach (string squote in c)
            {
                if (squote.EqualsIgnoreCase(s))
                    return true;
            }

            return false;
        }

        public static bool LookingAt(string str, string regex)
        {
            // TODO: is this correct?
            return Regex.IsMatch(str, "^" + regex);
        }

        public static String[] MapStringToArray(string map)
        {
            Regex r = new Regex(@"[,;]");
            String[] m = r.Split(map);
            int maxIndex = 0;
            String[] keys = new string[m.Length];
            int[] indices = new int[m.Length];
            for (int i = 0; i < m.Length; i++)
            {
                int index = m[i].LastIndexOf('=');
                keys[i] = m[i].Substring(0, index);
                indices[i] = int.Parse(m[i].Substring(index + 1));
                if (indices[i] > maxIndex)
                {
                    maxIndex = indices[i];
                }
            }

            String[] mapArr = new string[maxIndex + 1];
            Arrays.Fill(mapArr, null);
            for (int i = 0; i < m.Length; i++)
            {
                mapArr[indices[i]] = keys[i];
            }

            return mapArr;
        }

        public static IDictionary<String, String> MapStringToMap(string map)
        {
            Regex r = new Regex(@"[,;]");
            String[] m = r.Split(map);
            IDictionary<String, String> res = new HashMap<string, string>();
            foreach (string str in m)
            {
                int index = str.LastIndexOf('=');
                string key = str.Substring(0, index);
                string val = str.Substring(index + 1);
                res.Put(key.Trim(), val.Trim());
            }

            return res;
        }

        public static List<Regex> RegexesToPatterns(IEnumerable<String> regexes)
        {
            List<Regex> patterns = new List<Regex>();
            foreach (string regex in regexes)
            {
                patterns.Add(new Regex(regex, RegexOptions.Compiled));
            }

            return patterns;
        }

        public static List<String> RegexGroups(Regex regex, string str)
        {
            if (str == null)
            {
                return null;
            }

            var matcher = regex.Match(str);
            if (!matcher.Success)
            {
                return null;
            }

            List<String> groups = new List<String>();
            for (int index = 1; index <= matcher.Groups.Count; index++)
            {
                groups.Add(matcher.Groups[index].Value);
            }

            return groups;
        }

        public static bool Matches(string str, string regex)
        {
            return Regex.IsMatch(str, "^" + regex + "$");
        }

        public static ISet<String> StringToSet(string str, string delimiter)
        {
            ISet<String> ret = null;
            if (str != null)
            {
                var r = new Regex(delimiter);
                String[] fields = r.Split(str);
                ret = new HashSet<string>();
                foreach (string field in fields)
                {
                    var field2 = field.Trim();
                    ret.Add(field2);
                }
            }

            return ret;
        }

        public static string JoinWords(IEnumerable<IHasWord> l, string glue)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (IHasWord o in l)
            {
                if (!first)
                {
                    sb.Append(glue);
                }
                else
                {
                    first = false;
                }

                sb.Append(o.Word());
            }

            return sb.ToString();
        }

        public static string Join<E>(IList<E> l, string glue, IFunction<E, String> toStringFunc, int start, int end)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            start = System.Math.Max(start, 0);
            end = System.Math.Min(end, l.Size());
            for (int i = start; i < end; i++)
            {
                if (!first)
                {
                    sb.Append(glue);
                }
                else
                {
                    first = false;
                }

                sb.Append(toStringFunc.Apply(l.Get(i)));
            }

            return sb.ToString();
        }

        public static string JoinWords(IList<IHasWord> l, string glue, int start, int end)
        {
            return Join(l, glue, new AnonymousFunction(), start, end);
        }

        private sealed class AnonymousFunction : IFunction<IHasWord, string>
        {
            public string Apply(IHasWord in_renamed)
            {
                return in_renamed.Word();
            }
        }

        public static string JoinWithOriginalWhiteSpace(IList<CoreLabel> tokens)
        {
            if (tokens.IsEmpty())
            {
                return @"";
            }

            CoreLabel lastToken = tokens.Get(0);
            StringBuilder buffer = new StringBuilder(lastToken.Word());
            for (int i = 1; i < tokens.Size(); i++)
            {
                CoreLabel currentToken = tokens.Get(i);
                int numSpaces = currentToken.BeginPosition() - lastToken.EndPosition();
                if (numSpaces < 0)
                {
                    numSpaces = 0;
                }

                buffer.Append(Repeat(' ', numSpaces)).Append(currentToken.Word());
                lastToken = currentToken;
            }

            return buffer.ToString();
        }

        public static string Join<X>(IEnumerable<X> l, string glue)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (X o in l)
            {
                if (!first)
                {
                    sb.Append(glue);
                }
                else
                {
                    first = false;
                }

                sb.Append(o);
            }

            return sb.ToString();
        }

        public static string Join(Object[] elements, string glue)
        {
            return (Join(elements, glue));
        }

        public static string Join(IEnumerable l)
        {
            return Join(l.Cast<object>().ToArray(), @" ");
        }

        public static string Join(Object[] elements)
        {
            return (Join(elements, @" "));
        }

        public static IList<String> Split(string s)
        {
            return Split(s, @"\\s+");
        }

        public static IList<String> Split(string str, string regex)
        {
            var r = new Regex(regex);
            return r.Split(str);
        }

        public static List<String> ValueSplit(string str, string valueRegex, string separatorRegex)
        {
            Regex vPat = new Regex(valueRegex, RegexOptions.Compiled);
            Regex sPat = new Regex(separatorRegex, RegexOptions.Compiled);
            List<String> ret = new List<String>();
            while (str.Length > 0)
            {
                var vm = vPat.Match(str);
                if (vm.Success && vm.Index == 0)
                {
                    // TODO: validate this logic
                    ret.Add(vm.Groups[0].Value);
                    str = str.Substring(vm.Length);
                }
                else
                {
                    throw new ArgumentException(@"valueSplit: " + valueRegex + @" doesn't match " + str);
                }

                if (str.Length > 0)
                {
                    var sm = sPat.Match(str);
                    if (sm.Success && sm.Index == 0)
                    {
                        str = str.Substring(sm.Length);
                    }
                    else
                    {
                        throw new ArgumentException(@"valueSplit: " + separatorRegex + @" doesn't match " + str);
                    }
                }
            }

            return ret;
        }

        public static string Pad(string str, int totalChars)
        {
            if (str == null)
            {
                str = @"null";
            }

            int slen = str.Length;
            StringBuilder sb = new StringBuilder(str);
            for (int i = 0; i < totalChars - slen; i++)
            {
                sb.Append(' ');
            }

            return sb.ToString();
        }

        public static string Pad(Object obj, int totalChars)
        {
            return Pad(obj.ToString(), totalChars);
        }

        public static string PadOrTrim(string str, int num)
        {
            if (str == null)
            {
                str = @"null";
            }

            int leng = str.Length;
            if (leng < num)
            {
                StringBuilder sb = new StringBuilder(str);
                for (int i = 0; i < num - leng; i++)
                {
                    sb.Append(' ');
                }

                return sb.ToString();
            }
            else if (leng > num)
            {
                return str.Substring(0, num);
            }
            else
            {
                return str;
            }
        }

        public static string PadLeftOrTrim(string str, int num)
        {
            if (str == null)
            {
                str = @"null";
            }

            int leng = str.Length;
            if (leng < num)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < num - leng; i++)
                {
                    sb.Append(' ');
                }

                sb.Append(str);
                return sb.ToString();
            }
            else if (leng > num)
            {
                return str.Substring(str.Length - num);
            }
            else
            {
                return str;
            }
        }

        public static string PadOrTrim(Object obj, int totalChars)
        {
            return PadOrTrim(obj.ToString(), totalChars);
        }

        public static string PadLeft(string str, int totalChars, char ch)
        {
            if (str == null)
            {
                str = @"null";
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0, num = totalChars - str.Length; i < num; i++)
            {
                sb.Append(ch);
            }

            sb.Append(str);
            return sb.ToString();
        }

        public static string PadLeft(string str, int totalChars)
        {
            return PadLeft(str, totalChars, ' ');
        }

        public static string PadLeft(Object obj, int totalChars)
        {
            return PadLeft(obj.ToString(), totalChars);
        }

        public static string PadLeft(int i, int totalChars)
        {
            return PadLeft(i.ToString(), totalChars);
        }

        public static string PadLeft(double d, int totalChars)
        {
            return PadLeft(d.ToString(), totalChars);
        }

        public static string Trim(string s, int maxWidth)
        {
            if (s.Length <= maxWidth)
            {
                return (s);
            }

            return (s.Substring(0, maxWidth));
        }

        public static string Trim(Object obj, int maxWidth)
        {
            return Trim(obj.ToString(), maxWidth);
        }

        public static string Repeat(string s, int times)
        {
            if (times == 0)
            {
                return @"";
            }

            StringBuilder sb = new StringBuilder(times * s.Length);
            for (int i = 0; i < times; i++)
            {
                sb.Append(s);
            }

            return sb.ToString();
        }

        public static string Repeat(char ch, int times)
        {
            if (times == 0)
            {
                return @"";
            }

            StringBuilder sb = new StringBuilder(times);
            for (int i = 0; i < times; i++)
            {
                sb.Append(ch);
            }

            return sb.ToString();
        }

        public static string FileNameClean(string s)
        {
            char[] chars = s.ToCharArray();
            StringBuilder sb = new StringBuilder();
            foreach (char c in chars)
            {
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c == '_'))
                {
                    sb.Append(c);
                }
                else
                {
                    if (c == ' ' || c == '-')
                    {
                        sb.Append('_');
                    }
                    else
                    {
                        sb.Append('x').Append((int)c).Append('x');
                    }
                }
            }

            return sb.ToString();
        }

        public static int NthIndex(string s, char ch, int n)
        {
            int index = 0;
            for (int i = 0; i < n; i++)
            {
                if (index == s.Length - 1)
                {
                    return -1;
                }

                index = s.IndexOf(ch, index + 1);
                if (index == -1)
                {
                    return (-1);
                }
            }

            return index;
        }

        public static string Truncate(int n, int smallestDigit, int biggestDigit)
        {
            int numDigits = biggestDigit - smallestDigit + 1;
            char[] result = new char[numDigits];
            for (int j = 1; j < smallestDigit; j++)
            {
                n = n / 10;
            }

            for (int j = numDigits - 1; j >= 0; j--)
            {
                result[j] = Character.ForDigit(n % 10, 10);
                n = n / 10;
            }

            return new string(result);
        }

        public static IDictionary<String, String[]> ArgsToMap(String[] args)
        {
            return ArgsToMap(args, Collections.EmptyMap<string, int>());
        }

        public static IDictionary<String, String[]> ArgsToMap(String[] args, IDictionary<String, int> flagsToNumArgs)
        {
            IDictionary<String, String[]> result = new HashMap<string, string[]>();
            List<String> remainingArgs = new List<String>();
            for (int i = 0; i < args.Length; i++)
            {
                string key = args[i];
                if (key[0] == '-')
                {
                    int numFlagArgs = flagsToNumArgs.Get(key);
                    int max = numFlagArgs == null ? 1 : numFlagArgs;
                    int min = numFlagArgs == null ? 0 : numFlagArgs;
                    List<String> flagArgs = new List<String>();
                    for (int j = 0; j < max && i + 1 < args.Length && (j < min || args[i + 1].Length == 0 || args[i + 1][0] != '-'); i++, j++)
                    {
                        flagArgs.Add(args[i + 1]);
                    }

                    if (result.ContainsKey(key))
                    {
                        String[] newFlagArg = new string[result.Get(key).Length + flagsToNumArgs.Get(key)];
                        int oldNumArgs = result.Get(key).Length;
                        Array.Copy(result.Get(key), 0, newFlagArg, 0, oldNumArgs);
                        for (int j = 0; j < flagArgs.Size(); j++)
                        {
                            newFlagArg[j + oldNumArgs] = flagArgs.Get(j);
                        }

                        result.Put(key, newFlagArg);
                    }
                    else
                    {
                        result.Put(key, flagArgs.ToArray());
                    }
                }
                else
                {
                    remainingArgs.Add(args[i]);
                }
            }

            result.Put(null, remainingArgs.ToArray());
            return result;
        }

        public static Properties ArgsToProperties(String[] args)
        {
            return ArgsToProperties(args, Collections.EmptyMap<string, int>());
        }

        public static Properties ArgsToProperties(String[] args, IDictionary<String, int> flagsToNumArgs)
        {
            Properties result = new Properties();
            List<String> remainingArgs = new List<String>();
            for (int i = 0; i < args.Length; i++)
            {
                string key = args[i];
                if (key.Length > 0 && key[0] == '-')
                {
                    if (key.Length > 1 && key[1] == '-')
                        key = key.Substring(2);
                    else
                        key = key.Substring(1);
                    int maxFlagArgs = flagsToNumArgs.Get(key);
                    int max = maxFlagArgs == null ? 1 : maxFlagArgs;
                    int min = maxFlagArgs == null ? 0 : maxFlagArgs;
                    List<String> flagArgs = new List<String>();
                    for (int j = 0; j < max && i + 1 < args.Length && (j < min || args[i + 1].Length == 0 || args[i + 1][0] != '-'); i++, j++)
                    {
                        flagArgs.Add(args[i + 1]);
                    }

                    if (flagArgs.IsEmpty())
                    {
                        result.SetProperty(key, @"true");
                    }
                    else
                    {
                        result.SetProperty(key, Join(flagArgs, @" "));
                        if (key.EqualsIgnoreCase(PROP) || key.EqualsIgnoreCase(PROPS) || key.EqualsIgnoreCase(PROPERTIES) || key.EqualsIgnoreCase(ARGUMENTS) || key.EqualsIgnoreCase(ARGS))
                        {
                            try
                            {
                                using (Stream is_renamed = new BufferedStream(new FileStream(result.GetProperty(key), FileMode.Open, FileAccess.Read)))
                                using (StreamReader reader = new StreamReader(is_renamed, Encoding.GetEncoding("utf-8")))
                                {
                                    result.Remove(key);
                                    result.Load(reader);
                                    foreach (Object propKey in result.Keys)
                                    {
                                        string newVal = result.GetProperty((string)propKey);
                                        result.SetProperty((string)propKey, newVal.Trim());
                                    }
                                }
                            }
                            catch (IOException e)
                            {
                                result.Remove(key);
                                Console.Error.WriteLine(@"argsToProperties could not read properties file: " + result.GetProperty(key));
                                throw;
                            }
                        }
                    }
                }
                else
                {
                    remainingArgs.Add(args[i]);
                }
            }

            if (!remainingArgs.IsEmpty())
            {
                result.SetProperty(@"", Join(remainingArgs, @" "));
            }

            if (result.ContainsKey(PROP))
            {
                string file = result.GetProperty(PROP);
                result.Remove(PROP);
                Properties toAdd = ArgsToProperties(new string[] { "-prop", file });
                for (Enumeration e = toAdd.propertyNames(); e.HasMoreElements(); )
                {
                    string key = (string)e.NextElement();
                    string val = toAdd.GetProperty(key);
                    if (!result.ContainsKey(key))
                    {
                        result.SetProperty(key, val);
                    }
                }
            }

            return result;
        }

        public static Properties PropFileToProperties(string filename)
        {
            Properties result = new Properties();
            try
            {
                using (Stream is_renamed = new BufferedStream(new FileStream(filename, FileMode.Open, FileAccess.Read)))
                {
                    result.Load(is_renamed);
                    foreach (Object propKey in result.KeySet())
                    {
                        string newVal = result.GetProperty((string)propKey);
                        result.SetProperty((string)propKey, newVal.Trim());
                    }
                }
                return result;
            }
            catch (IOException e)
            {
                throw new IOException(@"propFileToProperties could not read properties file: " + filename, e);
            }
        }

        public static Properties StringToProperties(string str)
        {
            Properties result = new Properties();
            return StringToProperties(str, result);
        }

        public static Properties StringToProperties(string str, Properties props)
        {
            Regex r = new Regex(",\\s*");
            String[] propsStr = r.Split(str.Trim());
            foreach (string term in propsStr)
            {
                int divLoc = term.IndexOf('=');
                string key;
                string value;
                if (divLoc >= 0)
                {
                    key = term.Substring(0, divLoc).Trim();
                    value = term.Substring(divLoc + 1).Trim();
                }
                else
                {
                    key = term.Trim();
                    value = @"true";
                }

                props.SetProperty(key, value);
            }

            return props;
        }

        public static string CheckRequiredProperties(Properties props, params string[] requiredProps)
        {
            foreach (string required in requiredProps)
            {
                if (props.GetProperty(required) == null)
                {
                    return required;
                }
            }

            return null;
        }

        public static void PrintToFile(FileInfo file, string message, bool append, bool printLn, string encoding)
        {
            TextWriter pw = null;
            try
            {
                TextWriter fw;
                if (encoding != null)
                {
                    fw = new StreamWriter(new FileStream(file.FullName, append ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write), Encoding.GetEncoding(encoding));
                }
                else
                {
                    fw = new StreamWriter(new FileStream(file.FullName, append ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write));
                }

                pw = fw;
                if (printLn)
                {
                    pw.WriteLine(message);
                }
                else
                {
                    pw.Write(message);
                }
            }
            catch (Exception)
            {
                Console.Error.WriteLine(@"Exception: in printToFile " + file.FullName);
                //e.PrintStackTrace();
            }
            finally
            {
                if (pw != null)
                {
                    pw.Flush();
                    pw.Close();
                }
            }
        }

        public static void PrintToFileLn(FileInfo file, string message, bool append)
        {
            TextWriter pw = null;
            try
            {
                pw = new StreamWriter(new FileStream(file.FullName, append ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write));
                pw.WriteLine(message);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(@"Exception: in printToFileLn " + file.FullName + ' ' + message);
                //e.PrintStackTrace();
            }
            finally
            {
                if (pw != null)
                {
                    pw.Flush();
                    pw.Close();
                }
            }
        }

        public static void PrintToFile(FileInfo file, string message, bool append)
        {
            TextWriter pw = null;
            try
            {
                pw = new StreamWriter(new FileStream(file.FullName, append ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write));
                pw.Write(message);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(@"Exception: in printToFile " + file.FullName);
                //e.PrintStackTrace();
            }
            finally
            {
                if (pw != null)
                {
                    pw.Flush();
                    pw.Close();
                }
            }
        }

        public static void PrintToFile(FileInfo file, string message)
        {
            PrintToFile(file, message, false);
        }

        public static void PrintToFile(string filename, string message, bool append)
        {
            PrintToFile(new FileInfo(filename), message, append);
        }

        public static void PrintToFileLn(string filename, string message, bool append)
        {
            PrintToFileLn(new FileInfo(filename), message, append);
        }

        public static void PrintToFile(string filename, string message)
        {
            PrintToFile(new FileInfo(filename), message, false);
        }

        public static IDictionary<String, String> ParseCommandLineArguments(String[] args)
        {
            return new HashMap<string, string>(ParseCommandLineArguments(args, false).ToDictionary(i => i.Key, i => (i.Value == null ? null : i.Value.ToString())));
        }

        public static IDictionary<String, Object> ParseCommandLineArguments(String[] args, bool parseNumbers)
        {
            IDictionary<String, Object> result = new HashMap<string, object>();
            for (int i = 0; i < args.Length; i++)
            {
                string key = args[i];
                if (key[0] == '-')
                {
                    if (i + 1 < args.Length)
                    {
                        string value = args[i + 1];
                        if (value[0] != '-')
                        {
                            if (parseNumbers)
                            {
                                Object numericValue = value;
                                try
                                {
                                    numericValue = Double.Parse(value);
                                }
                                catch (FormatException e2)
                                {
                                }

                                result.Put(key, numericValue);
                            }
                            else
                            {
                                result.Put(key, value);
                            }

                            i++;
                        }
                        else
                        {
                            result.Put(key, null);
                        }
                    }
                    else
                    {
                        result.Put(key, null);
                    }
                }
            }

            return result;
        }

        public static string StripNonAlphaNumerics(string orig)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < orig.Length; i++)
            {
                char c = orig[i];
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string StripSGML(string orig)
        {
            Regex sgmlPattern = new Regex(@"<.*?>", RegexOptions.Singleline);
            
            return sgmlPattern.Replace(orig, "");
        }

        public static void PrintStringOneCharPerLine(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                int c = s[i];
                Console.Out.WriteLine(c + @" \'" + (char)c + @"\' ");
            }
        }

        public static string EscapeString(string s, char[] charsToEscape, char escapeChar)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == escapeChar)
                {
                    result.Append(escapeChar);
                }
                else
                {
                    foreach (char charToEscape in charsToEscape)
                    {
                        if (c == charToEscape)
                        {
                            result.Append(escapeChar);
                            break;
                        }
                    }
                }

                result.Append(c);
            }

            return result.ToString();
        }

        public static String[] SplitOnCharWithQuoting(string s, char splitChar, char quoteChar, char escapeChar)
        {
            List<String> result = new List<String>();
            int i = 0;
            int length = s.Length;
            StringBuilder b = new StringBuilder();
            while (i < length)
            {
                char curr = s[i];
                if (curr == splitChar)
                {
                    if (b.Length > 0)
                    {
                        result.Add(b.ToString());
                        b = new StringBuilder();
                    }

                    i++;
                }
                else if (curr == quoteChar)
                {
                    i++;
                    while (i < length)
                    {
                        curr = s[i];
                        if ((curr == escapeChar) && (i + 1 < length) && (s[i + 1] == quoteChar))
                        {
                            b.Append(s[i + 1]);
                            i += 2;
                        }
                        else if (curr == quoteChar)
                        {
                            i++;
                            break;
                        }
                        else
                        {
                            b.Append(s[i]);
                            i++;
                        }
                    }
                }
                else
                {
                    b.Append(curr);
                    i++;
                }
            }

            if (b.Length > 0)
            {
                result.Add(b.ToString());
            }

            return result.ToArray();
        }

        public static int LongestCommonSubstring(string s, string t)
        {
            int[][] d;
            int n;
            int m;
            int i;
            int j;
            n = s.Length;
            m = t.Length;
            if (n == 0)
            {
                return 0;
            }

            if (m == 0)
            {
                return 0;
            }

            d = new int[n + 1][];
            
            for (i = 0; i <= n; i++)
            {
                d[i] = new int[m + 1];
                d[i][0] = 0;
            }

            for (j = 0; j <= m; j++)
            {
                d[0][j] = 0;
            }

            for (i = 1; i <= n; i++)
            {
                char s_i = s[i - 1];
                for (j = 1; j <= m; j++)
                {
                    char t_j = t[j - 1];
                    if (s_i == t_j)
                    {
                        d[i][j] = SloppyMath.Max(d[i - 1][j], d[i][j - 1], d[i - 1][j - 1] + 1);
                    }
                    else
                    {
                        d[i][j] = System.Math.Max(d[i - 1][j], d[i][j - 1]);
                    }
                }
            }

            return d[n][m];
        }

        public static int LongestCommonContiguousSubstring(string s, string t)
        {
            if (s.IsEmpty() || t.IsEmpty())
            {
                return 0;
            }

            int M = s.Length;
            int N = t.Length;
            int[][] d = new int[M + 1][];

            for (int i = 0; i <= M; i++)
            {
                d[i] = new int[N + 1];
            }

            for (int j = 0; j <= N; j++)
            {
                d[0][j] = 0;
            }

            for (int i = 0; i <= M; i++)
            {
                d[i][0] = 0;
            }

            int max = 0;
            for (int i = 1; i <= M; i++)
            {
                for (int j = 1; j <= N; j++)
                {
                    if (s[i - 1] == t[j - 1])
                    {
                        d[i][j] = d[i - 1][j - 1] + 1;
                    }
                    else
                    {
                        d[i][j] = 0;
                    }

                    if (d[i][j] > max)
                    {
                        max = d[i][j];
                    }
                }
            }

            return max;
        }

        public static int EditDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            int[][] d = new int[n + 1][];
            for (int i = 0; i <= n; i++)
            {
                d[i] = new int[m + 1];
                d[i][0] = i;
            }

            for (int j = 0; j <= m; j++)
            {
                d[0][j] = j;
            }

            for (int i = 1; i <= n; i++)
            {
                char s_i = s[i - 1];
                for (int j = 1; j <= m; j++)
                {
                    char t_j = t[j - 1];
                    int cost;
                    if (s_i == t_j)
                    {
                        cost = 0;
                    }
                    else
                    {
                        cost = 1;
                    }

                    d[i][j] = SloppyMath.Min(d[i - 1][j] + 1, d[i][j - 1] + 1, d[i - 1][j - 1] + cost);
                }
            }

            return d[n][m];
        }

        public static string PennPOSToWordnetPOS(string s)
        {
            if (Regex.IsMatch(s, @"NN|NNP|NNS|NNPS"))
            {
                return @"noun";
            }

            if (Regex.IsMatch(s, @"VB|VBD|VBG|VBN|VBZ|VBP|MD"))
            {
                return @"verb";
            }

            if (Regex.IsMatch(s, @"JJ|JJR|JJS|CD"))
            {
                return @"adjective";
            }

            if (Regex.IsMatch(s, @"RB|RBR|RBS|RP|WRB"))
            {
                return @"adverb";
            }

            return null;
        }

        public static string GetShortClassName(Object o)
        {
            if (o == null)
            {
                return @"null";
            }

            string name = o.GetType().Name;
            int index = name.LastIndexOf('.');
            if (index >= 0)
            {
                name = name.Substring(index + 1);
            }

            return name;
        }

        public static T ColumnStringToObject<T>(Type objClass, string str, string delimiterRegex, String[] fieldNames)
        {
            Regex delimiterPattern = Regex.Compile(delimiterRegex);
            return StringUtils.ColumnStringToObject<T>(objClass, str, delimiterPattern, fieldNames);
        }

        public static T ColumnStringToObject<T>(Type objClass, string str, Regex delimiterPattern, String[] fieldNames)
        {
            String[] fields = delimiterPattern.Split(str);
            T item = (T)Activator.CreateInstance(objClass);
            for (int i = 0; i < fields.Length; i++)
            {
                try
                {
                    FieldInfo field = objClass.GetField(fieldNames[i]);
                    field.SetValue(item, fields[i]);
                }
                catch (FieldAccessException ex)
                {
                    MethodInfo method = objClass.GetMethod(@"Set" + StringUtils.Capitalize(fieldNames[i]), new[] { typeof(string) });
                    method.Invoke(item, new object[] { fields[i] });
                }
            }

            return item;
        }

        public static string ObjectToColumnString(Object object_renamed, string delimiter, String[] fieldNames)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string fieldName in fieldNames)
            {
                if (sb.Length > 0)
                {
                    sb.Append(delimiter);
                }

                try
                {
                    FieldInfo field = object_renamed.GetType().GetField(fieldName);
                    sb.Append(field.GetValue(object_renamed));
                }
                catch (FieldAccessException ex)
                {
                    MethodInfo method = object_renamed.GetType().GetMethod(@"Get" + StringUtils.Capitalize(fieldName));
                    sb.Append(method.Invoke(object_renamed, null));
                }
            }

            return sb.ToString();
        }

        public static string Capitalize(string s)
        {
            if (char.IsLower(s[0]))
            {
                return char.ToUpper(s[0]) + s.Substring(1);
            }
            else
            {
                return s;
            }
        }

        public static bool IsCapitalized(string s)
        {
            return (char.IsUpper(s[0]));
        }

        public static string SearchAndReplace(string text, string from, string to)
        {
            from = EscapeString(from, new char[] { '.', '[', ']', '\\' }, '\\');
            Regex p = new Regex(from);
            
            return p.Replace(text, to);
        }

        public static string MakeHTMLTable(String[][] table, String[] rowLabels, String[] colLabels)
        {
            StringBuilder buff = new StringBuilder();
            buff.Append("<table class=\"auto\" border=\"1\" cellspacing=\"0\">\n");
            buff.Append("<tr>\n");
            buff.Append("<td></td>\n");
            for (int j = 0; j < table[0].Length; j++)
            {
                buff.Append("<td class=\"label\">").Append(colLabels[j]).Append("</td>\n");
            }

            buff.Append("</tr>\n");
            for (int i = 0; i < table.Length; i++)
            {
                buff.Append("<tr>\n");
                buff.Append("<td class=\"label\">").Append(rowLabels[i]).Append("</td>\n");
                for (int j = 0; j < table[i].Length; j++)
                {
                    buff.Append("<td class=\"data\">");
                    buff.Append(((table[i][j] != null) ? table[i][j] : ""));
                    buff.Append("</td>\n");
                }

                buff.Append("</tr>\n");
            }

            buff.Append(@"</table>");
            return buff.ToString();
        }

        public static string MakeAsciiTable(Object[][] table, Object[] rowLabels, Object[] colLabels, int padLeft, int padRight, bool tsv)
        {
            StringBuilder buff = new StringBuilder();
            buff.Append(MakeAsciiTableCell(@"", padLeft, padRight, tsv));
            for (int j = 0; j < table[0].Length; j++)
            {
                buff.Append(MakeAsciiTableCell(colLabels[j], padLeft, padRight, (j != table[0].Length - 1) && tsv));
            }

            buff.Append('\n');
            for (int i = 0; i < table.Length; i++)
            {
                buff.Append(MakeAsciiTableCell(rowLabels[i], padLeft, padRight, tsv));
                for (int j = 0; j < table[i].Length; j++)
                {
                    buff.Append(MakeAsciiTableCell(table[i][j], padLeft, padRight, (j != table[0].Length - 1) && tsv));
                }

                buff.Append('\n');
            }

            return buff.ToString();
        }

        private static string MakeAsciiTableCell(Object obj, int padLeft, int padRight, bool tsv)
        {
            string result = obj.ToString();
            if (padLeft > 0)
            {
                result = PadLeft(result, padLeft);
            }

            if (padRight > 0)
            {
                result = Pad(result, padRight);
            }

            if (tsv)
            {
                result = result + '\t';
            }

            return result;
        }

        public static void Main(String[] args)
        {
            String[] s = new[]
        {
        @"there once was a man", @"this one is a manic", @"hey there", @"there once was a mane", @"once in a manger.", @"where is one match?", @"Jo3seph Smarr!", @"Joseph R Smarr"
        }

            ;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.Out.WriteLine(@"s1: " + s[i]);
                    Console.Out.WriteLine(@"s2: " + s[j]);
                    Console.Out.WriteLine(@"edit distance: " + EditDistance(s[i], s[j]));
                    Console.Out.WriteLine(@"LCS:           " + LongestCommonSubstring(s[i], s[j]));
                    Console.Out.WriteLine(@"LCCS:          " + LongestCommonContiguousSubstring(s[i], s[j]));
                    Console.Out.WriteLine();
                }
            }
        }

        public static string ToAscii(string s)
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c > 127)
                {
                    string result = @"?";
                    if (c >= 0x00c0 && c <= 0x00c5)
                    {
                        result = @"A";
                    }
                    else if (c == 0x00c6)
                    {
                        result = @"AE";
                    }
                    else if (c == 0x00c7)
                    {
                        result = @"C";
                    }
                    else if (c >= 0x00c8 && c <= 0x00cb)
                    {
                        result = @"E";
                    }
                    else if (c >= 0x00cc && c <= 0x00cf)
                    {
                        result = @"F";
                    }
                    else if (c == 0x00d0)
                    {
                        result = @"D";
                    }
                    else if (c == 0x00d1)
                    {
                        result = @"N";
                    }
                    else if (c >= 0x00d2 && c <= 0x00d6)
                    {
                        result = @"O";
                    }
                    else if (c == 0x00d7)
                    {
                        result = @"x";
                    }
                    else if (c == 0x00d8)
                    {
                        result = @"O";
                    }
                    else if (c >= 0x00d9 && c <= 0x00dc)
                    {
                        result = @"U";
                    }
                    else if (c == 0x00dd)
                    {
                        result = @"Y";
                    }
                    else if (c >= 0x00e0 && c <= 0x00e5)
                    {
                        result = @"a";
                    }
                    else if (c == 0x00e6)
                    {
                        result = @"ae";
                    }
                    else if (c == 0x00e7)
                    {
                        result = @"c";
                    }
                    else if (c >= 0x00e8 && c <= 0x00eb)
                    {
                        result = @"e";
                    }
                    else if (c >= 0x00ec && c <= 0x00ef)
                    {
                        result = @"i";
                    }
                    else if (c == 0x00f1)
                    {
                        result = @"n";
                    }
                    else if (c >= 0x00f2 && c <= 0x00f8)
                    {
                        result = @"o";
                    }
                    else if (c >= 0x00f9 && c <= 0x00fc)
                    {
                        result = @"u";
                    }
                    else if (c >= 0x00fd && c <= 0x00ff)
                    {
                        result = @"y";
                    }
                    else if (c >= 0x2018 && c <= 0x2019)
                    {
                        result = @"\'";
                    }
                    else if (c >= 0x201c && c <= 0x201e)
                    {
                        result = @"\";
                    }
                    else if (c >= 0x0213 && c <= 0x2014)
                    {
                        result = @"-";
                    }
                    else if (c >= 0x00A2 && c <= 0x00A5)
                    {
                        result = @"$";
                    }
                    else if (c == 0x2026)
                    {
                        result = @".";
                    }

                    b.Append(result);
                }
                else
                {
                    b.Append(c);
                }
            }

            return b.ToString();
        }

        public static string ToCSVString(String[] fields)
        {
            StringBuilder b = new StringBuilder();
            foreach (string fld in fields)
            {
                if (b.Length > 0)
                {
                    b.Append(',');
                }

                string field = EscapeString(fld, new char[] { '\"' }, '\"');
                b.Append('\"').Append(field).Append('\"');
            }

            return b.ToString();
        }

        public static string Tr(string input, string from, string to)
        {
            StringBuilder sb = null;
            int len = input.Length;
            for (int i = 0; i < len; i++)
            {
                int ind = from.IndexOf(input[i]);
                if (ind >= 0)
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder(input);
                    }

                    sb[i] = to[ind];
                }
            }

            if (sb == null)
            {
                return input;
            }
            else
            {
                return sb.ToString();
            }
        }

        public static string Chomp(string s)
        {
            if (s.Length == 0)
                return s;
            int l_1 = s.Length - 1;
            if (s[l_1] == '\n')
            {
                return s.Substring(0, l_1);
            }

            return s;
        }

        public static string Chomp(Object o)
        {
            return Chomp(o.ToString());
        }

        public static void PrintErrInvocationString(string cls, String[] args)
        {
            Console.Error.WriteLine(ToInvocationString(cls, args));
        }

        public static string ToInvocationString(string cls, String[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(cls).Append(@" invoked on ").Append(DateTime.UtcNow);
            sb.Append(@" with arguments:\n  ");
            foreach (string arg in args)
            {
                sb.Append(' ').Append(arg);
            }

            return sb.ToString();
        }

        public static string GetBaseName(string fileName)
        {
            return GetBaseName(fileName, @"");
        }

        public static string GetBaseName(string fileName, string suffix)
        {
            String[] elts = fileName.Split(new[] { '/' });
            string lastElt = elts[elts.Length - 1];
            if (lastElt.EndsWith(suffix))
            {
                lastElt = lastElt.Substring(0, lastElt.Length - suffix.Length);
            }

            return lastElt;
        }

        public static bool IsAlpha(string s)
        {
            Regex p = new Regex(@"^[\\p{Alpha}\\s]+$");

            return p.IsMatch(s);
        }

        public static bool IsNumeric(string s)
        {
            Regex p = new Regex(@"^[\\p{Digit}\\s\\.]+$");

            return p.IsMatch(s);
        }

        public static bool IsAlphanumeric(string s)
        {
            Regex p = Regex.Compile(@"^[\\p{Alnum}\\s\\.]+$");
            
            return m.Matches();
        }

        public static bool IsPunct(string s)
        {
            Regex p = new Regex(@"^[\\p{Punct}]+$");

            return p.IsMatch(s);
        }

        public static bool IsAcronym(string s)
        {
            Regex p = new Regex(@"^[\\p{Upper}]+$");

            return p.IsMatch(s);
        }

        public static string GetNotNullString(string s)
        {
            if (s == null)
                return @"";
            else
                return s;
        }

        public static string ResolveVars(string str, IDictionary<string, string> props)
        {
            if (str == null)
                return null;
            Regex p = Regex.Compile(@"\\$\\{(\\w+)\\}|\\$(\\w+)");
            StringBuilder sb = new StringBuilder();

            var matches = p.Matches(str);

            foreach (Match m in matches)
            {
                string varName = m.Groups[1].Value.IsEmpty() ? m.Groups[2].Value : m.Groups[1].Value;
                string vrValue;
                if (props.ContainsKey(varName))
                {
                    vrValue = (string)props.Get(varName);
                }
                else
                {
                    vrValue = Environment.GetEnvironmentVariable(varName);
                }

                // TODO: this is probably wrong
                sb.Append(p.Replace(m.Value, null == vrValue ? @"" : vrValue));
            }

            sb.Append(str.Substring(matches[matches.Count - 1].Index + matches[matches.Count - 1].Length));
            return sb.ToString();
        }

        public static Properties ArgsToPropertiesWithResolve(String[] args)
        {
            SortedDictionary<String, String> result = new SortedDictionary<String, String>();
            IDictionary<String, String> existingArgs = new SortedDictionary<String, String>();
            for (int i = 0; i < args.Length; i++)
            {
                string key = args[i];
                if (key.Length > 0 && key[0] == '-')
                {
                    if (key.Length > 1 && key[1] == '-')
                        key = key.Substring(2);
                    else
                        key = key.Substring(1);
                    int max = 1;
                    int min = 0;
                    List<String> flagArgs = new List<String>();
                    for (int j = 0; j < max && i + 1 < args.Length && (j < min || args[i + 1].Length == 0 || args[i + 1][0] != '-'); i++, j++)
                    {
                        flagArgs.Add(args[i + 1]);
                    }

                    if (flagArgs.IsEmpty())
                    {
                        existingArgs.Put(key, @"true");
                    }
                    else
                    {
                        if (key.EqualsIgnoreCase(PROP) || key.EqualsIgnoreCase(PROPS) || key.EqualsIgnoreCase(PROPERTIES) || key.EqualsIgnoreCase(ARGUMENTS) || key.EqualsIgnoreCase(ARGS))
                        {
                            result.PutAll(PropFileToTreeMap(Join(flagArgs, @" "), existingArgs));
                            i++;
                            existingArgs.Clear();
                        }
                        else
                            existingArgs.Put(key, Join(flagArgs, @" "));
                    }
                }
            }

            result.PutAll(existingArgs);
            foreach (Entry<String, String> o in result.EntrySet())
            {
                string val = ResolveVars(o.GetValue(), result);
                result.Put(o.GetKey(), val);
            }

            Properties props = new Properties();
            props.PutAll(result);
            return props;
        }

        public static SortedDictionary<String, String> PropFileToTreeMap(string filename, IDictionary<String, String> existingArgs)
        {
            SortedDictionary<String, String> result = new SortedDictionary<String, String>();
            result.PutAll(existingArgs);
            foreach (string l in IOUtils.ReadLines(filename))
            {
                l = l.Trim();
                if (l.IsEmpty() || l.StartsWith(@"#"))
                    continue;
                int index = l.IndexOf('=');
                if (index == -1)
                    result.Put(l, @"true");
                else
                    result.Put(l.Substring(0, index).Trim(), l.Substring(index + 1).Trim());
            }

            return result;
        }

        public static ICollection<String> GetNgrams(List<String> words, int minSize, int maxSize)
        {
            List<List<String>> ng = CollectionUtils.GetNGrams(words, minSize, maxSize);
            ICollection<String> ngrams = new List<String>();
            foreach (List<String> n in ng)
                ngrams.Add(StringUtils.Join(n, @" "));
            return ngrams;
        }

        public static ICollection<String> GetNgramsFromTokens(List<CoreLabel> words, int minSize, int maxSize)
        {
            List<String> wordsStr = new List<String>();
            foreach (CoreLabel l in words)
                wordsStr.Add(l.Word());
            List<List<String>> ng = CollectionUtils.GetNGrams(wordsStr, minSize, maxSize);
            ICollection<String> ngrams = new List<String>();
            foreach (List<String> n in ng)
                ngrams.Add(StringUtils.Join(n, @" "));
            return ngrams;
        }

        public static ICollection<String> GetNgramsString(string s, int minSize, int maxSize)
        {
            Regex r = new Regex("\\s+");
            return GetNgrams(r.Split(s), minSize, maxSize);
        }
    }
}
