using manaslearning.T4.Templates.Models;
using System.Collections.Generic;

namespace manaslearning.T4.Templates
{
    public partial class InvoiceTemplate
    {
        public Invoice invoice;

        public InvoiceTemplate(Invoice invoice)
        {
            this.invoice = invoice;
        }
    }

    public class Invoice
    {
        /// <summary>
        /// InvoiceNumber is same as OrderNumber
        /// </summary>
        public string InvoiceNumber { get; set; }
        /// <summary>
        /// InvoiceDate is same as Payment date
        /// </summary>
        public string InvoiceDate { get; set; }
        public Company Company { get; set; }
        public Customer Customer { get; set; }

        public bool IsEmail { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; }
    }

    public class InvoiceItem
    {
        public int Sno { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string PurchasedDate { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GSTRate { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string TotalAmountInWords { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
        public Address CustomerAddress { get; set; }
    }
}
