using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Common.Models.Message
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "<Pending>")]
    public class MessageResponse
    {
        public string Role { get; set; }

        public string Content { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "<Pending>")]
    public class Choice
    {
        public long Index { get; set; }

        public MessageResponse Message { get; set; }
    }

    public class ViewChatGPTResponse
    {
        public string Id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "<Pending>")]
        public string Object { get; set; }

        public long Created { get; set; }

        public string Model { get; set; }

        public IList<Choice> Choices { get; set; }
    }
}