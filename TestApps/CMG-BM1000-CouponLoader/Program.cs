// See https://aka.ms/new-console-template for more information
using ExcelDataReader;
using static WebThree.Shared.Utilities;
using WebThree.Shared;
using ProtonPack.BusinessLogic;
using System.Collections.Generic;
using System.Linq;
using ProtonPack.Data;

Console.WriteLine("Hello, World!");


var cu = new CompanyUser { CompanyId = new Guid("C8494FD1-D34A-461F-A176-B79BB578C17F"), CompanyOnly = true };
using var manager = new AssetManager(cu);

var assets = manager.GetByAssetType(AssetTypes.BM1000);
var newAssets = new List<Asset>();
foreach( var asset in assets)
{
    var data = System.Text.Json.JsonSerializer.Deserialize<List<string>>(asset.AssetData);
    if( null != data )
    {
        var child = manager.Create();
        child.ParentAssetID = asset.ID;
        child.AssetData = data[0];
        child.AssetNumber = data[1];
        child.AssetTypeID = AssetTypes.MurrayCoin;
        manager.Add(child).GetAwaiter().GetResult();
    }
}



using (var stream = File.Open("C:\\Temp\\BFM\\BM-NFT-Code.xlsx", FileMode.Open, FileAccess.Read))
{
    // Auto-detect format, supports:
    //  - Binary Excel files (2.0-2003 format; *.xls)
    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
    using (var reader = ExcelReaderFactory.CreateReader(stream))
    {
        var rowNumber = 0;
        do
        {
            reader.Read(); // Header line
            while (reader.Read())
            {
                var coupon = reader.GetString(1);
                var url = reader.GetString(2);

                var asset = assets.FirstOrDefault( a => a.AssetNumber == rowNumber.ToString() && a.AssetTypeID == AssetTypes.BM1000 );
                if (null == asset)
                {
                    asset = manager.Create();
                    asset.AssetNumber = rowNumber.ToString();
                    asset.AssetData = System.Text.Json.JsonSerializer.Serialize(new List<string> { url, coupon });
                    asset.AssetTypeID = AssetTypes.BM1000;
                    manager.Add(asset).GetAwaiter().GetResult();
                }
                else
                {
                    asset.AssetNumber = rowNumber.ToString();
                    asset.AssetData = System.Text.Json.JsonSerializer.Serialize(new List<string> { url, coupon });
                    asset.AssetTypeID = AssetTypes.BM1000;
                    manager.Update(asset).GetAwaiter().GetResult();
                }
                rowNumber++;
            }
        } while (reader.NextResult());
    }
}
