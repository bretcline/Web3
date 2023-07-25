using System.Data;

namespace WebThree.Shared.Data
{
    public static class BaseItemExtensions
    {
        public static void SimpleCopyPropertiesTo<T, TU>(this T source, TU dest)
        {
            var allSourceProps = typeof(T).GetProperties().ToList();
            var sourceProps = allSourceProps.Where(x => x.CanRead).ToList();
            var destProps = typeof(TU).GetProperties()
                .Where(x => x.CanWrite)
                .ToList();

            foreach (var sourceProp in sourceProps)
            {
                var value = sourceProp.GetValue(source, null);
                var p = destProps.FirstOrDefault(x => x.Name == sourceProp.Name);
                if (null != p)
                {
                    var destValue = p.GetValue(dest, null);
                    if (p.CanWrite)
                    {
                        if (null == value)
                            p.SetValue(dest, null, null);
                        else if (!value.Equals(destValue))
                            p.SetValue(dest, value, null);
                    }
                }
            }
        }


        public static void CopyPropertiesTo<T, TU>(this T source, TU dest, bool everything = false) where TU : IDataObject
        {
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            var destProps = typeof(TU).GetProperties()
                .Where(x => x.CanWrite)
                .ToList();

            foreach (var sourceProp in sourceProps)
            {
                if (everything ||
                    (destProps.Any(x => x.Name == sourceProp.Name)
                     && sourceProp.Name != "ID"
                     && sourceProp.Name != "CreatedBy"
                     && sourceProp.Name != "CreatedDate"
                     && sourceProp.Name != "UpdatedBy"
                     && sourceProp.Name != "UpdatedDate"
                     && sourceProp.Name != "Deleted"))
                {
                    var value = sourceProp.GetValue(source, null);
                    if (sourceProp.Name.EndsWith("ID"))
                    {
                        if (value is Guid id)
                        {
                            if (id == Guid.Empty)
                            {
                                continue;
                            }
                        }
                    }

                    var p = destProps.FirstOrDefault(x => x.Name == sourceProp.Name);
                    if (null != p)
                    {
                        var destValue = p.GetValue(dest, null);
                        if (p.CanWrite)
                        {
                            if (null == value)
                                p.SetValue(dest, null, null);
                            else if (!value.Equals(destValue))
                                p.SetValue(dest, value, null);
                        }
                    }
                }
            }
        }
    }
}
