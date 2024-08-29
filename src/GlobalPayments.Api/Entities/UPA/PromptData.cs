using System.Collections.Generic;

namespace GlobalPayments.Api.Entities.UPA {
    public class PromptData {
        public PromptMessages Prompts { get; set; }       
        public List<Button> Buttons { get; set; }        
        public int? Timeout { get; set; } = 0;             
        /// <summary>
        /// List of text which will be displayed in Menu 1 to Menu 6. This should contain at least 2 items and a maximum of 6 items.
        /// </summary>
        public List<string> Menu { get; set; }
    }
}
