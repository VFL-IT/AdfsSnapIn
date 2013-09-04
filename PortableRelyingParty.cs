using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Microsoft.IdentityServer.PolicyModel.Configuration;
using Microsoft.IdentityServer.PowerShell.Commands;
using Microsoft.IdentityServer.PowerShell.Resources;
using VflIt.Samples.AdfsSnapIn.Commands;

namespace VflIt.Samples.AdfsSnapIn
{
    [DataContract]
    public class PortableRelyingParty
    {
        public PortableRelyingParty()
        {
            m_RequestSigningCertificate = new List<byte[]>();
            ClaimsAccepted = new List<SerializableClaimDescription>();
            SamlEndpoints = new List<SerializableSamlEndpoint>();
        }

        [DataMember]
        private byte[] m_EncryptionCertificate;

        [DataMember]
        private IList<byte[]> m_RequestSigningCertificate;

        [DataMember]
        public bool AutoUpdateEnabled { get; set; }

        [DataMember]
        public IList<SerializableClaimDescription> ClaimsAccepted { get; set; }

        [DataMember]
        public bool ConflictWithPublishedPolicy { get; set; }

        [DataMember]
        public string DelegationAuthorizationRules { get; set; }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public bool EncryptClaims { get; set; }

        [DataMember]
        public bool EncryptedNameIdRequired { get; set; }

        [IgnoreDataMember]
        public X509Certificate2 EncryptionCertificate
        {
            get
            {
                return m_EncryptionCertificate != null ? new X509Certificate2(m_EncryptionCertificate) : null;
            }
            set
            {
                if (value != null)
                {
                    m_EncryptionCertificate = value.GetRawCertData();
                }
            }
        }

        [DataMember]
        public RevocationSetting EncryptionCertificateRevocationCheck { get; set; }

        [DataMember]
        public IList<string> Identifier { get; set; }

        [DataMember]
        public string ImpersonationAuthorizationRules { get; set; }

        [DataMember]
        public string IssuanceAuthorizationRules { get; set; }

        [DataMember]
        public string IssuanceTransformRules { get; set; }

        [DataMember]
        public DateTime LastMonitoredTime { get; set; }

        [DataMember]
        public bool? LastPublishedPolicyCheckSuccessful { get; set; }

        [DataMember]
        public DateTime LastUpdateTime { get; set; }

        [DataMember]
        public Uri MetadataUrl { get; set; }

