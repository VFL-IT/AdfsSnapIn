using System.Collections.Generic;
using System.Text;

namespace VflIt.Samples.AdfsSnapIn
{
    public class ClaimIssuanceRuleSet
    {
        private readonly IEnumerable<ClaimIssuanceRule> m_Rules;

        public ClaimIssuanceRuleSet(IEnumerable<ClaimIssuanceRule> rules)
        {
            m_Rules = rules;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var rule in m_Rules)
            {
                sb.AppendLine(rule.ToString());
            }
            return sb.ToString();
        }
    }
}