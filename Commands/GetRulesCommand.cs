using System.Linq;
using System.Management.Automation;
using Microsoft.IdentityServer.PowerShell.Commands;
using Microsoft.IdentityServer.PowerShell.Resources;

namespace VflIt.Samples.AdfsSnapIn.Commands
{
    [Cmdlet(VerbsCommon.Get, "ADFSRules")]
    public class GetRulesCommand : Cmdlet   
    {
        [Parameter(Mandatory = true, ParameterSetName = "Name", Position = 0, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            var relyingPartyTrustCommand = new GetRelyingPartyTrustCommand();
            relyingPartyTrustCommand.Name = new[] {Name};
            var relyingPartyTrust = relyingPartyTrustCommand.Invoke<RelyingPartyTrust>().Single();
            var p = new ClaimIssuanceRuleParser(relyingPartyTrust.IssuanceTransformRules);
            foreach (var rule in p.Parse())
            {
                WriteObject(rule);
            }
        }
    }
}