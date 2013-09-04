using System.Management.Automation;
using System.Text;

namespace VflIt.Samples.AdfsSnapIn.Commands
{
    [Cmdlet(VerbsCommon.Add, "ADFSRuleNameId")]
    public class AddNameIdRuleCommand : AddRuleCommand
    {
        protected override System.Collections.Generic.IEnumerable<ClaimIssuanceRule> GetRulesToAdd()
        {
            var nameIdRuleDefinition = new StringBuilder();
            nameIdRuleDefinition.AppendLine(@"c:[Type == ""http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname"", Issuer == ""AD AUTHORITY""]");
            nameIdRuleDefinition.AppendLine(@" => issue(store = ""Active Directory"", types = (""http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier""), query = "";userPrincipalName;{0}"", param = c.Value);");

            var nameIdRule = new ClaimIssuanceRule {Name = "Issue NameID", Definition = nameIdRuleDefinition.ToString()};
            yield return nameIdRule;
        }
    }
}