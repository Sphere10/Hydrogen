using System.Linq;
using NUnit.Framework;
using Sphere10.Framework.Collections;

namespace Sphere10.Framework.Tests
{

    public class PreallocatedListTests
    {
        [Test]
        public void AddRange()
        {
            int[] start = Enumerable.Repeat(0, 10).ToArray();
            
            ExtendedList<int> list = new ExtendedList<int>(start);
            PreallocatedList<int> preallocatedList = new PreallocatedList<int>(list);
            Assert.IsTrue(preallocatedList.All(x => x == default));

            preallocatedList.AddRange(Enumerable.Range(0, 5));
            preallocatedList.AddRange(Enumerable.Range(5, 5));

            Assert.AreEqual(Enumerable.Range(0, 10).ToList(), preallocatedList);
        }
        
        [Test]
        public void InsertAtIndex()
        {
            int[] input = Enumerable.Repeat(0, 10).ToArray();
            
            ExtendedList<int> list = new ExtendedList<int>(input);
            PreallocatedList<int> preallocatedList = new PreallocatedList<int>(list);
            
            preallocatedList.InsertRange(0, input.Reverse());
            
            Assert.AreEqual(input.Reverse(), list);
        }
        
        [Test]
        public void RemoveAtIndex()
        {
            int[] expected = Enumerable.Repeat(0, 10).ToArray();
            
            ExtendedList<int> list = new ExtendedList<int>(expected);
            PreallocatedList<int> preallocatedList = new PreallocatedList<int>(list);
            
            preallocatedList.RemoveRange(0, preallocatedList.Count);
            
            Assert.IsTrue(preallocatedList.All(x => x == default));
        }
    }

}