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
	[SugarTable("IpfsItems")]
    public partial class IpfsItem : IDataObject 
    {
		public IpfsItem()
		{
			ID = new Guid();
		}

        [SugarColumn(IsPrimaryKey=true, ColumnName="IpfsItemID")]
		public Guid ID { get; set; }
        [JsonPropertyName("name")]
        public string ItemName { get; set; }
        [JsonPropertyName("description")] 
        public string ItemDescription { get; set; }
        [JsonPropertyName("image")]
        public string ItemImage { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Deleted { get; set; }
        public Guid? AssetID { get; set; }
        public Guid ContractTypeID { get; set; }
        public string ItemIndex { get; set; }
        public bool OnChain { get; set; }
    }
}
#pragma warning restore 1591
