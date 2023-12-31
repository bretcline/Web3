// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591

using System;
using System.Linq;
using System.Text;
using SqlSugar;
using WebThree.Shared.Data;

namespace ProtonPack.Data
{
	[SugarTable("EventNfts")]
    public partial class EventNft : IDataObject 
    {
		public EventNft()
		{
			ID = new Guid();
		}

        [SugarColumn(IsPrimaryKey=true, ColumnName="EventNftID")]
		public Guid ID { get; set; } 
        public Guid EventID { get; set; }
        public Guid NftID { get; set; }
        public DateTime CheckinDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }

}
#pragma warning restore 1591
