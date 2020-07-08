using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SupportBank
{
    public class XmlReadWriter : IReadWriter
    {
        private static DateTime baseDate = new DateTime(1900, 1, 1);
        
        public List<Transaction> Read(string filename)
        {
            var transactions = new List<Transaction>();
            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(File.ReadAllText(filename));
            XmlNodeList nodes = xmldoc.DocumentElement.SelectNodes("/TransactionList/SupportTransaction");
            foreach (XmlNode node in nodes)
            {
                var Narrative = node.SelectSingleNode("Description").InnerText;
                var Amount = decimal.Parse(node.SelectSingleNode("Value").InnerText);
                var From = node.SelectSingleNode("Parties/From").InnerText;
                var To = node.SelectSingleNode("Parties/To").InnerText;
                var Date =  baseDate.AddDays(Int32.Parse(node.Attributes[0].Value));
                var t = new Transaction(Date, From, To, Narrative, Amount);
                transactions.Add(t);
            }

            return transactions;
        }

        public void Write(string filename, HashSet<Transaction> transactions)
        {
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
            using (XmlWriter xmlWriter = XmlWriter.Create(filename, settings))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("TransactionList");
                foreach (var transaction in transactions)
                {
                    xmlWriter.WriteStartElement("SupportTransaction");
                    xmlWriter.WriteAttributeString("Date", (transaction.Date - baseDate).Days.ToString());
                    xmlWriter.WriteStartElement("Description");
                    xmlWriter.WriteString(transaction.Narrative);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Value");
                    xmlWriter.WriteString(transaction.Amount.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Parties");
                    xmlWriter.WriteStartElement("From");
                    xmlWriter.WriteString(transaction.From);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("To");
                    xmlWriter.WriteString(transaction.To);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
        }
    }
}