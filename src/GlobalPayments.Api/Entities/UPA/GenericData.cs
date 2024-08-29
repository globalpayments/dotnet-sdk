using GlobalPayments.Api.Entities.Enums;
using System.Collections.Generic;

namespace GlobalPayments.Api.Entities.UPA {
    public class GenericData {
        public PromptMessages Prompts { get; set; }
        public string TextButton1 { get; set; }
        public string TextButton2 { get; set; }
        /// <summary>
        /// List<TextFormat> Indicates the format of the data to be entered.
        /// </summary>
        public List<TextFormat> EntryFormat { get; set; }
        /// <summary>
        /// int Number of decimal places for numeric entry format. This is required if the entry format is numeric
        /// </summary>
        public int? DecimalPlaces { get; set; }
        /// <summary>
        /// int Minimum length for the entry
        /// </summary>
        public int? EntryMinLen { get; set; }
        /// <summary>
        /// InputAlignment Alignment when displaying the inputs. If this is not sent in the packet. The default alignment is LR.
        /// </summary>
        public InputAlignment Alignment { get; set; } = InputAlignment.LEFT_TO_RIGHT;
        /// <summary>
        /// int Maximum length for the entry.
        /// </summary>
        public int? EntryMaxLen { get; set; }
        /// <summary>
        /// int Number of seconds that the message will be displayed. If this parameter is not received, the        
        /// default timeout is the IdleTimeout value set through the settings.
        /// </summary>
        public int? Timeout { get; set; }
    }
}
