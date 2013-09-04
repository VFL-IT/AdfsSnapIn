using System;
using System.Xml.Linq;

namespace VflIt.Samples.AdfsSnapIn
{
    public class SamlFederationMetadataGenerator : FederationMetadataGenerator
    {
        public SamlFederationMetadataGenerator(PortableRelyingParty portableRelyingParty) : base(portableRelyingParty) {}

        public override XDocument Generate()
        {
            if (!IsSamlRp())
            {
                throw new InvalidOperationException("Can only generate metadata for SAML-P RPs");
            }
            XDocument document = GetEntityDescriptor();
            var entityDescriptor = document.Root;

            var ssoDescriptor = GetSpSsoDescriptor();
            entityDescriptor.Add(ssoDescriptor);

            AddCertificates(ssoDescriptor);

            foreach (var endpoint in PortableRelyingParty.SamlEndpoints)
            {
                ssoDescriptor.Add(GetSamlEndpoint(endpoint));
            }
            return document;
        }

        protected XElement GetSpSsoDescriptor()
        {
            var cont = string.Format(
                @"<md:SPSSODescriptor xmlns:md=""urn:oasis:names:tc:SAML:2.0:metadata"" AuthnRequestsSigned=""{0}"" WantAssertionsSigned=""true"" protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"" />",
                CanSignRequests().ToString().ToLowerInvariant()
                );
            var spSsoDescriptor = XElement.Parse(cont);
            return spSsoDescriptor;
        }


        protected XElement GetSamlEndpoint(SerializableSamlEndpoint endpoint)
        {
            var cont = string.Format(@"<md:{0} xmlns:md=""urn:oasis:names:tc:SAML:2.0:metadata"" Binding=""{1}"" Location=""{2}"" ResponseLocation=""{3}"" index=""{4}"" isDefault=""{5}""/>",
                                     ParseProtocol(endpoint.Protocol), endpoint.BindingUri, endpoint.Location, endpoint.ResponseLocation, endpoint.Index,
                                     endpoint.IsDefault.ToString().ToLowerInvariant());
            return XElement.Parse(cont);
        }

        private string ParseProtocol(string protocol)
        {
            switch (protocol)
            {
                case "SAMLAssertionConsumer":
                    return "AssertionConsumerService";
                case "SAMLLogout":
                    return "SingleLogoutService";
                default:
                    throw new InvalidOperationException(string.Format("Unsupported SAML protocol: {0}", protocol));
            }
        }
    }
}
 