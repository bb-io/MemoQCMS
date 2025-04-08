using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.MemoQCMS.Models.Responses.Orders
{
    public class OrderResponse
    {
        [Display("Order ID")]
        public string OrderId { get; set; }

        public string Name { get; set; }

        [Display("Callback URL")]
        public string CallbackUrl { get; set; }

        [Display("Date of creation")]
        public DateTime TimeCreated { get; set; }

        public DateTime? Deadline { get; set; }

        public string Status { get; set; }

        [Display("Jobs completion")]
        public string JobsCompletion { get; set; }
    }
}
