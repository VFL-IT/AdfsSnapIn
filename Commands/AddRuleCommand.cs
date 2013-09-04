using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.IdentityServer.PowerShell.Commands;

namespace VflIt.Samples.AdfsSnapIn.Commands
{
    public abstract class AddRuleCommand : Cmdlet
    {
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string Name { get; set; }

        protected abstract IEnumerable<ClaimIssuanceRule> GetRulesToAdd();

        protected override void ProcessRecord()
        {
            GetRulesCommand c = new GetRulesCommand();
            c.Name = Name;
            var rules = c.Invoke<ClaimIssuanceRule>().Where(rule => !string.IsNullOrEmpty((rule.Definition ?? "").Trim())).ToList();

            
            rules.AddRange(GetRulesToAdd());

            var ruleSet = new ClaimIssuanceRuleSet(rules);

            var serializedRuleSet = ruleSet.ToString();

            var setRelyingPartyTrustCommand = new SetRelyingPartyTrustCommand
                                                  {
                                                      TargetName = Name, 
                                                      IssuanceTransformRules = serializedRuleSet
                                                  };
            var result = setRelyingPartyTrustCommand.Invoke();
            foreach (object o in result)
            {
                WriteObject(o);
            }
        }
    }
}