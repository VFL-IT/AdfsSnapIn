using System.Text;

namespace VflIt.Samples.AdfsSnapIn
{
    public class ClaimIssuanceRule
    {
        public const string CustomTemplateName = "CustomTemplateName";
        public const string PassthroughTemplateName = "PassThroughClaims";

        public ClaimIssuanceRule()
        {
            Template = CustomTemplateName;
        }

        public string Name { get; set; }

        public string Definition { get; set; }

        public string Template { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Template != CustomTemplateName)
            {
                sb.AppendLine(string.Format(@"@RuleTemplate = ""{0}""", Template));
            }
            sb.AppendLine(string.Format(@"@RuleName = ""{0}""", Name));
            sb.AppendLine(Definition);
            return sb.ToString();
        }
    }
}