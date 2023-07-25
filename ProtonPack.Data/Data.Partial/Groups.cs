using SqlSugar;

namespace ProtonPack.Data
{
    public partial class Group
    {
        [SugarColumn(IsIgnore = true)]
        public List<Rule> Rules { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<Customer> Customers { get; set; }


    }
}
