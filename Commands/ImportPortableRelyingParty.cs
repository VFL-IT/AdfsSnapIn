using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml.Linq;
using Microsoft.IdentityServer.PowerShell.Commands;
using Microsoft.IdentityServer.PowerShell.Resources;

namespace VflIt.Samples.AdfsSnapIn.Commands
{
    [Cmdlet("Import", "ADFSPortableRelyingParty")]
    public class ImportPortableRelyingParty : PSCmdlet
    {
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string Name { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string Path { get; set; }

        [Parameter]
        public SwitchParameter PassThru { get; set; }

        protected override void ProcessRecord()
        {
            var getRelyingPartyTrustCommand = new GetRelyingPartyTrustCommand();
            getRelyingPartyTrustCommand.Name = new[] { Name };
            var exists = getRelyingPartyTrustCommand.Invoke<RelyingPartyTrust>().Any();
            if (exists)
            {
                var error = "Relying party already exists";
                ThrowTerminatingError(new ErrorRecord(new Exception(error), error, ErrorCategory.ResourceExists, Name));
            }
            PortableRelyingParty portableRelyingParty;
            var path = SessionState.Path.GetUnresolvedProviderPathFromPSPath(Path);
            using (var stream = File.OpenRead(path))
            {
                var serializer = new DataContractSerializer(typeof(PortableRelyingParty));
                portableRelyingParty = (PortableRelyingParty)serializer.ReadObject(stream);
            }
            
            var generator = FederationMetadataGenerator.FromSerializedRelyingParty(portableRelyingParty);
            var federationMetadata = generator.Generate();

            var addRelyingPartyTrustCommand = new AddRelyingPartyTrustCommand
                                                  {
                                                      Name = Name,
                                                      MetadataFile = GetTempMetadataFile(federationMetadata),
                                                      PassThru = PassThru
                                                  };
            portableRelyingParty.CopyToRelyingPartyTrust(addRelyingPartyTrustCommand);
            // Cannot clean up metadata, as that seems to introduce a timing issue, preventing the RP from being created correctly

            var rp = addRelyingPartyTrustCommand.Invoke<RelyingPartyTrust>().ToArray();
            
            if (PassThru)
            {
                foreach (var relyingPartyTrust in rp)
                {
                    WriteObject(relyingPartyTrust);
                }
            }
        }

        private string GetTempMetadataFile(XDocument federationMetadata)
        {
            var path = System.IO.Path.GetTempFileName();
            File.WriteAllText(path,federationMetadata.ToString());
            return path;
        }
    }
}