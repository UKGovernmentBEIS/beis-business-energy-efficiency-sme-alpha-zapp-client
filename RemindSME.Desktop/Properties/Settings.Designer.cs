﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RemindSME.Desktop.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.7.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime NextHibernationTime {
            get {
                return ((global::System.DateTime)(this["NextHibernationTime"]));
            }
            set {
                this["NextHibernationTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool HeatingOptIn {
            get {
                return ((bool)(this["HeatingOptIn"]));
            }
            set {
                this["HeatingOptIn"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool HibernationOptIn {
            get {
                return ((bool)(this["HibernationOptIn"]));
            }
            set {
                this["HibernationOptIn"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("18:15:00")]
        public global::System.TimeSpan DefaultHibernationTime {
            get {
                return ((global::System.TimeSpan)(this["DefaultHibernationTime"]));
            }
            set {
                this["DefaultHibernationTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DisplaySettingExplanations {
            get {
                return ((bool)(this["DisplaySettingExplanations"]));
            }
            set {
                this["DisplaySettingExplanations"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Pseudonym {
            get {
                return ((string)(this["Pseudonym"]));
            }
            set {
                this["Pseudonym"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime MostRecentFirstLoginReminderDismissal {
            get {
                return ((global::System.DateTime)(this["MostRecentFirstLoginReminderDismissal"]));
            }
            set {
                this["MostRecentFirstLoginReminderDismissal"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime MostRecentLastToLeaveReminderDismissal {
            get {
                return ((global::System.DateTime)(this["MostRecentLastToLeaveReminderDismissal"]));
            }
            set {
                this["MostRecentLastToLeaveReminderDismissal"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime LastToLeaveReminderSnoozeUntilTime {
            get {
                return ((global::System.DateTime)(this["LastToLeaveReminderSnoozeUntilTime"]));
            }
            set {
                this["LastToLeaveReminderSnoozeUntilTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string CompanyId {
            get {
                return ((string)(this["CompanyId"]));
            }
            set {
                this["CompanyId"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string CompanyName {
            get {
                return ((string)(this["CompanyName"]));
            }
            set {
                this["CompanyName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />")]
        public global::System.Collections.Specialized.StringCollection WorkNetworks {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["WorkNetworks"]));
            }
            set {
                this["WorkNetworks"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />")]
        public global::System.Collections.Specialized.StringCollection OtherNetworks {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["OtherNetworks"]));
            }
            set {
                this["OtherNetworks"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double MostRecentPeakTemperature {
            get {
                return ((double)(this["MostRecentPeakTemperature"]));
            }
            set {
                this["MostRecentPeakTemperature"] = value;
            }
        }
    }
}
