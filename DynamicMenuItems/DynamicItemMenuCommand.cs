using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicMenuItems
{
    public class DynamicItemMenuCommand: OleMenuCommand
    {
        private Predicate<int> _matches;

        public DynamicItemMenuCommand(
            CommandID rootId, 
            Predicate<int> matches, 
            EventHandler invokeHandler,
            EventHandler beforeQueryStatusHandler)
            : base(invokeHandler,null, beforeQueryStatusHandler, rootId)
        {
            if(matches == null)
            {
                throw new ArgumentException(nameof(matches));
            }

            _matches = matches;
        }

        public override bool DynamicItemMatch(int cmdId)
        {
            if(_matches(cmdId))
            {
                this.MatchedCommandId = cmdId;
                return true;
            }

            this.MatchedCommandId = 0;
            return false;
        }

    }
}
