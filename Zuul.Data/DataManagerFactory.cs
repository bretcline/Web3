

// This file was automatically generated by the PetaPoco T4 Template
// Do not make changes directly to this file - edit the template instead
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: `Zuul-T4`
//     Provider:               `System.Data.SqlClient`
//     Connection String:      `Server=tcp:venkman-dev.database.windows.net,1433;Database=Zuul-DEV;User ID=gozer;password=**zapped**;`
//     Schema:                 `dbo`
//     Include Views:          `True`

//     Factory Name:          `SqlClientFactory`
// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591

using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace Zuul.Data
{
    public static partial class DataManagerFactory
    {
        public static U GetDataManager<T, U>(CompanyUser cu = null) where U : IDataManager<T>
        {
            return (U)GetDataManager<T>(cu);
        }

        public static IDataManager<T> GetDataManager<T>(CompanyUser cu = null)
        {
            IDataManager<T> dataManager;

            if (cu == null)
                cu = new CompanyUser();

            var origStatus = cu.ForceAuthDb;

            cu.ForceAuthDb = true;

            var type = (DataObjects)Enum.Parse(typeof(DataObjects), typeof(T).Name);
            switch (type)
            {
                case (DataObjects.Company):
                    {
                        var manager = new CompanyDataManager(cu);
                        dataManager = (IDataManager<T>)manager;
                        break;
                    }
                case (DataObjects.ContractSetting):
                    {
                        var manager = new ContractSettingDataManager(cu);
                        dataManager = (IDataManager<T>)manager;
                        break;
                    }
                case (DataObjects.User):
                    {
                        var manager = new UserDataManager(cu);
                        dataManager = (IDataManager<T>)manager;
                        break;
                    }
                case (DataObjects.UserSession):
                    {
                        var manager = new UserSessionDataManager(cu);
                        dataManager = (IDataManager<T>)manager;
                        break;
                    }
                case (DataObjects.Validation):
                    {
                        var manager = new ValidationDataManager(cu);
                        dataManager = (IDataManager<T>)manager;
                        break;
                    }
                default:
                    throw new Exception($"Invalid Data Type: {nameof(T)}");
            }

            cu.ForceAuthDb = origStatus;

            return dataManager;
        }
    }
}
#pragma warning restore 1591