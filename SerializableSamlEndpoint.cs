using System;
using System.Runtime.Serialization;
using Microsoft.IdentityServer.PowerShell.Resources;

namespace VflIt.Samples.AdfsSnapIn
{
    [DataContract]
    public class SerializableSamlEndpoint
    {
        [DataMember]
        public string Binding { get; set; }
        [DataMember]
        public Uri BindingUri { get; set; }
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public bool IsDefault { get; set; }
        [DataMember]
        public Uri Location{ get; set; }
        [DataMember]
        public string Protocol{ get; set; }
        [DataMember]
        public Uri ResponseLocation { get; set; }

        public static SerializableSamlEndpoint FromSamlEndpoint(SamlEndpoint samlEndpoint)
        {
            var source = samlEndpoint;
            var target = new SerializableSamlEndpoint();

            target.Binding = source.Binding;
            target.BindingUri = source.BindingUri;
            target.Index = source.Index;
            target.IsDefault = source.IsDefault;
            target.Location = source.Location;
            target.Protocol = source.Protocol;
            target.ResponseLocation = source.ResponseLocation;

            return target;
        }
    }
}