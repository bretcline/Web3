using SqlSugar;
using System.Text.Json.Serialization;

namespace ProtonPack.Data
{



    public partial class IpfsItem 
    {
        [SugarColumn(IsIgnore = true)]
        [JsonPropertyName("attributes")]
        public List<IpfsAttribute> Attributes { get; set; }
    }

    public partial class IpfsAttribute 
    {
        //public override bool Equals(object? obj)
        //{
        //    var item = obj as IpfsAttribute;
        //    if( item == null)
        //        return false;
        //    else if (this.AttributeType.CompareTo(item.AttributeType) != 0)
        //        return false;
        //    else if (this.AttributeValue.CompareTo(item.AttributeValue) != 0)
        //        return false;
        //    else if (this.AttributeType.CompareTo(item.AttributeType) == 0 && this.AttributeValue.CompareTo(item.AttributeValue) == 0 )
        //        return true;
        //    else 
        //        return false;
        //}
    }
}
