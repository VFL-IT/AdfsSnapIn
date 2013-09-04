using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace VflIt.Samples.AdfsSnapIn.Commands
{
    [Cmdlet(VerbsCommon.Add, "ADFSRuleName")]
    public class AddNameRuleCommand : AddRuleCommand
    {

        protected override IEnumerable<ClaimIssuanceRule>  GetRulesToAdd()
        {
 	        var nameRuleDefinition = new StringBuilder();
            nameRuleDefinition.AppendLine(@"c:[Type == ""http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname"", Issuer == ""AD AUTHORITY""]");
            nameRuleDefinition.AppendLine(@"=> issue(store = ""Active Directory"", types = (""http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name""), query = "";displayName;{0}"", param = c.Value);");

            var nameRule = new ClaimIssuanceRule { Name = "Issue Name", Definition = nameRuleDefinition.ToString()};
            yield return nameRule;
        }
    }
}