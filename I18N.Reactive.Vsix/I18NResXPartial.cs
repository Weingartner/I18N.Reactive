using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace I18N.Reactive.Vsix
{
    public partial class I18NResX
    {
        public string Namespace { get; private set; }

        public string ClassName { get; set; }

        public List<I18NHelpers.ResXItem> Items { get; private set; } 

        public I18NResX(string ns, string resxPath)
        {
            Namespace = ns;
            ClassName = I18NHelpers.Current.NormalizeString(Path.GetFileNameWithoutExtension(resxPath));
            Items = new List<I18NHelpers.ResXItem>();
            I18NHelpers.AddResX(string.Empty, ns, resxPath, Items);
        }

    }

    public class I18NHelpers
    {
        #region Singleton
        // http://www.yoda.arachsys.com/csharp/singleton.html (Fourth: Simplified)
        /// <summary>
        /// Public instance to Helpers
        /// </summary>
        public static readonly I18NHelpers Current = new I18NHelpers();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static I18NHelpers() { }

        I18NHelpers()
        {
            // Eventual init code
        }
        #endregion

        /// <summary>
        /// Determine the processing level. Not used yet.
        /// </summary>
        public enum ProcessLevel
        {
            /// <summary>
            /// Process all files from Folder recursive, in which T4ResX.tt resides
            /// </summary>
            Folder,

            /// <summary>
            /// Process all files from Project recursive, in which T4ResX.tt resides
            /// </summary>
            Project
        }

        /// <summary>
        /// Declared type of entry. Not used yet.
        /// </summary>
        [Flags]
        public enum ResxType
        {
            None     = 0,
            Constant = 1
        }

        /// <summary>
        /// Template used for each RESX item discovered
        /// </summary>
        public class ResXItem
        {
            public string NameSpace { get; set; }
            public string ClassName { get; set; }

            public string Key       { get; set; }
            public string Value     { get; set; }
            public string Comment   { get; set; }
            public string Culture   { get; set; }
        }

        /// <summary>
        /// Match files without culture extension
        /// </summary>
        public const string CultureInvariantRegex = @".*\.[a-z]{2}(-[a-z]{2})?\.resx$";

        /// <summary>
        /// Finds tokens in the form of {NAME}
        /// </summary>
        public const string NamedTokenRegex = @"{\b\p{Lu}{3,}\b}";

        /// <summary>
        /// Finds tokens in the form of {0}
        /// </summary>
        public const string ParamTokenRegex = @"{[0-9]{1}}";

        /// <summary>
        /// Finds tokens in the form of {NAME} & {0}
        /// </summary>
        public const string AnyTokenRegex = @"{[0-9]{1}}|{\b\p{Lu}{3,}\b}";

        /// <summary>
        /// Get the declared type of an item
        /// INFO: Currently only constants works. This is open for future ideas.
        /// </summary>
        public ResxType GetType(string value)
        {
            var result = ResxType.None;

            if (Regex.IsMatch(value, @"\[type:constant\]"))
            {
                result |= ResxType.Constant;
            }

            return result;
        }

        /// <summary>
        /// See if the content contains any tokens
        /// </summary>
        public bool HasTokens(string content)
        {
            return Regex.IsMatch(content, AnyTokenRegex);
        }

        ///<summary>
        /// Reformat a string to various forms
        ///</summary>
        public string NormalizeString(string s, bool isClass = true, bool camelCase = true)
        {

            return s.Split('.')
                    .Aggregate((c, n) => NormalizeItem(c, isClass, camelCase) + (isClass ? "." : "_") + NormalizeItem(n, isClass, camelCase));
        }

        /// <summary>
        /// Same as above but single item
        /// </summary>
        public string NormalizeItem(string s, bool isClass = true, bool camelCase = true)
        {
            s = s.Replace(isClass ? "." : "_", "#");

            var r = @"[^\p{L}0-9#]";
            var m = Regex.Matches(s, r);

            foreach (Match match in m)
            {
                if (camelCase)
                {
                    var chars              = s.ToCharArray();
                    chars[match.Index + 1] = char.ToUpper(s[match.Index + 1]);
                    s                      = new string(chars);
                }

                s = s.Remove(match.Index, 1);
                s = s.Insert(match.Index, "_");
            }

            if (Regex.IsMatch(s, @"^[0-9]"))
            {
                s = s.Insert(0, "_");
            }

            return s.Replace("#", isClass ? "." : "_");
        }

        #region ResXLoading
        public static void AddResX(string projectPath, string rootNameSpace, string itemPath, List<ResXItem> items)
        {            
            var culture = CultureInfo.InvariantCulture;
            var file    = System.IO.Path.GetFileNameWithoutExtension(itemPath);
            try
            {
                if (file != null)
                {
                    culture = CultureInfo.CreateSpecificCulture(file.Split('.').Last());
                }
            }
            catch
            {
                culture = CultureInfo.InvariantCulture;
            }

            var xml = new XmlDocument();
            xml.Load(itemPath);

            if (xml.DocumentElement != null)
            {
                var nodes = xml.DocumentElement.SelectNodes("//data");
                if (nodes != null)
                {
                    foreach (XmlElement element in nodes)
                    {
                        var entry = new ResXItem
                                        {
                                            Key       = string.Empty,
                                            Value     = string.Empty,
                                            Comment   = string.Empty,
                                            Culture   = culture.Name
                                        };

                        var elementKey = element.Attributes.GetNamedItem("name");
                        if (elementKey != null)
                        {
                            entry.Key = elementKey.Value ?? string.Empty;
                        }

                        var elementValue = element.SelectSingleNode("value");
                        if (elementValue != null)
                        {
                            entry.Value = elementValue.InnerText;
                        }

                        var elementComment = element.SelectSingleNode("comment");
                        if (elementComment != null)
                        {
                            entry.Comment = elementComment.InnerText;
                        }

                        items.Add(entry);
                    }
                }
            }
        }
        #endregion      
    }
}
