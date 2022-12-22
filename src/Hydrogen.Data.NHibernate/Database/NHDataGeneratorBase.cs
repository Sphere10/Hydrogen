using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using System.Threading.Tasks;
using Hydrogen;

namespace Hydrogen.Data.NHibernate {
    public abstract class NHDataGeneratorBase : IDataGenerator {
	    protected NHDataGeneratorBase(ISessionFactory sessionFactory)  {
			SessionFactory = sessionFactory;
	    }

		protected ISessionFactory SessionFactory { get; set; }

        public void Populate() {
            using (var session = SessionFactory.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    var data = CreateData();
                    data.ForEach(session.SaveOrUpdate);
                    transaction.Commit();
                }
                session.Flush();
                session.Close();
            } 
        }

        protected abstract IEnumerable<object> CreateData();

		protected IEnumerable<TypeTable<TEnum>> CreateTypeTable<TEnum>()
			where TEnum : Enum {

			var typeofEnum = typeof(TEnum);
			foreach (Enum @enum in Enum.GetValues(typeofEnum)) {
				var entity = new TypeTable<TEnum> {
					ID = (int) Enum.Parse(typeof(TEnum), @enum.ToString()),
					Name = Enum.GetName(typeofEnum, @enum),
					Description =  Tools.Enums.GetDescription(@enum)
				};

				yield return entity;
			}
		}

		protected IEnumerable<object> CreateTypeTable(Type enumType) {
			Guard.ArgumentNotNull(enumType, nameof(enumType));
			Guard.Argument(enumType.IsEnum, nameof(enumType), "Not an enum type");

			foreach (Enum @enum in Enum.GetValues(enumType)) {
				var typeTableType = typeof(TypeTable<>).MakeGenericType(enumType);
				var entity = Activator.CreateInstance(typeTableType) as TypeTable;
				entity.ID = (int) Enum.Parse(enumType, @enum.ToString());
				entity.Name = Enum.GetName(enumType, @enum);
				entity.Description =  Tools.Enums.GetDescription(@enum);
				yield return entity;
			}
		}
    }
}

