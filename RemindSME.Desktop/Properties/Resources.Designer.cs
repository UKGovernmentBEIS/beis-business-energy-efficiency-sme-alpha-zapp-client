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
        ///   Looks up a localized string similar to Perhaps consider opening the windows instead of putting on the air conditioning?.
        /// </summary>
        internal static string Notification_HeatingDefault_Message {
            get {
                return ResourceManager.GetString("Notification_HeatingDefault_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It&apos;s hot today!.
        /// </summary>
        internal static string Notification_HeatingDefault_Title {
            get {
                return ResourceManager.GetString("Notification_HeatingDefault_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please make sure the heating / air conditioning is set to a sensible temperature for today&apos;s weather. If it&apos;s hot, can you open windows instead?.
        /// </summary>
        internal static string Notification_HeatingFirstLogin_Message {
            get {
                return ResourceManager.GetString("Notification_HeatingFirstLogin_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Good morning!.
        /// </summary>
        internal static string Notification_HeatingFirstLogin_Title {
            get {
                return ResourceManager.GetString("Notification_HeatingFirstLogin_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Don&apos;t forget to switch off the lights and heating / air conditioning if you&apos;re the last one out tonight!.
        /// </summary>
        internal static string Notification_LastOut_Message {
            get {
                return ResourceManager.GetString("Notification_LastOut_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Staying a bit later?.
        /// </summary>
        internal static string Notification_LastOut_Title {
            get {
                return ResourceManager.GetString("Notification_LastOut_Title", resourceCulture);
            }
        }
    }
}