        [DataMember]
        public bool MonitoringEnabled { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int NotBeforeSkew { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        public string OrganizationInfo { get; set; }

        [DataMember]
        public string ProtocolProfile { get; set; }

        [IgnoreDataMember]
        public IList<X509Certificate2> RequestSigningCertificate
        {
            get
            {
                return m_RequestSigningCertificate.Select(cert => new X509Certificate2(cert)).ToList();
            }
            set
            {
                var serializableCerts = value.Select(cert => cert.GetRawCertData()).ToList();
                m_RequestSigningCertificate = serializableCerts.ToList();
            }
        }

        [DataMember]
        public IList<SerializableSamlEndpoint> SamlEndpoints { get; set; }

        [DataMember]
        public string SamlResponseSignature { get; set; }

        [DataMember]
        public string SignatureAlgorithm { get; set; }

        [DataMember]
        public bool SignedSamlRequestsRequired { get; set; }

        [DataMember]
        public RevocationSetting SigningCertificateRevocationCheck { get; set; }

        [DataMember]
        public int TokenLifetime { get; set; }

        [DataMember]
        public Uri WSFedEndpoint { get; set; }


        public static PortableRelyingParty FromRelyingPartyTrust(RelyingPartyTrust relyingPartyTrust)
        {
            var source = relyingPartyTrust;
            var target = new PortableRelyingParty();

            target.AutoUpdateEnabled = source.AutoUpdateEnabled;
            target.ClaimsAccepted = (source.ClaimsAccepted ?? new ClaimDescription[0]).Select(SerializableClaimDescription.FromClaimDescription).ToList();
            target.ConflictWithPublishedPolicy = source.ConflictWithPublishedPolicy;
            target.DelegationAuthorizationRules = source.DelegationAuthorizationRules;
            target.Enabled = source.Enabled;
            target.EncryptClaims = source.EncryptClaims;
            target.EncryptedNameIdRequired = source.EncryptedNameIdRequired;
            target.EncryptionCertificate = source.EncryptionCertificate;
            target.EncryptionCertificateRevocationCheck = source.EncryptionCertificateRevocationCheck;
            target.Identifier = source.Identifier;
            target.ImpersonationAuthorizationRules = source.ImpersonationAuthorizationRules;
            target.IssuanceAuthorizationRules = source.IssuanceAuthorizationRules;
            target.IssuanceTransformRules = source.IssuanceTransformRules;
            target.LastMonitoredTime = source.LastMonitoredTime;
            target.LastPublishedPolicyCheckSuccessful = source.LastPublishedPolicyCheckSuccessful;
            target.LastUpdateTime = source.LastUpdateTime;
            target.MetadataUrl = source.MetadataUrl;
            target.MonitoringEnabled = source.MonitoringEnabled;
            target.Name = source.Name;
            target.NotBeforeSkew = source.NotBeforeSkew;
            target.Notes = source.Notes;
            target.OrganizationInfo = source.OrganizationInfo;
            target.ProtocolProfile = source.ProtocolProfile;
            target.RequestSigningCertificate = (source.RequestSigningCertificate ?? new ReadOnlyCollection<X509Certificate2>(new X509Certificate2[0])).ToArray();
            target.SamlEndpoints = (source.SamlEndpoints ?? new SamlEndpoint[0]).Select(SerializableSamlEndpoint.FromSamlEndpoint).ToList();
            target.SamlResponseSignature = source.SamlResponseSignature;
            target.SignatureAlgorithm = source.SignatureAlgorithm;
            target.SignedSamlRequestsRequired = source.SignedSamlRequestsRequired;
            target.SigningCertificateRevocationCheck = source.SigningCertificateRevocationCheck;
            target.TokenLifetime = source.TokenLifetime;
            target.WSFedEndpoint = source.WSFedEndpoint;

            return target;
        }

        public XDocument GenerateFederationMetadata()
        {
            FederationMetadataGenerator f = FederationMetadataGenerator.FromSerializedRelyingParty(this);
            return f.Generate();
        }

 
        public void CopyToRelyingPartyTrust(AddRelyingPartyTrustCommand relyingPartyTrust)
        {
            var source = this;
            var target = relyingPartyTrust;

            target.AutoUpdateEnabled = source.AutoUpdateEnabled;
            target.DelegationAuthorizationRules = source.DelegationAuthorizationRules;
            target.EncryptClaims = source.EncryptClaims;
            target.EncryptedNameIdRequired = source.EncryptedNameIdRequired;
            target.EncryptionCertificate = source.EncryptionCertificate;
            target.EncryptionCertificateRevocationCheck = source.EncryptionCertificateRevocationCheck.ToString();
            target.Identifier = source.Identifier.ToArray();
            target.ImpersonationAuthorizationRules = source.ImpersonationAuthorizationRules;
            target.IssuanceAuthorizationRules = source.IssuanceAuthorizationRules;
            target.IssuanceTransformRules = source.IssuanceTransformRules;
            target.MetadataUrl = source.MetadataUrl;
            target.MonitoringEnabled = source.MonitoringEnabled;
            target.NotBeforeSkew = source.NotBeforeSkew;
            target.Notes = source.Notes;
            target.ProtocolProfile = source.ProtocolProfile;
            target.RequestSigningCertificate = source.RequestSigningCertificate.ToArray();
            target.SamlResponseSignature = source.SamlResponseSignature;
            target.SignatureAlgorithm = source.SignatureAlgorithm;
            target.SignedSamlRequestsRequired = source.SignedSamlRequestsRequired;
            target.TokenLifetime = source.TokenLifetime;
            target.WSFedEndpoint = source.WSFedEndpoint;
            target.SamlEndpoint = source.SamlEndpoints.Select(endpoint => SamlEndpointFactory.Create(endpoint.Protocol, endpoint.Location, endpoint.ResponseLocation, endpoint.Binding, endpoint.IsDefault, endpoint.Index)).ToArray();
            target.SigningCertificateRevocationCheck = source.SigningCertificateRevocationCheck.ToString();
            //target.ClaimsAccepted = source.ClaimsAccepted.Select(SerializableClaimDescription.FromClaimDescription).ToList();
            //target.ConflictWithPublishedPolicy = source.ConflictWithPublishedPolicy;
            //target.LastMonitoredTime = source.LastMonitoredTime;
            //target.LastPublishedPolicyCheckSuccessful = source.LastPublishedPolicyCheckSuccessful;
            //target.LastUpdateTime = source.LastUpdateTime;
            //target.Name = source.Name;
            //target.OrganizationInfo = source.OrganizationInfo;
        }
    }
}