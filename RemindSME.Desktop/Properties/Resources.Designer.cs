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
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RemindSME.Desktop.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It&apos;s going to be hot today (up to {0:F0}°C)! Please set the air conditioning to a sensible temperature and close the windows..
        /// </summary>
        internal static string Reminder_CheckAirCon_Message {
            get {
                return ResourceManager.GetString("Reminder_CheckAirCon_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Check the air conditioning.
        /// </summary>
        internal static string Reminder_CheckAirCon_Title {
            get {
                return ResourceManager.GetString("Reminder_CheckAirCon_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It&apos;s cold today ({0:F0}°C)! Please set the heating to a sensible temperature and close the windows..
        /// </summary>
        internal static string Reminder_CheckHeating_Message {
            get {
                return ResourceManager.GetString("Reminder_CheckHeating_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Check the heating.
        /// </summary>
        internal static string Reminder_CheckHeating_Title {
            get {
                return ResourceManager.GetString("Reminder_CheckHeating_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The temperature outside has changed, make sure air conditioning is set to a suitable temperature. If it is hot, can you open the windows instead of using the air con?.
        /// </summary>
        internal static string Reminder_HeatingDefault_Message {
            get {
                return ResourceManager.GetString("Reminder_HeatingDefault_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Check the air conditioning.
        /// </summary>
        internal static string Reminder_HeatingDefault_Title {
            get {
                return ResourceManager.GetString("Reminder_HeatingDefault_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Looks like you&apos;re one of the first people in the office today, make sure the air conditioning is set to a suitable temperature for this morning&apos;s weather..
        /// </summary>
        internal static string Reminder_HeatingFirstLogin_Message {
            get {
                return ResourceManager.GetString("Reminder_HeatingFirstLogin_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Check the air conditioning.
        /// </summary>
        internal static string Reminder_HeatingFirstLogin_Title {
            get {
                return ResourceManager.GetString("Reminder_HeatingFirstLogin_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Looks like you&apos;re one of the last in the office, don&apos;t forget to turn off the air conditioning and lights before you leave..
        /// </summary>
        internal static string Reminder_LastToLeave_Message {
            get {
                return ResourceManager.GetString("Reminder_LastToLeave_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Switch everything off before you leave.
        /// </summary>
        internal static string Reminder_LastToLeave_Title {
            get {
                return ResourceManager.GetString("Reminder_LastToLeave_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It&apos;s going to be {0:F0}°C today. Please consider whether you need the heating or air conditioning. Can you open windows instead?.
        /// </summary>
        internal static string Reminder_WeatherDefault_Message {
            get {
                return ResourceManager.GetString("Reminder_WeatherDefault_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Check the air conditioning.
        /// </summary>
        internal static string Reminder_WeatherDefault_Title {
            get {
                return ResourceManager.GetString("Reminder_WeatherDefault_Title", resourceCulture);
            }
        }
    }
}
