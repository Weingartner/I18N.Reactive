﻿/*
 * T4ResX
 * Author  Robert Hoffmann (itechnology)
 * License MIT / http://bit.ly/mit-license
 *
 * Version 1.00
 * https://github.com/itechnology/T4ResX
 */ 
 using System;
 using System.Linq; 
 using System.Threading;
 using System.Reflection;
 using System.Collections.Generic; 
 using System.Text.RegularExpressions;
 using ReactiveUI;
 using Weingartner.I18N;

 namespace Weingarner.I18N.Sample {
    /// <summary>
    /// Class that contains all our little helper functions
    /// </summary>
    public static class Utilities {
        /// <summary>
        /// A fake attribute that allows us to filter classes by Attribute
        /// It's helpfull when using GetResourcesByNameSpace(), and when T4ResX is tossed into a project containing other classes/properties
        /// Like this we only return stuff generated by T4ResX itself
        /// </summary>
        public class Localized : Attribute {}

        ///<summary>
        /// We bind this function to our replacement function when needed
        /// Like this the replacement function can reside in any assembly you like
        /// Bind it once on ApplicationStart, or rebind it to a different replacement function before calling it
        ///
        /// Poor Man's IOC
        /// http://www.i-technology.net
        ///</summary>
        public static Func<string, string> GetReplacementString = key => key;

        #region Methods
        /// <summary>
        /// Look up ressources from a specific namespace
        /// </summary>
        /// <param name="ns">Namspace to get resources from</param>
        /// <returns>Dictionary&lt;namespace, Dictionary&lt;key, value&gt;&gt;</returns>
        public static Dictionary<string, Dictionary<string, string>> GetResourcesByNameSpace(string ns)
        {
            var result = new Dictionary<string, Dictionary<string, string>>();

            var qs = ns.Split('^');
                ns = qs[0];

            var path  = string.IsNullOrEmpty(ns) ? "Weingarner.I18N.Sample" : string.Format("{0}.{1}", "Weingarner.I18N.Sample", ns);
            var wCard = path;

            if (ns.EndsWith(".*"))
            {
                wCard = path.Replace(".*", "");
            }
            
            var current = Assembly.GetExecutingAssembly();
            current
                .GetTypes()
                .Where(type => type.GetCustomAttributes(typeof(Localized), false).Length != 0)
                .Where(type => type.Namespace != null && (ns == "" || (ns.EndsWith(".*")
                                                                           ? type.Namespace.StartsWith(wCard, StringComparison.InvariantCultureIgnoreCase)
                                                                           : string.Equals(type.Namespace, path, StringComparison.InvariantCultureIgnoreCase)))
                 )
                .Where(type => qs.Length != 2 || Regex.IsMatch(type.Name, qs[1], RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline))
                .ToList()
                .ForEach(typeFound =>
                {
                    var instance = current.CreateInstance(typeFound.FullName);
                    if (instance != null)
                    {
                        var instanceType  = instance.GetType();
                        var instanceClass = instanceType.FullName.Replace(wCard, "");
                        var propertyList  = new Dictionary<string, string>();

                        instanceType
                            .GetProperties()
                            .Where(t => t.GetCustomAttributes(typeof(Localized), false).Length != 0)
                            .ToList()
                            .ForEach(property => propertyList.Add(property.Name, property.GetValue(null, null).ToString()));

                        if (propertyList.Count > 0)
                        {
                            result.Add(instanceClass.StartsWith(".") ? instanceClass.Substring(1) : instanceClass, propertyList);
                        }
                    }
                });

            return result;
        }
        #endregion
    }
 }
    
namespace Weingarner.I18N.Sample  {
    [Utilities.Localized]	
    public partial class Branding : ReactiveObject {
    
        public Branding (){
            CultureResources
                .Instance
                .WhenAnyValue(p=>p.CultureInfo)
// ReSharper disable once ExplicitCallerInfoArgument
                .Subscribe(c=>this.RaisePropertyChanged(""));
        }
        ///<summary>
        /// Return this class as a Dictionary&lt;class, Dictionary&lt;key, value&gt;&gt;
        ///</summary>
        public static Dictionary<string, Dictionary<string, string>> GetAsDictionary() {
            return Utilities.GetResourcesByNameSpace("^Branding");
        }

        private static System.Resources.ResourceManager _resourceManager;    
    
