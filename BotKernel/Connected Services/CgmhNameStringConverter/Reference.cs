﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//
//     變更此檔案可能會導致不正確的行為，而且若已重新產生
//     程式碼，則會遺失變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CgmhNameStringConverter
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CgmhNameStringConverter.WebService1Soap")]
    internal interface WebService1Soap
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Big5Hex2UTF8", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<string> Big5Hex2UTF8Async(string strHex);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Big5String2UTF8String", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<string> Big5String2UTF8StringAsync(string strBig5);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UTF8ToBig5Hex", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<string> UTF8ToBig5HexAsync(string strUTF8);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    internal interface WebService1SoapChannel : CgmhNameStringConverter.WebService1Soap, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.2.0-preview1.23462.5")]
    internal partial class WebService1SoapClient : System.ServiceModel.ClientBase<CgmhNameStringConverter.WebService1Soap>, CgmhNameStringConverter.WebService1Soap
    {
        
        /// <summary>
        /// 實作此部分方法來設定服務端點。
        /// </summary>
        /// <param name="serviceEndpoint">要設定的端點</param>
        /// <param name="clientCredentials">用戶端認證</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public WebService1SoapClient(EndpointConfiguration endpointConfiguration) : 
                base(WebService1SoapClient.GetBindingForEndpoint(endpointConfiguration), WebService1SoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WebService1SoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(WebService1SoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WebService1SoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(WebService1SoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WebService1SoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<string> Big5Hex2UTF8Async(string strHex)
        {
            return base.Channel.Big5Hex2UTF8Async(strHex);
        }
        
        public System.Threading.Tasks.Task<string> Big5String2UTF8StringAsync(string strBig5)
        {
            return base.Channel.Big5String2UTF8StringAsync(strBig5);
        }
        
        public System.Threading.Tasks.Task<string> UTF8ToBig5HexAsync(string strUTF8)
        {
            return base.Channel.UTF8ToBig5HexAsync(strUTF8);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.WebService1Soap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.WebService1Soap12))
            {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                textBindingElement.MessageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None);
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpTransportBindingElement httpBindingElement = new System.ServiceModel.Channels.HttpTransportBindingElement();
                httpBindingElement.AllowCookies = true;
                httpBindingElement.MaxBufferSize = int.MaxValue;
                httpBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("找不到名為 \'{0}\' 的端點。", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.WebService1Soap))
            {
                return new System.ServiceModel.EndpointAddress("http://10.30.111.60/transform_string.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.WebService1Soap12))
            {
                return new System.ServiceModel.EndpointAddress("http://10.30.111.60/transform_string.asmx");
            }
            throw new System.InvalidOperationException(string.Format("找不到名為 \'{0}\' 的端點。", endpointConfiguration));
        }
        
        public enum EndpointConfiguration
        {
            
            WebService1Soap,
            
            WebService1Soap12,
        }
    }
}
