using System;
using System.Globalization;
using ReactiveUI;

namespace Weingartner.I18N
{
    /// <summary>
    /// Wraps up XAML access to instance of WPFLocalize.Properties.Resources, list of available cultures, and method to change culture
    /// </summary>
    public class CultureResources : ReactiveObject
    {
        private CultureInfo _CultureInfo;
        public CultureInfo CultureInfo
        {
            get { return _CultureInfo; }
            set { this.RaiseAndSetIfChanged(ref _CultureInfo, value); }
        }

        private CultureResources()
        {
        }

        private static readonly Lazy<CultureResources> _Instance 
            = new Lazy<CultureResources>(()=> new CultureResources());

        public static CultureResources Instance
        {
            get { return _Instance.Value; }
        }
    }

}
