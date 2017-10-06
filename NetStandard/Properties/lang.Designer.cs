// This code is automatically generated and shares the copyright of the library it is generated into.
using System;
using System.ComponentModel;
using System.Linq; 
using System.Reflection;
using System.Collections.Generic; 
using System.Text.RegularExpressions;
using I18N.Reactive;

namespace NetStandard.Properties  {
    public class lang : INotifyPropertyChanged {
    
        private lang (){
            CultureResources
                .Instance.PropertyChanged += (s, e) => {
                    if (e.PropertyName == "CultureInfo") {
                        RaisePropertyChanged();
                    }
                };
        }

        public static readonly lang Instance = new lang();

        // This is for the XAML code
        public static lang GetInstance() {
            return Instance;
        }

        public void RaisePropertyChanged() {
            OnPropertyChanged("a");
            OnPropertyChanged("b");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private static System.Resources.ResourceManager _resourceManager;    
    
        ///<summary>
        /// Get the ResourceManager
        ///</summary>
        private static System.Resources.ResourceManager ResourceManager 
        {
            get 
            {
                return _resourceManager ?? (_resourceManager = new System.Resources.ResourceManager("NetStandard.Properties.lang", typeof(lang).Assembly));
            }
        }

        ///<summary>
        ///	Get localized entry for a given key
        ///</summary>
        public static string GetResourceString(string key, params object[] args)
        {
            var value = ResourceManager.GetString(key, CultureResources.Instance.CultureInfo );
            return CultureResources.Instance.ProcessArguments(value, args);
        } 

        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>a</description>
        ///        </item>
        ///        <item>
        ///            <description></description>
        ///        </item>
        ///    </list>
        ///</summary>
        public string a => GetResourceString("a");

        ///<summary>
        ///    <list type='bullet'>
        ///        <item>
        ///            <description>b</description>
        ///        </item>
        ///        <item>
        ///            <description></description>
        ///        </item>
        ///    </list>
        ///</summary>
        public string b => GetResourceString("b");
    }
}
