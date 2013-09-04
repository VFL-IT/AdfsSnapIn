using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace VflIt.Samples.AdfsSnapIn.Commands
{
    [Cmdlet(VerbsCommon.Add, "ADFSRuleFilteredRole")]
    public class AddFilteredRolesRuleCommand : AddRuleCommand
    {
        // Rule names are not required to be unique
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string RoleFilter { get; set; }

        protected override IEnumerable<ClaimIssuanceRule> GetRulesToAdd()
        {
            var addRolesRuleDefinition = new StringBuilder();
            addRolesRuleDefinition.AppendLine(@"c:[Type == ""http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname"", Issuer == ""AD AUTHORITY""]");
            addRolesRuleDefinition.AppendLine(@" => add(store = ""Active Directory"", types = (""http://schemas.microsoft.com/ws/2008/06/identity/claims/role""), query = "";tokenGroups;{0}"", param = c.Value);");

            var addRolesRule = new ClaimIssuanceRule { Definition = addRolesRuleDefinition.ToString() };
            addRolesRule.Name = "Role - Add all groups to role";
            yield return addRolesRule;

            var filterAndIssueRolesRuleDefinition = new StringBuilder();
            filterAndIssueRolesRuleDefinition.AppendLine(string.Format(@"c:[Type == ""http://schemas.microsoft.com/ws/2008/06/identity/claims/role"", Value =~ ""^(?i){0}""]", RoleFilter));
            filterAndIssueRolesRuleDefinition.AppendLine(@" => issue(claim = c);");

            var filterAndIssueRolesRule = new ClaimIssuanceRule { Definition = filterAndIssueRolesRuleDefinition.ToString() };
            filterAndIssueRolesRule.Name = string.Format("Role - Filter added roles by {0}", RoleFilter);
            filterAndIssueRolesRule.Template = ClaimIssuanceRule.PassthroughTemplateName;
            yield return filterAndIssueRolesRule;
        }
    }
}