using System.Collections.Generic;

namespace VflIt.Samples.AdfsSnapIn
{
    public class ClaimIssuanceRuleParser
    {
        private string[] m_RuleLines;

        public ClaimIssuanceRuleParser(string ruleString)
        {
            m_RuleLines = ruleString.Split('\n');
        }

        private const string RuleNameLineType = "@RuleName";
        private const string RuleTemplateLineType = "@RuleTemplate";


        public IEnumerable<ClaimIssuanceRule> Parse()
        {
            ClaimIssuanceRule claimIssuanceRule = null;
            string currentLine = null;
            string previousLine = null;
            string nextLine = null;
            foreach (var ruleLine in RuleLinesWithEndPadded())
            {
                nextLine = ruleLine;
                if (ShouldNewObject(currentLine, previousLine))
                {
                    claimIssuanceRule = new ClaimIssuanceRule();
                }
                if (IsTemplateLine(currentLine))
                {
                    claimIssuanceRule.Template = GetValue(RuleTemplateLineType, currentLine);
                } 
                else if (IsNameLine(currentLine))
                {
                    claimIssuanceRule.Name = GetValue(RuleNameLineType, currentLine);
                }
                else if (IsRuleLine(currentLine))
                {
                    claimIssuanceRule.Definition += currentLine;
                }
                if (ShouldYieldObject(currentLine, nextLine))
                {
                    yield return claimIssuanceRule;
                }
                previousLine = currentLine;
                currentLine = nextLine;
            }
          
        }

        private bool IsRuleLine(string currentLine)
        {
            return currentLine != null;
        }

        private const string EndDelimiter = "&&&&&&&&&&&&&&&&END&&&&&&&&&&&&&";

        private IEnumerable<string> RuleLinesWithEndPadded()
        {
            var ruleLines = new List<string>();
            ruleLines.AddRange(m_RuleLines);
            ruleLines.Add(EndDelimiter);
            return ruleLines;
        }

        private static bool IsNameLine(string line)
        {
            return (line ?? "").Trim().StartsWith(RuleNameLineType);
        }

        private static bool IsTemplateLine(string line)
        {
            return (line ?? "").Trim().StartsWith(RuleTemplateLineType);
        }

        private static bool IsEndLine(string line)
        {
            return line == EndDelimiter;
        }

        private static string GetValue(string lineType, string ruleLine)
        {
            return ruleLine.Replace(lineType + " = ", "").Trim(new[] { ' ', '"', '\n', '\r' });
        }

        private bool ShouldYieldObject(string currentLine, string nextLine)
        {
            if (currentLine == null)
            {
                return false;
            }
            if (IsTemplateLine(nextLine))
            {
                return true;
            }
            if (!IsTemplateLine(currentLine) && IsNameLine(nextLine))
            {
                return true;
            }
            if (IsEndLine(nextLine))
            {
                return true;
            }
            return false;
        }

        private bool ShouldNewObject(string currentLine, string previousLine)
        {
            if (currentLine == null)
            {
                return true;
            }
            if (IsTemplateLine(currentLine))
            {
                return true;
            }
            if (!IsTemplateLine(previousLine) && IsNameLine(currentLine))
            {
                return true;
            }
            return false;
        }
    }
}
