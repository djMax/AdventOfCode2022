using System.Diagnostics;
using AdventOfCode2022.Day19;
using AdventOfCode2022.Search;

var sample = @"Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.";

var blueprints = Blueprint.ParseAll(AdventOfCode2022.Util.File.Read("AdventOfCode2022.Day19.data.txt"));
var start = Stopwatch.StartNew();
var part1 = blueprints.ToArray()
    .AsParallel()
    .Select(bp => bp.WithMinutes(24).Search<RobotState, Blueprint>(
        new RobotState(0, RobotState.getResources(0), RobotState.getResources(1))
    ))
    .Select((value, index) => value!.Resources[Resource.Geode] * (index + 1))
    .Sum();
Console.WriteLine("Part 1 {0} in {1}s", part1, start.Elapsed.TotalSeconds);
