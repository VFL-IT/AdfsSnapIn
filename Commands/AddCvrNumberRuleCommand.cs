using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace VflIt.Samples.AdfsSnapIn.Commands
{
    [Cmdlet(VerbsCommon.Add, "ADFSRuleCvrNumber")]
    public class AddCvrNumberRuleCommand : AddRuleCommand
    {

        protected override IEnumerable<ClaimIssuanceRule>  GetRulesToAdd()
        {
 	        var cvrNumberRuleDefinition = new StringBuilder();
            cvrNumberRuleDefinition.AppendLine(@"c:[Type == ""http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname"", Issuer == ""AD AUTHORITY""]");
            cvrNumberRuleDefinition.AppendLine(@" => issue(store = ""DliAttributeStore"", types = (""http://claims.dlbr.dk/2012/02/cvrnumber""), query = ""CvrNumber"", param = c.Value);");

            var cvrNumberRule = new ClaimIssuanceRule { Name = "Issue CVR Number", Definition = cvrNumberRuleDefinition.ToString()};
            yield return cvrNumberRule;
        }
    }
}