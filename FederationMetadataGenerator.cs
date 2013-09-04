using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace VflIt.Samples.AdfsSnapIn
{
    public class FederationMetadataGenerator
    {
        protected readonly PortableRelyingParty PortableRelyingParty;

        protected FederationMetadataGenerator(PortableRelyingParty portableRelyingParty)
        {
            PortableRelyingParty = portableRelyingParty;
        }

        public static FederationMetadataGenerator FromSerializedRelyingParty(PortableRelyingParty portableRelyingParty)
        {
            if (IsSamlRp(portableRelyingParty))
            {
                return new SamlFederationMetadataGenerator(portableRelyingParty);        
            }
            return new WsFedFederationmetadataGenerator(portableRelyingParty);
        }

        public virtual XDocument Generate()
        {
            throw new NotImplementedException();
        }

        private static bool IsSamlRp(PortableRelyingParty portableRelyingParty)
        {
            return portableRelyingParty.SamlEndpoints.Any();
        }

        protected bool IsSamlRp()
        {
            return IsSamlRp(PortableRelyingParty);
        }

        protected bool CanSignRequests()
        {
            return PortableRelyingParty.RequestSigningCertificate.Any();
        }

        protected bool CanEncryptRequests()
        {
            return PortableRelyingParty.EncryptionCertificate != null;
        }

        protected XElement GetEndpointReference(string endpointReference)
        {
            var cont =
                string.Format(
                    @"<EndpointReference xmlns:fed=""http://docs.oasis-open.org/wsfed/federation/200706"" xmlns=""http://www.w3.org/2005/08/addressing""><Address>{0}</Address></EndpointReference>",
                    endpointReference);
            return XElement.Parse(cont);
        }

        protected XDocument GetEntityDescriptor()
        {
            return XDocument.Parse(
                string.Format(
                    @"<md:EntityDescriptor xmlns:md=""urn:oasis:names:tc:SAML:2.0:metadata"" xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion"" xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"" entityID=""{0}""/>",
                    PortableRelyingParty.Identifier.First()));
        }

        protected static XElement GetKeyElement(string use, X509Certificate2 key)
        {
            return XElement.Parse(string.Format(
                @"<md:KeyDescriptor xmlns:md=""urn:oasis:names:tc:SAML:2.0:metadata"" use=""{0}""><ds:KeyInfo xmlns:ds=""http://www.w3.org/2000/09/xmldsig#""><ds:X509Data><ds:X509Certificate>{1}</ds:X509Certificate></ds:X509Data></ds:KeyInfo></md:KeyDescriptor>",
                use,
                Convert.ToBase64String(key.GetRawCertData())));
        }

        protected void AddCertificates(XElement root)
        {
            if (CanSignRequests())
            {
                foreach (var key in PortableRelyingParty.RequestSigningCertificate)
                {
                    root.Add(GetKeyElement("signing", key));
                }
            }
            if (CanEncryptRequests())
            {
                root.Add(GetKeyElement("encryption", PortableRelyingParty.EncryptionCertificate));
            }
        }
    }
}