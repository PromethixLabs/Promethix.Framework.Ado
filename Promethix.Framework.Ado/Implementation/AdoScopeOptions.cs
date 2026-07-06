using Promethix.Framework.Ado.Enums;
using System.Data;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoScopeOptions
    {
        public AdoScopeOptions()
        {
            JoinOption = AdoScopeOption.JoinExisting;
            ScopeExecutionOption = AdoContextGroupExecutionOption.Standard;
        }

        public AdoScopeOptions(AdoScopeOptions other)
        {
            if (other == null)
            {
                JoinOption = AdoScopeOption.JoinExisting;
                ScopeExecutionOption = AdoContextGroupExecutionOption.Standard;
                return;
            }

            JoinOption = other.JoinOption;
            ScopeExecutionOption = other.ScopeExecutionOption;
            IsolationLevel = other.IsolationLevel;
        }

        public AdoScopeOption JoinOption { get; internal set; }

        public AdoContextGroupExecutionOption ScopeExecutionOption { get; internal set; }

        public IsolationLevel? IsolationLevel { get; internal set; }
    }
}
