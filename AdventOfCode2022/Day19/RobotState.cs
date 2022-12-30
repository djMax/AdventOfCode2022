using System;
using System.Collections.Generic;
using AdventOfCode2022.Search;

namespace AdventOfCode2022.Day19
{
    public class RobotState : SearchState<RobotState, Blueprint>
    {
        public string ToString(Blueprint world)
        {
            return String.Format("Blueprint {0}: {1}", world.Index, this.Resources[Resource.Geode]);
        }

        public static Dictionary<Resource, int> getResources(int ore = 0)
        {
            return new Dictionary<Resource, int>
            {
                { Resource.Ore, ore },
                { Resource.Clay, 0 },
                { Resource.Obsidian, 0 },
                { Resource.Geode, 0 }
            };
        }

        public int Minute { get; }

        public Dictionary<Resource, int> Resources { get; }

        public Dictionary<Resource, int> Robots { get; }

        public RobotState(int minute, Dictionary<Resource, int> resources, Dictionary<Resource, int> robots)
        {
            this.Minute = minute;
            this.Resources = resources;
            this.Robots = robots;
        }

        public int MaxScore(Blueprint blueprint)
        {
            if (this.Minute == blueprint.Minutes)
            {
                return this.Resources[Resource.Geode];
            }
            var remaining = blueprint.Minutes - this.Minute;
            return this.Resources[Resource.Geode] + ((((remaining - 1)) * remaining) / 2) + 1;
        }

        public string Key => String.Format(
            "{0},{1},{2},{3},{4},{5},{6},{7}",
            this.Minute,
            this.Resources[Resource.Ore], this.Resources[Resource.Clay],
            this.Resources[Resource.Obsidian], this.Resources[Resource.Geode],
            this.Robots[Resource.Ore], this.Robots[Resource.Clay],
            this.Robots[Resource.Obsidian]
        );

        public bool IsComplete(Blueprint blueprint)
        {
            return this.Minute == blueprint.Minutes;
        }

        public bool ShouldPrune(Blueprint world, RobotState best)
        {
            return best.Resources[Resource.Geode] > this.MaxScore(world);
        }

        public int CompareTo(RobotState? other)
        {
            if (other == null)
            {
                return -1;
            }
            int cmp = other.Resources[Resource.Geode] - this.Resources[Resource.Geode];
            if (cmp != 0)
            {
                return cmp;
            }
            cmp = other.Minute - this.Minute;
            if (cmp != 0)
            {
                return cmp;
            }
            cmp = other.Robots[Resource.Obsidian] - this.Robots[Resource.Obsidian];
            if (cmp != 0)
            {
                return cmp;
            }
            cmp = other.Robots[Resource.Clay] - this.Robots[Resource.Clay];
            if (cmp != 0)
            {
                return cmp;
            }
            cmp = other.Resources[Resource.Obsidian] - this.Resources[Resource.Obsidian];
            if (cmp != 0)
            {
                return cmp;
            }
            cmp = other.Resources[Resource.Clay] - this.Resources[Resource.Clay];
            if (cmp != 0)
            {
                return cmp;
            }
            return cmp;
        }

        RobotState GoForward()
        {
            var resources = this.Resources.ToDictionary(orig => orig.Key, orig => orig.Value + this.Robots[orig.Key]);
            return new RobotState(this.Minute + 1, resources, this.Robots);
        }

        RobotState AddRobot(Blueprint blueprint, Resource r)
        {
            var costs = blueprint.Costs[r];
            var consumed = this.Resources.ToDictionary(orig => orig.Key, orig => orig.Value - costs[orig.Key]);
            if (consumed.Any(pair => pair.Value < 0))
            {
                throw new Exception("Could not afford robot");
            }
            if (r == Resource.Geode)
            {
                var withGeode = consumed.ToDictionary(orig => orig.Key, orig =>
                {
                    if (orig.Key == Resource.Geode)
                    {
                        return consumed[Resource.Geode] + (blueprint.Minutes - this.Minute - 1);
                    }
                    return orig.Value + this.Robots[orig.Key];
                });
                return new RobotState(this.Minute + 1, withGeode, this.Robots);
            }
            else
            {
                var resources = consumed.ToDictionary(orig => orig.Key, orig => orig.Value + this.Robots[orig.Key]);
                var addedRobot = this.Robots.ToDictionary(orig => orig.Key, orig =>
                {
                    if (orig.Key == r)
                    {
                        return this.Robots[r] + 1;
                    }
                    return orig.Value;
                });
                return new RobotState(this.Minute + 1, resources, addedRobot);
            }
        }

        bool CanAfford(Blueprint blueprint, Resource r)
        {
            var cost = blueprint.Costs[r];
            return this.Resources.All(pair => pair.Value >= cost[pair.Key]);
        }

        bool Needs(Blueprint blueprint, Resource r)
        {
            if (this.Minute == blueprint.Minutes)
            {
                return false;
            }
            return r == Resource.Geode || this.Robots[r] < blueprint.MaxRobots[r];
        }

        RobotState[] SearchState<RobotState, Blueprint>.Next(Blueprint world, HashSet<string> visitedKeys)
        {
            var remaining = world.Minutes - this.Minute;
            if (remaining == 0)
            {
                return new RobotState[] { };
            }

            var nextStates = new List<RobotState>();
            nextStates.Add(this.GoForward());

            var resources = Enum.GetValues(typeof(Resource));
            foreach (Resource r in resources)
            {
                if (this.CanAfford(world, r) && this.Needs(world, r))
                {
                    nextStates.Add(this.AddRobot(world, r));
                }
            }

            return nextStates.ToArray();
        }
    }
}