        ///<summary>
        /// Get the ResourceManager
        ///</summary>
        private static System.Resources.ResourceManager ResourceManager 
        {
            get 
            {
                return _resourceManager ?? (_resourceManager = new System.Resources.ResourceManager("Weingarner.I18N.Sample.Branding", typeof(Branding).Assembly));
            }
        }

        ///<summary>
        ///	Get localized entry for a given key
        ///</summary>
        public static string GetResourceString(string key, params object[] args)
        {
            var value = ResourceManager.GetString(key, CultureResources.Instance.CultureInfo );

            if (!string.IsNullOrEmpty(value))
            {
                var regex  = @"{\b\p{Lu}{3,}\b}";
                var tokens = Regex.Matches(value, regex).Cast<Match>().Select(m => m.Value).ToList();
                    tokens
                        .ForEach(t =>
                        {
                            value = value.Replace(t, Utilities.GetReplacementString(t.Replace("{", "").Replace("}", "")));
                        });

                if (args.Any())
                {
                    regex  = @"{[0-9]{1}}";
                    tokens = Regex.Matches(value, regex).Cast<Match>().Select(m => m.Value).ToList();

                    if (tokens.Any())
                    {
                        // If argument length is less than token length, add an error message
                        // This can happen if arguments are accidentally forgottent in a translation
                        if (args.Count() < tokens.Count())
                        {
                            var newArgs = new List<object>();
                            for (var i = 0; i < tokens.Count(); i++)
                            {
                                newArgs.Add(args.Length > i ? args[i] : "argument {" + i + "} is undefined");
                            }

                            args = newArgs.ToArray();
                        }

                        value = string.Format(value, args);
                    }
                }		        		
            }
    
            return value;
        } 
        
        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>Info: Set your prefered language in your browser settings to EN or FR to see changes</description>
        ///        </item>
        ///        <item>
        ///            <description></description>
        ///        </item>
        ///    </list>
        ///</summary>
        [Utilities.Localized]
        public string Info { get { return GetResourceString("Info"); } }

        
    }
}

namespace Weingarner.I18N.Sample  {
    	
    public partial class Branding : ReactiveObject {
    
        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>Same stuff in JavaScript :)</description>
        ///        </item>
        ///        <item>
        ///            <description></description>
        ///        </item>
        ///    </list>
        ///</summary>
        [Utilities.Localized]
        public string JavaScriptTitle { get { return GetResourceString("JavaScriptTitle"); } }

        
    }
}

namespace Weingarner.I18N.Sample  {
    	
    public partial class Branding : ReactiveObject {
    
        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>Send</description>
        ///        </item>
        ///        <item>
        ///            <description></description>
        ///        </item>
        ///    </list>
        ///</summary>
        [Utilities.Localized]
        public string Submit { get { return GetResourceString("Submit"); } }

        
    }
}

namespace Weingarner.I18N.Sample  {
    	
    public partial class Branding : ReactiveObject {
    
        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>T4ResX Sample</description>
        ///        </item>
        ///        <item>
        ///            <description></description>
        ///        </item>
        ///    </list>
        ///</summary>
        [Utilities.Localized]
        public string Title { get { return GetResourceString("Title"); } }

        
    }
}

namespace Weingarner.I18N.Sample  {
    	
    public partial class Branding : ReactiveObject {
    
        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>Welcome to {BRAND}</description>
        ///        </item>
        ///        <item>
        ///            <description></description>
        ///        </item>
        ///    </list>
        ///</summary>
        public string WelcomeFormatted(params object[] args) { return GetResourceString("Welcome", args); }
        
        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>Welcome to {BRAND}</description>
        ///        </item>
        ///        <item>
        ///            <description></description>
        ///        </item>
        ///    </list>
        ///</summary>
        [Utilities.Localized]
        public string Welcome { get { return GetResourceString("Welcome"); } }

        
    }
}

namespace Weingarner.I18N.Sample.Models  {
    [Utilities.Localized]	
    public partial class User : ReactiveObject {
    
        public User (){
            CultureResources
                .Instance
                .WhenAnyValue(p=>p.CultureInfo)
// ReSharper disable once ExplicitCallerInfoArgument
                .Subscribe(c=>this.RaisePropertyChanged(""));
        }
        ///<summary>
        /// Return this class as a Dictionary&lt;class, Dictionary&lt;key, value&gt;&gt;
        ///</summary>
        public static Dictionary<string, Dictionary<string, string>> GetAsDictionary() {
            return Utilities.GetResourcesByNameSpace("Models^User");
        }

