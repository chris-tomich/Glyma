﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.544
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 5.0.61118.0
// 
namespace SilverlightMappingToolBasic.ThemeService {
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ThemeResult", Namespace="http://schemas.datacontract.org/2004/07/ThemeService")]
    public partial class ThemeResult : object, System.ComponentModel.INotifyPropertyChanged {
        
        private bool SuccessField;
        
        private SilverlightMappingToolBasic.ThemeService.Theme ThemeField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Success {
            get {
                return this.SuccessField;
            }
            set {
                if ((this.SuccessField.Equals(value) != true)) {
                    this.SuccessField = value;
                    this.RaisePropertyChanged("Success");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public SilverlightMappingToolBasic.ThemeService.Theme Theme {
            get {
                return this.ThemeField;
            }
            set {
                if ((object.ReferenceEquals(this.ThemeField, value) != true)) {
                    this.ThemeField = value;
                    this.RaisePropertyChanged("Theme");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Theme", Namespace="http://schemas.datacontract.org/2004/07/ThemeService")]
    public partial class Theme : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string AssemblyField;
        
        private string NameField;
        
        private System.Collections.ObjectModel.ObservableCollection<SilverlightMappingToolBasic.ThemeService.ThemeSkin> SkinField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Assembly {
            get {
                return this.AssemblyField;
            }
            set {
                if ((object.ReferenceEquals(this.AssemblyField, value) != true)) {
                    this.AssemblyField = value;
                    this.RaisePropertyChanged("Assembly");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<SilverlightMappingToolBasic.ThemeService.ThemeSkin> Skin {
            get {
                return this.SkinField;
            }
            set {
                if ((object.ReferenceEquals(this.SkinField, value) != true)) {
                    this.SkinField = value;
                    this.RaisePropertyChanged("Skin");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ThemeSkin", Namespace="http://schemas.datacontract.org/2004/07/ThemeService")]
    public partial class ThemeSkin : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string AssemblyField;
        
        private string ClassField;
        
        private string NameField;
        
        private string NodeTypeField;
        
        private System.Collections.ObjectModel.ObservableCollection<SilverlightMappingToolBasic.ThemeService.ThemeSkinProperty> PropertyField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Assembly {
            get {
                return this.AssemblyField;
            }
            set {
                if ((object.ReferenceEquals(this.AssemblyField, value) != true)) {
                    this.AssemblyField = value;
                    this.RaisePropertyChanged("Assembly");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Class {
            get {
                return this.ClassField;
            }
            set {
                if ((object.ReferenceEquals(this.ClassField, value) != true)) {
                    this.ClassField = value;
                    this.RaisePropertyChanged("Class");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NodeType {
            get {
                return this.NodeTypeField;
            }
            set {
                if ((object.ReferenceEquals(this.NodeTypeField, value) != true)) {
                    this.NodeTypeField = value;
                    this.RaisePropertyChanged("NodeType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<SilverlightMappingToolBasic.ThemeService.ThemeSkinProperty> Property {
            get {
                return this.PropertyField;
            }
            set {
                if ((object.ReferenceEquals(this.PropertyField, value) != true)) {
                    this.PropertyField = value;
                    this.RaisePropertyChanged("Property");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ThemeSkinProperty", Namespace="http://schemas.datacontract.org/2004/07/ThemeService")]
    public partial class ThemeSkinProperty : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string NameField;
        
        private string ValueField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Value {
            get {
                return this.ValueField;
            }
            set {
                if ((object.ReferenceEquals(this.ValueField, value) != true)) {
                    this.ValueField = value;
                    this.RaisePropertyChanged("Value");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ThemeService.IThemeService")]
    public interface IThemeService {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IThemeService/GetTheme", ReplyAction="http://tempuri.org/IThemeService/GetThemeResponse")]
        System.IAsyncResult BeginGetTheme(string name, System.AsyncCallback callback, object asyncState);
        
        SilverlightMappingToolBasic.ThemeService.ThemeResult EndGetTheme(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IThemeService/GetContextMenuXaml", ReplyAction="http://tempuri.org/IThemeService/GetContextMenuXamlResponse")]
        System.IAsyncResult BeginGetContextMenuXaml(string name, System.AsyncCallback callback, object asyncState);
        
        string EndGetContextMenuXaml(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IThemeServiceChannel : SilverlightMappingToolBasic.ThemeService.IThemeService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetThemeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetThemeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public SilverlightMappingToolBasic.ThemeService.ThemeResult Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((SilverlightMappingToolBasic.ThemeService.ThemeResult)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetContextMenuXamlCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetContextMenuXamlCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public string Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ThemeServiceClient : System.ServiceModel.ClientBase<SilverlightMappingToolBasic.ThemeService.IThemeService>, SilverlightMappingToolBasic.ThemeService.IThemeService {
        
        private BeginOperationDelegate onBeginGetThemeDelegate;
        
        private EndOperationDelegate onEndGetThemeDelegate;
        
        private System.Threading.SendOrPostCallback onGetThemeCompletedDelegate;
        
        private BeginOperationDelegate onBeginGetContextMenuXamlDelegate;
        
        private EndOperationDelegate onEndGetContextMenuXamlDelegate;
        
        private System.Threading.SendOrPostCallback onGetContextMenuXamlCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public ThemeServiceClient() {
        }
        
        public ThemeServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ThemeServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ThemeServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ThemeServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<GetThemeCompletedEventArgs> GetThemeCompleted;
        
        public event System.EventHandler<GetContextMenuXamlCompletedEventArgs> GetContextMenuXamlCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult SilverlightMappingToolBasic.ThemeService.IThemeService.BeginGetTheme(string name, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetTheme(name, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        SilverlightMappingToolBasic.ThemeService.ThemeResult SilverlightMappingToolBasic.ThemeService.IThemeService.EndGetTheme(System.IAsyncResult result) {
            return base.Channel.EndGetTheme(result);
        }
        
        private System.IAsyncResult OnBeginGetTheme(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string name = ((string)(inValues[0]));
            return ((SilverlightMappingToolBasic.ThemeService.IThemeService)(this)).BeginGetTheme(name, callback, asyncState);
        }
        
        private object[] OnEndGetTheme(System.IAsyncResult result) {
            SilverlightMappingToolBasic.ThemeService.ThemeResult retVal = ((SilverlightMappingToolBasic.ThemeService.IThemeService)(this)).EndGetTheme(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetThemeCompleted(object state) {
            if ((this.GetThemeCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetThemeCompleted(this, new GetThemeCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetThemeAsync(string name) {
            this.GetThemeAsync(name, null);
        }
        
        public void GetThemeAsync(string name, object userState) {
            if ((this.onBeginGetThemeDelegate == null)) {
                this.onBeginGetThemeDelegate = new BeginOperationDelegate(this.OnBeginGetTheme);
            }
            if ((this.onEndGetThemeDelegate == null)) {
                this.onEndGetThemeDelegate = new EndOperationDelegate(this.OnEndGetTheme);
            }
            if ((this.onGetThemeCompletedDelegate == null)) {
                this.onGetThemeCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetThemeCompleted);
            }
            base.InvokeAsync(this.onBeginGetThemeDelegate, new object[] {
                        name}, this.onEndGetThemeDelegate, this.onGetThemeCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult SilverlightMappingToolBasic.ThemeService.IThemeService.BeginGetContextMenuXaml(string name, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetContextMenuXaml(name, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        string SilverlightMappingToolBasic.ThemeService.IThemeService.EndGetContextMenuXaml(System.IAsyncResult result) {
            return base.Channel.EndGetContextMenuXaml(result);
        }
        
        private System.IAsyncResult OnBeginGetContextMenuXaml(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string name = ((string)(inValues[0]));
            return ((SilverlightMappingToolBasic.ThemeService.IThemeService)(this)).BeginGetContextMenuXaml(name, callback, asyncState);
        }
        
        private object[] OnEndGetContextMenuXaml(System.IAsyncResult result) {
            string retVal = ((SilverlightMappingToolBasic.ThemeService.IThemeService)(this)).EndGetContextMenuXaml(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetContextMenuXamlCompleted(object state) {
            if ((this.GetContextMenuXamlCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetContextMenuXamlCompleted(this, new GetContextMenuXamlCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetContextMenuXamlAsync(string name) {
            this.GetContextMenuXamlAsync(name, null);
        }
        
        public void GetContextMenuXamlAsync(string name, object userState) {
            if ((this.onBeginGetContextMenuXamlDelegate == null)) {
                this.onBeginGetContextMenuXamlDelegate = new BeginOperationDelegate(this.OnBeginGetContextMenuXaml);
            }
            if ((this.onEndGetContextMenuXamlDelegate == null)) {
                this.onEndGetContextMenuXamlDelegate = new EndOperationDelegate(this.OnEndGetContextMenuXaml);
            }
            if ((this.onGetContextMenuXamlCompletedDelegate == null)) {
                this.onGetContextMenuXamlCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetContextMenuXamlCompleted);
            }
            base.InvokeAsync(this.onBeginGetContextMenuXamlDelegate, new object[] {
                        name}, this.onEndGetContextMenuXamlDelegate, this.onGetContextMenuXamlCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override SilverlightMappingToolBasic.ThemeService.IThemeService CreateChannel() {
            return new ThemeServiceClientChannel(this);
        }
        
        private class ThemeServiceClientChannel : ChannelBase<SilverlightMappingToolBasic.ThemeService.IThemeService>, SilverlightMappingToolBasic.ThemeService.IThemeService {
            
            public ThemeServiceClientChannel(System.ServiceModel.ClientBase<SilverlightMappingToolBasic.ThemeService.IThemeService> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginGetTheme(string name, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = name;
                System.IAsyncResult _result = base.BeginInvoke("GetTheme", _args, callback, asyncState);
                return _result;
            }
            
            public SilverlightMappingToolBasic.ThemeService.ThemeResult EndGetTheme(System.IAsyncResult result) {
                object[] _args = new object[0];
                SilverlightMappingToolBasic.ThemeService.ThemeResult _result = ((SilverlightMappingToolBasic.ThemeService.ThemeResult)(base.EndInvoke("GetTheme", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginGetContextMenuXaml(string name, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = name;
                System.IAsyncResult _result = base.BeginInvoke("GetContextMenuXaml", _args, callback, asyncState);
                return _result;
            }
            
            public string EndGetContextMenuXaml(System.IAsyncResult result) {
                object[] _args = new object[0];
                string _result = ((string)(base.EndInvoke("GetContextMenuXaml", _args, result)));
                return _result;
            }
        }
    }
}
