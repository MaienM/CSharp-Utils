﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CSharpUtils.StatusLoggerServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="StatusChangeEventArgs", Namespace="http://schemas.datacontract.org/2004/07/CSharpUtils.Utils.StatusLogger")]
    [System.SerializableAttribute()]
    public partial class StatusChangeEventArgs : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string KeyField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private object NewValueField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NewValueStringField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private object OldValueField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string OldValueStringField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime TimestampField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Key {
            get {
                return this.KeyField;
            }
            set {
                if ((object.ReferenceEquals(this.KeyField, value) != true)) {
                    this.KeyField = value;
                    this.RaisePropertyChanged("Key");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public object NewValue {
            get {
                return this.NewValueField;
            }
            set {
                if ((object.ReferenceEquals(this.NewValueField, value) != true)) {
                    this.NewValueField = value;
                    this.RaisePropertyChanged("NewValue");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NewValueString {
            get {
                return this.NewValueStringField;
            }
            set {
                if ((object.ReferenceEquals(this.NewValueStringField, value) != true)) {
                    this.NewValueStringField = value;
                    this.RaisePropertyChanged("NewValueString");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public object OldValue {
            get {
                return this.OldValueField;
            }
            set {
                if ((object.ReferenceEquals(this.OldValueField, value) != true)) {
                    this.OldValueField = value;
                    this.RaisePropertyChanged("OldValue");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string OldValueString {
            get {
                return this.OldValueStringField;
            }
            set {
                if ((object.ReferenceEquals(this.OldValueStringField, value) != true)) {
                    this.OldValueStringField = value;
                    this.RaisePropertyChanged("OldValueString");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Timestamp {
            get {
                return this.TimestampField;
            }
            set {
                if ((this.TimestampField.Equals(value) != true)) {
                    this.TimestampField = value;
                    this.RaisePropertyChanged("Timestamp");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="StatusLoggerServiceReference.IStatusLoggerService", CallbackContract=typeof(CSharpUtils.StatusLoggerServiceReference.IStatusLoggerServiceCallback))]
    public interface IStatusLoggerService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStatusLoggerService/OpenSession", ReplyAction="http://tempuri.org/IStatusLoggerService/OpenSessionResponse")]
        void OpenSession();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStatusLoggerService/OpenSession", ReplyAction="http://tempuri.org/IStatusLoggerService/OpenSessionResponse")]
        System.Threading.Tasks.Task OpenSessionAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IStatusLoggerServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStatusLoggerService/OnChanged", ReplyAction="http://tempuri.org/IStatusLoggerService/OnChangedResponse")]
        void OnChanged(CSharpUtils.StatusLoggerServiceReference.StatusChangeEventArgs e);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IStatusLoggerServiceChannel : CSharpUtils.StatusLoggerServiceReference.IStatusLoggerService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class StatusLoggerServiceClient : System.ServiceModel.DuplexClientBase<CSharpUtils.StatusLoggerServiceReference.IStatusLoggerService>, CSharpUtils.StatusLoggerServiceReference.IStatusLoggerService {
        
        public StatusLoggerServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public StatusLoggerServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public StatusLoggerServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public StatusLoggerServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public StatusLoggerServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void OpenSession() {
            base.Channel.OpenSession();
        }
        
        public System.Threading.Tasks.Task OpenSessionAsync() {
            return base.Channel.OpenSessionAsync();
        }
    }
}
