using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace ProtonPack.Data
{
    public class IpfsItemDataManager : BaseDataManager<IpfsItem>
    {

        internal IpfsItemDataManager(CompanyUser cu) : base(cu)
        {
        }


        public override ISugarQueryable<IpfsItem> DefaultQuery()
        {
            var rc = base.DefaultQuery()
                .Mapper(r => r.Attributes, r => r.Attributes.First().IpfsItemID);
            return rc;
        }

        public IpfsItem GetByName( string name ) 
        {
            return DefaultQuery().First( i => i.ItemName == name && i.ContractTypeID == CompanyUser.ContractTypeId );
        }

        public IpfsItem? GetByItemIndex(string index)
        {
            return DefaultQuery().First(i => i.ItemIndex == index && i.ContractTypeID == CompanyUser.ContractTypeId);
        }

        public List<IpfsItem> GetByContractType(Guid contractTypeId)
        {
            return DefaultQuery().Where(i => i.ContractTypeID == CompanyUser.ContractTypeId).ToList();
        }

        public override IpfsItem Add(IpfsItem entity)
        {
            entity.ContractTypeID = CompanyUser.ContractTypeId;
            entity.ID = Guid.NewGuid();
            var rc = base.Add(entity);
            var attribMan = DataManagerFactory.GetDataManager<IpfsAttribute, IpfsAttributeDataManager>(CompanyUser);

            if( entity.Attributes != null )
            {
                var attributes = entity.Attributes.ToList();
                entity.Attributes.Clear();
                foreach (var attribute in attributes)
                {
                    attribute.ID = Guid.NewGuid();
                    attribute.IpfsItemID = rc.ID;
                    entity.Attributes.Add(attribMan.Add(attribute));
                }
            }
            return rc;
        }

        public override IpfsItem Update(IpfsItem entity, IpfsItem? item = null)
        {
            var attribMan = DataManagerFactory.GetDataManager<IpfsAttribute, IpfsAttributeDataManager>(CompanyUser);

            if ( null == item )
                item = this.Get( entity.ID );

            var originalAttribs = item?.Attributes.ToList();

            var rc = base.Update(entity, item);

            var added = new List<IpfsItem>();
            var deleted = new List<IpfsItem>();
            var updated = new List<IpfsItem>();

            if( null != originalAttribs && null != entity.Attributes )
            {
                // Delete and Update
                foreach (var attribute in originalAttribs)
                {
                    IpfsAttribute? current = null;
                    try
                    {
                        current = entity.Attributes.FirstOrDefault(a => a.AttributeType.Equals(attribute.AttributeType, StringComparison.InvariantCultureIgnoreCase));
                        if (null == current)
                        {
                            attribMan.Delete(attribute.ID);
                        }
                        else
                        {
                            if (attribute.AttributeValue.CompareTo(current.AttributeValue) != 0)
                            {
                                attribute.AttributeValue = current.AttributeValue;
                                attribMan.Update(attribute);
                            }
                        }
                    }
                    catch( Exception e )
                    {
                        Console.WriteLine($"Orig {entity.ID} - {attribute.AttributeType} : {attribute.AttributeValue}");
                        Console.WriteLine($"Curr {entity.ID} - {current?.AttributeType ?? "unassigned"} : {current?.AttributeValue ?? "unassigned"}");
                    }
                }
            }

            if( null != entity.Attributes)
            {
                foreach (var attribute in entity.Attributes)
                {
                    var current = originalAttribs.FirstOrDefault(a => a.AttributeType.Equals(attribute.AttributeType, StringComparison.InvariantCultureIgnoreCase));
                    if (null == current)
                    {
                        var attrib = new IpfsAttribute
                        {
                            AttributeType = attribute.AttributeType,
                            AttributeValue = attribute.AttributeValue,
                            IpfsItemID = rc.ID,
                        };
                        attribMan.Add(attrib);
                    }
                }
            }

            return rc;
        }
    }
}
