using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sphere10.Framework;

namespace Sphere10.FrameworkTester.WinForms
{
	public class TestClassDataSource<TItem> : IDataSource<TestClass>
	{
		List<TestClass> AllItems = new List<TestClass>();

		public void RefreshData()
		{
			AllItems = LoadData(73);
		}

		// create some dummy data
		public TestClassDataSource()
		{
			RefreshData();
		}

		static List<TestClass> LoadData(int size)
		{
			var testData = new List<TestClass>();

			for (int i = 0; i < size; i++)
			{
				var testObject = new TestClass();
				testObject.FillWithTestData(i);
				testData.Add(testObject);
			}

			return testData;
		}

		public async Task Create(IEnumerable<TestClass> entities)
		{
			AllItems.AddRange(entities);
		}

		public async Task Delete(IEnumerable<TestClass> entities)
		{
			foreach (var entity in entities)
			{
				var index = AllItems.IndexOf(entity);
				if (index >= 0) AllItems.RemoveAt(index);
			}
		}

		public IEnumerable<TestClass> New(int count)
		{
			var returnList = new List<TestClass>();
			var newId = AllItems.Max(x => x.Id) + 1;

			for (int i = 0; i < count; i++)
			{
				var newEntity = new TestClass();
				newEntity.FillWithTestData(newId++);
				AllItems.Add(newEntity);
				returnList.Add(newEntity);
			}

			return returnList;
		}

		public Task<DataSourceItems<TestClass>> Read(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection)
		{
			// make sure the requested page is logical
			if (page < 0) page = 0;
			else if (page > AllItems.Count / pageLength) page = AllItems.Count / pageLength;

			var startIndex = pageLength * page;

			// the last page might not have a full [page of data
			if (startIndex + pageLength >= AllItems.Count) pageLength = AllItems.Count - startIndex;

			var items = (IEnumerable<TestClass>)AllItems.GetRange(startIndex, pageLength);

			var result = new DataSourceItems<TestClass>() {
				Items = items,
				Page = page,
				TotalCount = AllItems.Count()
			};
			
			return Task.FromResult(result);

			//return Task.Run(() => (IEnumerable<TestClass>)AllItems.GetRange(startIndex, pageLength));
		}

		public Task Refresh(TestClass[] entity)
		{
			throw new NotImplementedException();
		}

		public Task Update(IEnumerable<TestClass> entities)
		{
			var test = entities.ToArray()[0].ToString();

			return Task.Run(() =>
			{
				foreach (var entity in entities)
				{
					for (int i = 0; i < AllItems.Count; i++)
					{
						if (AllItems[i].Id == entity.Id)
						{
							AllItems[i] = entity;
							break;
						}
					}
				}
			}
			);
		}

		public Task<Result> Validate(IEnumerable<(TestClass entity, CrudAction action)> actions)
		{
			throw new NotImplementedException();
		}

		public Task<int> Count { get { return Task.Run(() => AllItems.Count); } }

		public Task<DataSourceCapabilities> Capabilities => Task.FromResult(DataSourceCapabilities.Default);
	}

}
