﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DNEE.Internal.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SR {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SR() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DNEE.Internal.Resources.SR", typeof(SR).Assembly);
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
        ///   Looks up a localized string similar to There was an error while invoking the handlers for event {0}.
        /// </summary>
        internal static string ErrorInvokingEvents {
            get {
                return ResourceManager.GetString("ErrorInvokingEvents", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Event handle is not valid!.
        /// </summary>
        internal static string EventHandleInvalid {
            get {
                return ResourceManager.GetString("EventHandleInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided origin is not valid!.
        /// </summary>
        internal static string EventName_OriginNotValid {
            get {
                return ResourceManager.GetString("EventName_OriginNotValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Event name is not valid!.
        /// </summary>
        internal static string EventNameInvalid {
            get {
                return ResourceManager.GetString("EventNameInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided EventHandle was not created by this EventSource..
        /// </summary>
        internal static string EventSource_HandleNotFromThisSource {
            get {
                return ResourceManager.GetString("EventSource_HandleNotFromThisSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided origin is already attached to an event source!.
        /// </summary>
        internal static string EventSource_OriginAlreadyAttached {
            get {
                return ResourceManager.GetString("EventSource_OriginAlreadyAttached", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Next can only be invoked once!.
        /// </summary>
        internal static string Handler_NextInvokedOnceOnly {
            get {
                return ResourceManager.GetString("Handler_NextInvokedOnceOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not get the constructing member for DataOrigin.
        /// </summary>
        internal static string Origin_CouldNotGetConstructingMember {
            get {
                return ResourceManager.GetString("Origin_CouldNotGetConstructingMember", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Source can only be set once!.
        /// </summary>
        internal static string Origin_SourceSetOnceOnly {
            get {
                return ResourceManager.GetString("Origin_SourceSetOnceOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {3}({1}::{0} for {2}).
        /// </summary>
        internal static string Origin_StringFormat {
            get {
                return ResourceManager.GetString("Origin_StringFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Attempting to get the EventSource associated with an origin!.
        /// </summary>
        internal static string Origin_TryGetSourceFromOrigin {
            get {
                return ResourceManager.GetString("Origin_TryGetSourceFromOrigin", resourceCulture);
            }
        }
    }
}