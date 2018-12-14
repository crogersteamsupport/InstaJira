using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TeamSupport.ServiceLibrary
{
	public class ZohoContactData
	{
		public class Data
		{
			public string id { get; set; }
			public string First_Name { get; set; }
			public string Last_Name { get; set; }
			public string Full_Name { get; set; }
			public AccountName Account_Name { get; set; }
			public DateTime? Date_of_Birth { get; set; }
			public string Mailing_Street { get; set; }
			public string Mailing_City { get; set; }
			public string Mailing_State { get; set; }
			public string Mailing_Zip { get; set; }
			public string Mailing_Country { get; set; }
			public string Mobile { get; set; }
			public string Phone { get; set; }
			public string Home_Phone { get; set; }
			public string Email { get; set; }
			public string Secondary_Email { get; set; }
			public string Fax { get; set; }
			public string Other_Street { get; set; }
			public string Other_City { get; set; }
			public string Other_State { get; set; }
			public string Other_Zip { get; set; }
			public string Other_Country { get; set; }
			public string Other_Phone { get; set; }
			public string Twitter { get; set; }
			public string Salutation { get; set; }
			public ZohoAccountData.Owner Owner { get; set; }
			public string Description { get; set; }
			[JsonProperty(PropertyName = "$currency_symbol")]
			public string Currency_Symbol { get; set; }
			public string Vendor_Name { get; set; }
			public string Reports_To { get; set; }
			public DateTime? Last_Activity_Time { get; set; }
			public string Asst_Phone { get; set; }
			public string Department { get; set; }
			public ZohoAccountData.ModifiedBy Modified_By { get; set; }
			public string Skype_ID { get; set; }
			[JsonProperty(PropertyName = "$process_flow")]
			public bool Process_Flow { get; set; }
			public string Assistant { get; set; }
			public bool Email_Opt_Out { get; set; }
			[JsonProperty(PropertyName = "$approved")]
			public bool Approved { get; set; }
			[JsonProperty(PropertyName = "$approval")]
			public ZohoAccountData.Approval Approval { get; set; }
			public DateTime Modified_Time { get; set; }
			public DateTime Created_Time { get; set; }
			[JsonProperty(PropertyName = "$followed")]
			public bool Followed { get; set; }
			public string Title { get; set; }
			[JsonProperty(PropertyName = "$editable")]
			public bool Editable { get; set; }
			public string Lead_Source { get; set; }
			public List<ZohoAccountData.Tag> Tag { get; set; }
			public ZohoAccountData.CreatedBy Created_By { get; set; }
			[JsonProperty(PropertyName = "Phone Ext")]
			public string TenmastExtension { get; set; }
		}

		public class AccountName
		{
			public string name { get; set; }
			public string id { get; set; }
		}

		public class Contact
		{
			public List<Data> data { get; set; }
			public ZohoAccountData.Info info { get; set; }
		}
	}
}
