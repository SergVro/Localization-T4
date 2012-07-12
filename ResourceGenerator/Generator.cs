using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using EPiServer.Framework.Localization;
using EPiServer.Framework.Localization.XmlResources;
using Microsoft.VisualStudio.TextTemplating;

namespace EPiServer.Resources
{
    public abstract class Generator : TextTransformation
    {

        //***********************************************************************************************

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
                    string[] normalized = keyHandler.NormalizeKey(resource.Key);
                    var resourceClass = new Resource
                    {
                        Language = culture.ToString(),
                        Key = resource.Key,
                        Value = resource.Value,
                        NormalizedKey = normalized,
                        Namespace = GetNamespace(normalized, rootNamespace),
                        ClassName = GetClassName(normalized),
                        PropertyName = GetPropertyName(normalized)
                    };
                    resourceList.Add(resourceClass);
                }
            }

            WriteLine("namespace " + rootNamespace);
            WriteLine("{");
            RenderClasses(resourceList, 0);
            WriteEndBracket(0);

    
  
        }

        private void RenderClasses(IEnumerable<Resource> resources, int level)
        {
            var resourceGroupig = resources.Where(r => r.NormalizedKey.Length > level)
            .GroupBy(r => r.NormalizedKey[level], r => r).Select(r => new {ContainingClassName = r.Key,  Resources = r})
                .Where(p => !p.ContainingClassName.Contains("[@")); // TODO: fix xpath keys
                
            foreach (var resource in resourceGroupig)
            {

                WriteClass(resource.ContainingClassName, level);

                var properties = resource.Resources.Where(r => r.NormalizedKey.Length == level + 2).ToList();
                foreach (var property in properties.GroupBy(p => p.PropertyName).Select(p => new { PropertyName = p.Key, Values = p })
                        .Where(p => !p.PropertyName.Contains("[@"))) // TODO: fix xpath keys
                {
                    WriteProperty(property.PropertyName, property.Values.ToList(), level);
                }
                RenderClasses(resource.Resources.Except(properties), level + 1);
                WriteEndBracket(level);
            }
        }

        private void WriteProperty(string propertyName, IList<Resource> values, int level)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }
            WriteLine(string.Format("{0}///<summary>", Tabs(level + 1)));
            foreach (var resource in values)
            {
                WriteLine(string.Format("{0}/// {1}: {2}<br/>", Tabs(level + 1), resource.Language, resource.Value));
            }
            WriteLine(string.Format("{0}///</summary>", Tabs(level + 1)));
            if (propertyName == values.First().ClassName)
            {
                propertyName = "_" + propertyName;
            }
            WriteLine(string.Format("{0}public static string @{1}{{ get {{ return LocalizationService.Current.GetString(\"{2}\"); }} }}", Tabs(level + 1), propertyName, values.First().Key));
        }

        private void WriteEndBracket(int level)
        {
            WriteLine(Tabs(level + 1)+"}");
        }

        private void WriteClass(string className, int level)
        {
            className = className.Replace("-", "_");
            WriteLine(string.Format("{0}public static class @{1}", Tabs(level + 1), className));
            WriteLine(string.Format("{0}{{", Tabs(level + 1)));
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
        }




        //***********************************************************************************************
    }
}