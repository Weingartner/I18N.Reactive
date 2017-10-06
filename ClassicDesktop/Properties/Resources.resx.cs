// This code is automatically generated and shares the copyright of the library it is generated into.
using System;
using System.ComponentModel;
using System.Linq; 
using System.Reflection;
using System.Collections.Generic; 
using System.Text.RegularExpressions;
using I18N.Reactive;

namespace ClassicDesktop.Properties  {
    public class Resources : INotifyPropertyChanged {
    
        private Resources (){
            CultureResources
                .Instance.PropertyChanged += (s, e) => {
                    if (e.PropertyName == "CultureInfo") {
                        RaisePropertyChanged();
                    }
                };
        }

        public static readonly Resources Instance = new Resources();

        // This is for the XAML code
        public static Resources GetInstance() {
            return Instance;
        }

        public void RaisePropertyChanged() {
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
                return _resourceManager ?? (_resourceManager = new System.Resources.ResourceManager("ClassicDesktop.Properties.Resources", typeof(Resources).Assembly));
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
    }
}
