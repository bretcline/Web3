using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using WebThree.Shared;
using WebThree.Shared.Data;
using static WebThree.Shared.Utilities;

namespace WebThree.Shared.Data
{
    public class PagedData<T>
    {
        public int TotalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<T> Data { get; set;  }
    }


    public class BaseDataManager<T> : RootDataManager, IDataManager<T> where T : class, IDataObject, new()
    {
        protected SqlSugarClient Context { get => base.context; }

        public BaseDataManager( RootDataManager db ) : base( db )
        {
        }

        public BaseDataManager( CompanyUser? cu ) : base( cu )
        {
        }

        public virtual ISugarQueryable<T> DefaultQuery()
        {
            return context.Queryable<T>();
        }
        public ISugarQueryable<T> DefaultQuery(Expression<Func<T, bool>> expression)
        {
            return context.Queryable<T>().Where(expression);
        }

        public int RecordCount()
        {
            return context.Queryable<T>().Count();
        }



        private void ValidateSession( CompanyUser cu )
        {
            if( Guid.Empty != cu.SessionId )
            {

            }
        }

        public virtual T First(Expression<Func<T, bool>> expression)
        {
            return DefaultQuery().First(expression);
        }

        public virtual List<T> GetAll()
        {
            var rc = DefaultQuery().ToList();
            return rc;
        }

        public PagedData<T> GetPaged(int pageIndex, int pageSize)
        {
            var pagedData = new PagedData<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRecords = DefaultQuery().Count(),
                Data = DefaultQuery().ToPageList(pageIndex, pageSize)
            };

            return pagedData;
        }

        public virtual T Get(Guid id)
        {
            return DefaultQuery().Single( i => i.ID == id );
        }

        private void PreAdd( T entity )
        {
            if (null != entity)
            {
                entity.ID = ( entity.ID == Guid.Empty ) ? Guid.NewGuid() : entity.ID;
                entity.CreatedBy = CompanyUser.AdminUserId;
                entity.CreatedDate = DateTime.UtcNow;
                entity.Deleted = false;
            }
            else
                throw new Exception("Null Object.");
        }

        public virtual T Add(T entity)
        {
            PreAdd( entity );
            entity = context.Insertable(entity).ExecuteReturnEntity();
            return entity;
        }

        public virtual List<T> BulkAdd( List<T> entities )
        {
            foreach( var entity in entities )
            {
                PreAdd(entity);
            }
            context.Fastest<T>().BulkCopy(entities);
            return entities;
        }

        public virtual T Update(T entity, T? item = null)
        {
            if (entity != null)
            {
                if( item == null)
                    item = Get(entity.ID);
                if( item != null )
                {
                    entity.CopyPropertiesTo(item);

                    item.UpdatedDate = DateTime.UtcNow;
                    item.UpdatedBy = CompanyUser.AdminUserId;
                    context.Updateable(item).ExecuteCommand();
                    return item;
                }
                else
                    throw new WebThreeException($"Object with ID {entity.ID} does not exist.");
            }
            else
                throw new Exception("Null Object.");
        }

        public virtual void Delete(Guid id)
        {
            var entity = Get(id);
            if( null != entity)
            {
                entity.Deleted = true;
                entity.UpdatedDate = DateTime.UtcNow;
                entity.UpdatedBy = CompanyUser.AdminUserId;
                context.Updateable(entity).ExecuteCommand();
            }
            else
                throw new Exception("Null Object.");
        }

        public virtual T Create()
        {
            return new T();
        }
    }
}
