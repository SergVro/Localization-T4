using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EPiServer.Framework.Localization;
using EPiServer.Framework.Localization.XmlResources;
using Microsoft.VisualStudio.TextTemplating;

namespace EPiServer.Resources
{
    public abstract class Generator : TextTransformation
    {

        //***********************************************************************************************

        private readonly Regex _selectByAttributeRegex = new Regex(
            "(?<nodeName>\\w+)\\[\\s*@\\s*(?<propertyName>\\w+)\\s*=\\s*'" +
            "(?<propertyValue>[^'\\r\\n]+)'\\s*\\]",
            RegexOptions.IgnoreCase
            | RegexOptions.Singleline
            | RegexOptions.ExplicitCapture
            | RegexOptions.CultureInvariant
            | RegexOptions.IgnorePatternWhitespace
            | RegexOptions.Compiled
            );

        private readonly Dictionary<string, string> _nameReplacements = new Dictionary<string, string>
                                                               {
                                                                   {" ","_"},
                                                                   {"-","_"},
                                                                   {".","_"},
                                                                   {",","_"},
                                                                   {";","_"},
                                                                   {"[",""},
                                                                   {"]",""},
                                                                   {"(",""},
                                                                   {")",""}, 
                                                                   {"{",""},
                                                                   {"}",""}
                                                               };

        public void Generate(string folder, string rootNamespace)
        {
            DirectoryInfo di = new DirectoryInfo(folder);
            if (!di.Exists)
            {
                throw new ArgumentException("Directory not found: " + folder);
            }
            var provider = new XmlLocalizationProvider();
            // Load all resources files from directory
            foreach (FileInfo fileInfo in di.GetFiles("*.xml"))
            {
                using (var stream = fileInfo.OpenRead())
                {
                    provider.Load(stream);
                }
            }

            WriteResourceClasses(provider, rootNamespace);
        }

        internal void WriteResourceClasses(LocalizationProvider provider, string rootNamespace)
        {
            var resourceList = new List<Resource>();
            var keyHandler = new ResourceKeyHandler();
            foreach (CultureInfo culture in provider.AvailableLanguages)
            {
                foreach (ResourceItem resource in provider.GetAllStrings("", new string[] { }, culture))
                {
                    string[] normalized = ProcessAttributeSelectors(keyHandler.NormalizeKey(resource.Key));
                    var resourceClass = new Resource
                    {
                        Language = culture.ToString(),
                        Key = resource.Key,
                        Value = resource.Value,
                        NormalizedKey = normalized,
                        Level = normalized.Length,
                        Namespace = GetNamespace(normalized, rootNamespace),
                        ClassName = GetClassName(normalized),
                        PropertyName = GetPropertyName(normalized)
                    };
                    resourceList.Add(resourceClass);
                }
            }

            WriteLine("namespace " + rootNamespace);
            WriteLine("{");
            RenderClasses(resourceList, String.Empty, 0);
            WriteEndBracket(0);
        }


        private string[] ProcessAttributeSelectors(string[] normalized)
        {
            var result = new List<String>();
            foreach (var normValue in normalized)
            {
                if (!normValue.Contains("@"))
                {
                    result.Add(normValue);
                    continue;
                }
                var matches = _selectByAttributeRegex.Matches(normValue);
                if (matches.Count == 0)
                {
                    result.Add(normValue);
                    continue;
                }
                foreach (Match match in matches)
                {
                    var node = match.Groups["nodeName"].Value;
                    var name = match.Groups["propertyName"].Value;
                    var value = match.Groups["propertyValue"].Value;
                    result.Add(GetSafeName(node));
                    result.Add(GetSafeName(value));
                    //System.Diagnostics.Debugger.Launch();
                }
            }
            return result.ToArray();
        }

        private void RenderClasses(IEnumerable<Resource> resources, string parentClassName, int level)
        {
            var resourceGroupig = resources.Where(r => r.NormalizedKey.Length > level)
                .GroupBy(r => r.NormalizedKey[level], r => r).Select(r => new {ContainingClassName = r.Key, Resources = r});
                
            foreach (var resource in resourceGroupig)
            {
                WriteClass(resource.ContainingClassName, parentClassName, level + 1);

                var properties = resource.Resources.Where(r => r.NormalizedKey.Length == level + 2).ToList();
                foreach (var property in properties.GroupBy(p => p.PropertyName).Select(p => new { PropertyName = p.Key, Values = p })) 
                {
                    WriteProperty(property.PropertyName, property.Values.ToList(), level + 2);
                }
                RenderClasses(resource.Resources.Except(properties), resource.ContainingClassName, level + 1);
                WriteEndBracket(level);
            }
        }

        private void WriteProperty(string propertyName, IList<Resource> values, int level)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }
            WriteLineIndent(level, "///<summary>");
            foreach (var resource in values)
            {
                WriteLineIndent(level, string.Format("/// {0}: {1}<br/>",  resource.Language, resource.Value.Replace("\r", " ").Replace("\n", " ")));
            }
            WriteLineIndent(level, "///</summary>");
            if (propertyName.Equals(values.First().ClassName))
            {
                propertyName = "_" + propertyName;
            }
            WriteLineIndent(level, string.Format("public static string @{0}{{ get {{ return LocalizationService.Current.GetString(\"{1}\"); }} }}",
                propertyName, values.First().Key));
        }

        private void WriteLineIndent(int indentLevel, string value)
        {
            WriteLine(String.Format("{0}{1}", Tabs(indentLevel), value));
        }
        private void WriteEndBracket(int level)
        {
            WriteLineIndent(level + 1, "}");
        }

        private void WriteClass(string className, string parentClassName, int level)
        {
            className = GetSafeName(className);
            if (className.Equals(parentClassName))
            {
                className = "_" + className;
            }
            WriteLineIndent(level, string.Format("public static class @{0}",  className));
            WriteLineIndent(level, "{");
        }


        private string GetSafeName(string name)
        {
            _nameReplacements.ToList().ForEach(kv => name = name.Replace(kv.Key, kv.Value));
            return System.Xml.XmlConvert.EncodeName(name);
        }

        private static string Tabs (int count)
        {
            return new String('\t', count);
        }


        internal string GetNamespace(string[] normalizedKey, string rootNamespace)
        {
            if (normalizedKey.Length <= 2)
            {
                return rootNamespace;
            }
            var keyNamespace = String.Join(".", normalizedKey.Reverse().Skip(2).Reverse());
            return keyNamespace;
        }

        internal string GetClassName(string[] normalizedKey)
        {
            if (normalizedKey.Length <= 1)
            {
                return "";
            }
            var keyClassName = normalizedKey.Reverse().Skip(1).First();
            return keyClassName;
        }

        internal string GetPropertyName(string[] normalizedKey)
        {
            var keyPropertyName = normalizedKey.Last();
    
            return keyPropertyName;
        }


        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        static string LowercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }

        internal class Resource
        {
            public string Language { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public string Namespace { get; set; }
            public string ClassName { get; set; }
            public string PropertyName { get; set; }
            public string[] NormalizedKey { get; set; }
            public int Level { get; set; }
        }




        //***********************************************************************************************
    }
}