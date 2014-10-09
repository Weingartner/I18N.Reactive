using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Weingartner.I18N.Annotations;

namespace Weingartner.I18N
{
    /// <summary>
    /// Wraps up XAML access to instance of WPFLocalize.Properties.Resources, list of available cultures, and method to change culture
    /// </summary>
    public class CultureResources : INotifyPropertyChanged
    {
        private CultureInfo _CultureInfo;
        public CultureInfo CultureInfo
        {
            get { return _CultureInfo; }
            set
            {
                if (_CultureInfo.Equals(value))
                    return;
                _CultureInfo = value;
                OnPropertyChanged();
            }
        }

        private CultureResources()
        {
        }

        private static readonly Lazy<CultureResources> _Instance
            = new Lazy<CultureResources>(() => new CultureResources());

        public static CultureResources Instance
        {
            get { return _Instance.Value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ProcessArguments(string value, object[] args)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var regex = @"{\b\p{Lu}{3,}\b}";
                var tokens = Regex.Matches(value, regex).Cast<Match>().Select(m => m.Value).ToList();
                tokens
                    .ForEach(t =>
                    {
                        value = value.Replace(t, t.Replace("{", "").Replace("}", ""));
                    });

                if (args.Any())
                {
                    regex = @"{[0-9]{1}}";
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
    }
}
