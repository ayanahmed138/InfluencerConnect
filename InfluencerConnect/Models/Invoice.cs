using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfluencerConnect.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public int ChatId { get; set; }

        public string BankAccountTitle { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }

        public decimal Amount { get; set; }
        public string Description { get; set; }

        public string PdfPath { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}