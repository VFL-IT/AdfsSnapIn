using System;
using System.Reflection;
using Microsoft.IdentityServer.PowerShell.Resources;

namespace VflIt.Samples.AdfsSnapIn.Commands
{
    /// <summary>
    /// For some reason, importing the endpoints (WS-Fed and SAML alike) does not work when the endpoints are specified in federation metadata
    /// They must be explicitly set as properties on the add command.
    /// However, SamlEndpoint has an internal constructor...
    /// </summary>
    static class SamlEndpointFactory
    {
        public static SamlEndpoint Create(string protocol, Uri location, Uri responseLocation, string binding, bool isDefault, int index)
        {
            // internal SamlEndpoint(string protocol, Uri location, Uri responseLocation, string binding, bool isDefault, int index)
            Type samlEndpointType = typeof(SamlEndpoint);
            var constructor = samlEndpointType.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(string), typeof(Uri), typeof(Uri), typeof(string), typeof(bool), typeof(int) },
                null
                );
            var endpoint = (SamlEndpoint) constructor.Invoke(new object[] {protocol, location, responseLocation, binding, isDefault, index});
            return endpoint;
        }
    }
}