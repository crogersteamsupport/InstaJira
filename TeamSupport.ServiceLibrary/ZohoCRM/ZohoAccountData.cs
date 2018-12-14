using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TeamSupport.ServiceLibrary
{
	public class ZohoAccountData
	{
		public class Owner
		{
			public string name { get; set; }
			public string id { get; set; }
		}

		public class ModifiedBy
		{
			public string name { get; set; }
			public string id { get; set; }
		}

		public class Approval
		{
			public bool @delegate { get; set; }
			public bool approve { get; set; }
			public bool reject { get; set; }
			public bool resubmit { get; set; }
		}

		public class CreatedBy
		{
			public string name { get; set; }
			public string id { get; set; }
		}

		public class ParentAccount
		{
			public string name { get; set; }
			public string id { get; set; }
		}

		public class Tag
		{
			public string name { get; set; }
			public string id { get; set; }
		}

		public class Data
		{
			public string id { get; set; }
			public string Account_Name { get; set; }
			public string Account_Number { get; set; }
			public Owner Owner { get; set; }
			public string Ownership { get; set; }
			public object Description { get; set; }
			[JsonProperty(PropertyName = "$currency_symbol")]
			public string Currency_Symbol { get; set; }
			public string Account_Type { get; set; }
			public object Rating { get; set; }
			public int? SIC_Code { get; set; }
			public object Website { get; set; }
			public int? Employees { get; set; }
			public DateTime Last_Activity_Time { get; set; }
			public string Industry { get; set; }
			public ModifiedBy Modified_By { get; set; }
			public object Account_Site { get; set; }
			[JsonProperty(PropertyName = "$process_flow")]
			public bool Process_Flow { get; set; }
			public string Phone { get; set; }
			[JsonProperty(PropertyName = "$approved")]
			public bool Approved { get; set; }
			public object Ticker_Symbol { get; set; }
			[JsonProperty(PropertyName = "$approval")]
			public Approval Approval { get; set; }
			public DateTime Modified_Time { get; set; }
			public string Billing_Street { get; set; }
			public string Billing_City { get; set; }
			public string Billing_Code { get; set; }
			public string Billing_State { get; set; }
			public string Billing_Country { get; set; }
			public DateTime Created_Time { get; set; }
			[JsonProperty(PropertyName = "$followed")]
			public bool Followed { get; set; }
			[JsonProperty(PropertyName = "$editable")]
			public bool Editable { get; set; }
			public ParentAccount Parent_Account { get; set; }
			public string Shipping_Street { get; set; }
			public string Shipping_City { get; set; }		
			public string Shipping_Code { get; set; }
			public string Shipping_State { get; set; }
			public string Shipping_Country { get; set; }
			public List<Tag> Tag { get; set; }
			public CreatedBy Created_By { get; set; }
			public string Fax { get; set; }
			public decimal? Annual_Revenue { get; set; }
		}

		public class Info
		{
			public int per_page { get; set; }
			public int count { get; set; }
			public int page { get; set; }
			public bool more_records { get; set; }
			//Used for the CustomFields matching/sync
			[JsonIgnore]
			public string JsonString { get; set; }
		}

		public class Account
		{
			public List<Data> data { get; set; }
			public Info info { get; set; }
		}
	}
}
