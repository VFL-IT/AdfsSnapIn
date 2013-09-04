using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Microsoft.IdentityServer.PowerShell.Commands;
using Microsoft.IdentityServer.PowerShell.Resources;
using Microsoft.PowerShell.Commands;

namespace VflIt.Samples.AdfsSnapIn.Commands
{
    [Cmdlet("Export", "ADFSPortableRelyingParty")]
    public class ExportPortableRelyingParty : PSCmdlet
    {
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string Name { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string Path { get; set; }

        protected override void ProcessRecord()
        {
            var relyingPartyTrustCommand = new GetRelyingPartyTrustCommand();
            relyingPartyTrustCommand.Name = new[] { Name };
            var relyingPartyTrust = relyingPartyTrustCommand.Invoke<RelyingPartyTrust>().Single();
            var portableRelyingParty = PortableRelyingParty.FromRelyingPartyTrust(relyingPartyTrust);
            var path = new FileInfo(SessionState.Path.GetUnresolvedProviderPathFromPSPath(Path));
            if (path.Exists)
            {
                path.Delete();
            }
            using (var stream = path.Create())
            {
                var serializer = new DataContractSerializer(typeof(PortableRelyingParty));
                serializer.WriteObject(stream, portableRelyingParty);
            }
            FormatXml(path);
        }

        private static void FormatXml(FileInfo path)
        {
            XDocument document = XDocument.Load(path.FullName);
            document.Save(path.FullName);
        }
    }
}