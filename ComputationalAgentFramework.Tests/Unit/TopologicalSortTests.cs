using ComputationalAgentFramework.Utils;

namespace ComputationalAgentFramework.Tests.Unit
{
    public class TopologicalSortTests
    {
        [Fact]
        public void TopologicalSort_LinearDependency_ShouldReturnCorrectOrder()
        {
            var itemA = new TopologicalSort.Item("A");
            var itemB = new TopologicalSort.Item("B", itemA);
            var itemC = new TopologicalSort.Item("C", itemB);
            
            var items = new[] { itemC, itemB, itemA };
            var sorted = TopologicalSort.Sort(items, x => x.Dependencies, new TopologicalSort.ItemEqualityComparer());
            
            var names = sorted.Select(x => x.Name).ToList();
            Assert.Equal(new[] { "A", "B", "C" }, names);
        }

        [Fact]
        public void TopologicalSort_NoDependencies_ShouldReturnSameOrder()
        {
            var itemA = new TopologicalSort.Item("A");
            var itemB = new TopologicalSort.Item("B");
            var itemC = new TopologicalSort.Item("C");
            
            var items = new[] { itemA, itemB, itemC };
            var sorted = TopologicalSort.Sort(items, x => x.Dependencies, new TopologicalSort.ItemEqualityComparer());
            
            Assert.Equal(3, sorted.Count);
            Assert.Contains(sorted, x => x.Name == "A");
            Assert.Contains(sorted, x => x.Name == "B");
            Assert.Contains(sorted, x => x.Name == "C");
        }

        [Fact]
        public void TopologicalSort_DiamondDependency_ShouldRespectAllDependencies()
        {
            var itemA = new TopologicalSort.Item("A");
            var itemB = new TopologicalSort.Item("B", itemA);
            var itemC = new TopologicalSort.Item("C", itemA);
            var itemD = new TopologicalSort.Item("D", itemB, itemC);
            
            var items = new[] { itemD, itemC, itemB, itemA };
            var sorted = TopologicalSort.Sort(items, x => x.Dependencies, new TopologicalSort.ItemEqualityComparer());
            
            var names = sorted.Select(x => x.Name).ToList();
            
            // A should come first
            Assert.Equal("A", names[0]);
            // D should come last
            Assert.Equal("D", names[3]);
            // B and C should be in the middle (order doesn't matter)
            Assert.Contains("B", names);
            Assert.Contains("C", names);
        }

        [Fact]
        public void TopologicalSort_CyclicDependency_ShouldThrow()
        {
            var itemA = new TopologicalSort.Item("A");
            var itemB = new TopologicalSort.Item("B");
            
            itemA.Dependencies = new[] { itemB };
            itemB.Dependencies = new[] { itemA };
            
            var items = new[] { itemA, itemB };
            
            Assert.Throws<ArgumentException>(() => 
                TopologicalSort.Sort(items, x => x.Dependencies, new TopologicalSort.ItemEqualityComparer()));
        }

        [Fact]
        public void TopologicalSort_EmptyList_ShouldReturnEmpty()
        {
            var items = Array.Empty<TopologicalSort.Item>();
            
            var sorted = TopologicalSort.Sort(items, x => x.Dependencies, new TopologicalSort.ItemEqualityComparer());
            
            Assert.Empty(sorted);
        }

        [Fact]
        public void TopologicalSort_SingleItem_ShouldReturnSingleItem()
        {
            var item = new TopologicalSort.Item("A");
            var items = new[] { item };
            
            var sorted = TopologicalSort.Sort(items, x => x.Dependencies, new TopologicalSort.ItemEqualityComparer());
            
            Assert.Single(sorted);
            Assert.Equal("A", sorted[0].Name);
        }

        [Fact]
        public void TopologicalSort_FanOutDependency_ShouldPlaceRootFirst()
        {
            var itemA = new TopologicalSort.Item("A");
            var itemB = new TopologicalSort.Item("B", itemA);
            var itemC = new TopologicalSort.Item("C", itemA);
            var itemD = new TopologicalSort.Item("D", itemA);
            
            var items = new[] { itemD, itemC, itemB, itemA };
            var sorted = TopologicalSort.Sort(items, x => x.Dependencies, new TopologicalSort.ItemEqualityComparer());
            
            var names = sorted.Select(x => x.Name).ToList();
            
            // A should be first
            Assert.Equal("A", names[0]);
            // B, C, D should follow in any order
            Assert.Contains("B", names.Skip(1));
            Assert.Contains("C", names.Skip(1));
            Assert.Contains("D", names.Skip(1));
        }

        [Fact]
        public void ItemEqualityComparer_ShouldComparByName()
        {
            var comparer = new TopologicalSort.ItemEqualityComparer();
            var item1 = new TopologicalSort.Item("A");
            var item2 = new TopologicalSort.Item("A");
            var item3 = new TopologicalSort.Item("B");
            
            Assert.True(comparer.Equals(item1, item2));
            Assert.False(comparer.Equals(item1, item3));
        }

        [Fact]
        public void ItemEqualityComparer_GetHashCode_ShouldBeConsistent()
        {
            var comparer = new TopologicalSort.ItemEqualityComparer();
            var item1 = new TopologicalSort.Item("A");
            var item2 = new TopologicalSort.Item("A");
            
            Assert.Equal(comparer.GetHashCode(item1), comparer.GetHashCode(item2));
        }
    }
}
