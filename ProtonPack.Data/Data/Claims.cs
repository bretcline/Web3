// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591

using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using SqlSugar;
using WebThree.Shared.Data;

namespace ProtonPack.Data
{
	[SugarTable("Claims")]
    public partial class Claim : IDataObject 
    {
		public Claim()
		{
			ID = new Guid();
		}

        [SugarColumn(IsPrimaryKey=true, ColumnName="ClaimID")]
		public Guid ID { get; set; } 
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("descripton")]
        public string Description { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("tokenId")]
        public string TokenID { get; set; }
        public Guid? ClaimTypeID { get; set; }
        public Guid? ParentAssetID { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("claimed")]
        public bool Claimed { get; set; }
        public Guid? ContractTypeID { get; set; }
        public string? OrderID { get; set; }
        public bool Deleted { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

}
#pragma warning restore 1591