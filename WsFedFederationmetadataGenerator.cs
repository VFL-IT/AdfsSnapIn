using System.Xml.Linq;

namespace VflIt.Samples.AdfsSnapIn
{
    public class WsFedFederationmetadataGenerator : FederationMetadataGenerator
    {
        public WsFedFederationmetadataGenerator(PortableRelyingParty portableRelyingParty) : base(portableRelyingParty) {}

        public override XDocument Generate()
        {
            XDocument document = GetEntityDescriptor();
            var entityDescriptor = document.Root;

            var roleDescriptor = GetRoleDescriptor();
            entityDescriptor.Add(roleDescriptor);

            AddCertificates(roleDescriptor);

            var targetScopes = GetTargetScopes();
            roleDescriptor.Add(targetScopes);

            foreach (var endpointReference in PortableRelyingParty.Identifier)
            {
                targetScopes.Add(GetEndpointReference(endpointReference));
            }
            if (IsPassiveWsFed())
            {
                var passiveRequestorEndpoint = GetPassiveRequestorEndpoint();
                roleDescriptor.Add(passiveRequestorEndpoint);
                passiveRequestorEndpoint.Add(GetEndpointReference(PortableRelyingParty.WSFedEndpoint.ToString()));
            }
            return document;
        }

        private bool IsPassiveWsFed()
        {
            return PortableRelyingParty.WSFedEndpoint != null;
        }

        protected XElement GetPassiveRequestorEndpoint()
        {
            var cont =
                @"<fed:PassiveRequestorEndpoint xmlns:fed=""http://docs.oasis-open.org/wsfed/federation/200706"" />";
            return XElement.Parse(cont);
        }

        protected XElement GetRoleDescriptor()
        {
            var cont =
                @"<md:RoleDescriptor xmlns:md=""urn:oasis:names:tc:SAML:2.0:metadata"" xsi:type=""fed:ApplicationServiceType"" xmlns:fed=""http://docs.oasis-open.org/wsfed/federation/200706"" protocolSupportEnumeration=""http://docs.oasis-open.org/wsfed/federation/200706"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""/>";
            return XElement.Parse(cont);
        }

        protected XElement GetTargetScopes()
        {
            var cont =
                @"<fed:TargetScopes xmlns:fed=""http://docs.oasis-open.org/wsfed/federation/200706"" />";
            return XElement.Parse(cont);
        }
    }
}