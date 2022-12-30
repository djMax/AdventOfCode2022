using System;
namespace AdventOfCode2022.Search
{
	public class World
	{
		public S? Search<S, W>(S initial)
			where S : SearchState<S, W>
			where W : World
		{
			var considered = 0;
			var q = new PriorityQueue<S, S>();
			S? best = default(S);
			var visited = new HashSet<string>();

			q.Enqueue(initial, initial);

			while (q.Count > 0)
			{
				var state = q.Dequeue();
				considered += 1;
				if (state.IsComplete((W)this))
				{
					if (best == null || state.CompareTo(best) < 0)
					{
						best = state;
					}
					continue;
				}

				var key = state.Key;
				if (visited.Contains(key))
				{
					continue;
				}
				visited.Add(key);

				if (best != null && state.ShouldPrune((W) this, best))
				{
					continue;
				}

				var next = state.Next((W) this, visited);
				foreach (var n in next)
				{
					if (!visited.Contains(n.Key))
					{
						q.Enqueue(n, n);
					}
				}
			}

			Console.WriteLine("Considered {0}, Best {1}", considered, best?.ToString((W) this));
			return best;
		}
	}
}