        private static System.Resources.ResourceManager _resourceManager;    
    
        ///<summary>
        /// Get the ResourceManager
        ///</summary>
        private static System.Resources.ResourceManager ResourceManager 
        {
            get 
            {
                return _resourceManager ?? (_resourceManager = new System.Resources.ResourceManager("Weingarner.I18N.Sample.Models.User", typeof(User).Assembly));
            }
        }

        ///<summary>
        ///	Get localized entry for a given key
        ///</summary>
        public static string GetResourceString(string key, params object[] args)
        {
            var value = ResourceManager.GetString(key, CultureResources.Instance.CultureInfo );

            if (!string.IsNullOrEmpty(value))
            {
                var regex  = @"{\b\p{Lu}{3,}\b}";
                var tokens = Regex.Matches(value, regex).Cast<Match>().Select(m => m.Value).ToList();
                    tokens
                        .ForEach(t =>
                        {
                            value = value.Replace(t, Utilities.GetReplacementString(t.Replace("{", "").Replace("}", "")));
                        });

                if (args.Any())
                {
                    regex  = @"{[0-9]{1}}";
                    tokens = Regex.Matches(value, regex).Cast<Match>().Select(m => m.Value).ToList();

                    if (tokens.Any())
                    {
                        // If argument length is less than token length, add an error message
                        // This can happen if arguments are accidentally forgottent in a translation
                        if (args.Count() < tokens.Count())
                        {
                            var newArgs = new List<object>();
                            for (var i = 0; i < tokens.Count(); i++)
                            {
                                newArgs.Add(args.Length > i ? args[i] : "argument {" + i + "} is undefined");
                            }

                            args = newArgs.ToArray();
                        }

                        value = string.Format(value, args);
                    }
                }		        		
            }
    
            return value;
        } 
        
        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>Enter your pseudo</description>
        ///        </item>
        ///        <item>
        ///            <description></description>
        ///        </item>
        ///    </list>
        ///</summary>
        [Utilities.Localized]
        public string Pseudo { get { return GetResourceString("Pseudo"); } }

        
    }
}

namespace Weingarner.I18N.Sample.Models  {
    	
    public partial class User : ReactiveObject {
    
        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>Sorry, your pseudo must be T4ResX ! :)</description>
        ///        </item>
        ///        <item>
        ///            <description></description>
        ///        </item>
        ///    </list>
        ///</summary>
        [Utilities.Localized]
        public string PseudoError { get { return GetResourceString("PseudoError"); } }

        
    }
}

namespace Weingarner.I18N.Sample.Models  {
    	
    public partial class User : ReactiveObject {
    
        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>Thanks !</description>
        ///        </item>
        ///        <item>
        ///            <description></description>
        ///        </item>
        ///    </list>
        ///</summary>
        [Utilities.Localized]
        public string PseudoOk { get { return GetResourceString("PseudoOk"); } }

        
    }
}

namespace Weingarner.I18N.Sample.Models  {
    	
    public partial class User : ReactiveObject {
    
        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>^T4ResX$</description>
        ///        </item>
        ///        <item>
        ///            <description>[type:constant]</description>
        ///        </item>
        ///    </list>
        ///</summary>
        [Utilities.Localized]
        public string PseudoRegex { get { return GetResourceString("PseudoRegex"); } }

        
        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>^T4ResX$</description>
        ///        </item>
        ///        <item>
        ///            <description>[type:constant]</description>
        ///        </item>
        ///        <item>
        ///            <description>
        ///					There are places where we cannot use strings as they are considered dynamic
        ///					
        ///					[RegularExpressionAttribute(User.PseudoRegexConstant, ErrorMessageResourceName = "PseudoError", ErrorMessageResourceType = typeof(User))]
        ///
        ///					However:
        ///					constant = no dynamic content
        ///					If you have an idea of how to make constants dynamically localizable, let me know !
        ///				</description>
        ///        </item>
        ///    </list>
        ///</summary>	
        public const string PseudoRegexConstant = "^T4ResX$";
        
    }
}

namespace Weingarner.I18N.Sample.Models  {
    	
    public partial class User : ReactiveObject {
    
        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>This field is required</description>
        ///        </item>
        ///        <item>
        ///            <description></description>
        ///        </item>
        ///    </list>
        ///</summary>
        [Utilities.Localized]
        public string RequiredError { get { return GetResourceString("RequiredError"); } }

        
    }
}
