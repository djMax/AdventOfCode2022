using System;
using System.Text.RegularExpressions;
using AdventOfCode2022.Search;

namespace AdventOfCode2022.Day19
{
	public class Blueprint: World
	{
		static Regex COSTS = new Regex(@"Each (.*) robot costs (.*)");

		public int Index { get; }
		public int Minutes { get; }

		public Dictionary<Resource, Dictionary<Resource, int>> Costs;

		public Dictionary<Resource, int> MaxRobots;

		public Blueprint(int index, int minutes, Dictionary<Resource, Dictionary<Resource, int>> costs, Dictionary<Resource, int> max)
		{
			this.Index = index;
			this.Minutes = minutes;
			this.Costs = costs;
			this.MaxRobots = max;
		}

		public Blueprint WithMinutes(int m)
		{
			return new Blueprint(this.Index, m, this.Costs, this.MaxRobots);
		}

		public static Blueprint Parse(string input)
		{
			var rMap = new Dictionary<string, Resource> {
				{ "ore", Resource.Ore },
				{ "clay", Resource.Clay },
				{ "obsidian", Resource.Obsidian },
				{ "geode", Resource.Geode }
			};
			var s1 = input.Split(":");
			var index = int.Parse(s1[0].Split(" ")[1]);

			var costs = new Dictionary<Resource, Dictionary<Resource, int>>();
			var max = RobotState.getResources(0);

			s1[1].Trim().Split(".").ToList().ForEach(costExp =>
			{
				if (String.IsNullOrWhiteSpace(costExp))
				{
					return;
				}
				var spec = COSTS.Match(costExp.Trim());
				var type = rMap[spec.Groups[1].Captures[0].Value];
				var cost = RobotState.getResources(0);
				costs[type] = cost;
				spec.Groups[2].Captures[0].Value.Split(" and ").ToList().ForEach(c =>
				{
					var parts = c.Split(" ");
					var resource = rMap[parts[1]];
					var count = int.Parse(parts[0]);
					max[resource] = max.ContainsKey(resource) ? Math.Max(count, max[resource]) : count;
					cost[resource] = count;
				});
			});
			return new Blueprint(index, 0, costs, max);
        }

		public static Blueprint[] ParseAll(string input)
		{
			return input.Split("\n").ToList().Select(str => Parse(str)).ToArray();
		}
    }
}

