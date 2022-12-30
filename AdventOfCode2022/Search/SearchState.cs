using System;
using System.Collections.Generic;
	
namespace AdventOfCode2022.Search
{
	public interface SearchState<S, W>: IComparable<S>
		where W: World where S : SearchState<S, W>
	{
		string Key { get; }
		bool IsComplete(W world);
		bool ShouldPrune(W world, S best);
		S[] Next(W world, HashSet<string> visitedKeys);

		string ToString(W world);
	}
}

