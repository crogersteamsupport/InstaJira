using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TeamSupport.ServiceLibrary
{
	public class ZohoAccountProducts
	{
		public class Data
		{
			public string id { get; set; }
			public string Product_Name { get; set; }
			public string Description { get; set; }
			public string Product_Category { get; set; }
			public string Product_Code { get; set; }
			public int? Qty_in_Demand { get; set; }
			public ZohoAccountData.Owner Owner { get; set; }
			[JsonProperty(PropertyName = "$currency_symbol")]
			public string Currency_Symbol { get; set; }
			public string Vendor_Name { get; set; }
			public DateTime? Sales_Start_Date { get; set; }
			public List<object> Tax { get; set; }
			public bool Product_Active { get; set; }
			public ZohoAccountData.ModifiedBy Modified_By { get; set; }
			[JsonProperty(PropertyName = "$process_flow")]
			public bool Process_Flow { get; set; }
			public string Manufacturer { get; set; }
			public DateTime? Support_Expiry_Date { get; set; }
			[JsonProperty(PropertyName = "$approved")]
			public bool Approved { get; set; }
			[JsonProperty(PropertyName = "$approval")]
			public ZohoAccountData.Approval Approval { get; set; }
			public DateTime Modified_Time { get; set; }
			public DateTime Created_Time { get; set; }
			public int? Commission_Rate { get; set; }
			[JsonProperty(PropertyName = "$followed")]
			public bool Followed { get; set; }
			public object Handler { get; set; }
			[JsonProperty(PropertyName = "$editable")]
			public bool Editable { get; set; }
			public DateTime? Support_Start_Date { get; set; }
			public string Usage_Unit { get; set; }
			public int? Qty_Ordered { get; set; }
			public int? Qty_in_Stock { get; set; }
			public ZohoAccountData.CreatedBy Created_By { get; set; }
			public List<ZohoAccountData.Tag> Tag { get; set; }
			public DateTime? Sales_End_Date { get; set; }
			public decimal? Unit_Price { get; set; }
			public bool Taxable { get; set; }
			public int? Reorder_Level { get; set; }
		}

		public class Product
		{
			public List<Data> data { get; set; }
			public ZohoAccountData.Info info { get; set; }
		}
	}
}
