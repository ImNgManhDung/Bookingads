using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Common.Models.Message
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "<Pending>")]
    public class MessageRequest
    {
        public string Role { get; set; }

        public string Content { get; set; }
    }

    public class ViewChatGPTRequest
    {
        public string Model { get; set; }

        public IEnumerable<MessageRequest> Messages { get; set; }
    }
}