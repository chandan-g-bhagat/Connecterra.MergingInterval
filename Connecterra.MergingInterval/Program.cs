using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Connecterra.MergingInterval
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            #region Input One: Comment region Two and Uncomment this to run

            var intervals = new List<Interval>
            {
                new Interval
                {
                    start = 1,
                    end = 20,
                    Actions=Ops.Added
                },
                new Interval
                {
                    start = 55,
                    end = 58,
                    Actions=Ops.Added
                },
                new Interval
                {
                    start = 60,
                    end = 89,
                    Actions=Ops.Added
                },
                new Interval
                {
                    start = 15,
                    end = 31,
                    Actions=Ops.Added
                },
                new Interval
                {
                    start = 10,
                    end = 15,
                    Actions=Ops.Added
                },
                new Interval
                {
                    start = 1,
                    end = 20,
                    Actions = Ops.Removed
                },
                new Interval
                {
                    start = 33,
                    end = 36,
                    Actions = Ops.Added
                },
                new Interval
                {
                    start = 11,
                    end = 16,
                    Actions = Ops.Deleted
                }
            };

            #endregion Input One: Comment region Two and Uncomment this to run

            #region Input Two: Comment region One and Uncomment this to run

            //var intervals = new List<Interval>
            //{
            //    new Interval
            //    {
            //        start = 1,
            //        end = 6,
            //        Actions=Ops.Added
            //    },
            //    new Interval
            //    {
            //        start = 5,
            //        end = 7,
            //        Actions=Ops.Added
            //    },
            //    new Interval
            //    {
            //        start = 2,
            //        end = 3,
            //        Actions=Ops.Deleted
            //    },
            //};

            #endregion Input Two: Comment region One and Uncomment this to run

            var mergedIntervals = Merge(intervals, 7);

            foreach (var item in mergedIntervals.OrderBy(p => p.start))
            {
                Console.WriteLine($"[{item.start}, {item.end}]");
            }

            Console.ReadKey();
        }

        public static List<Interval> Merge(List<Interval> intervals, int mergeDistance)
        {
            var result = new List<IntervalGroup>();
            var group = new IntervalGroup();
            foreach (var item in intervals.OrderBy(p => p.Actions))
            {
                group = result.Where(c => c.Groups.Any(g => Math.Abs(g.end - item.start) <= mergeDistance || Math.Abs(g.end - item.end) <= mergeDistance)).FirstOrDefault();
                switch (item.Actions)
                {
                    case Ops.Added:
                        if (group != null)
                        {
                            group.Groups.Add(item);
                        }
                        else
                        {
                            group = new IntervalGroup();
                            group.Groups = new List<Interval>();
                            result.Add(group);
                            group.Groups.Add(item);
                        }
                        break;

                    case Ops.Removed:
                        group.Groups.Remove(group.Groups.Where(c => c.start == item.start && c.end == item.end).First());
                        break;

                    case Ops.Deleted:
                        var selectedResult = result.FirstOrDefault(p => p.Groups.Any(g => g.start < item.start));
                        if (selectedResult != null)
                        {
                            var splitIntervalOne = new IntervalGroup();
                            var splitIntervaltwo = new IntervalGroup();

                            //split first interval
                            foreach (var itemOne in selectedResult.Groups.Where(p => p.start <= item.start).OrderBy(p => p.start))
                            {
                                var innerInterval = new Interval();
                                innerInterval.start = itemOne.start;
                                if (itemOne.end < item.end)
                                {
                                    selectedResult.Groups.Remove(itemOne);
                                }
                                if (itemOne.end > item.start)
                                {
                                    innerInterval.end = item.start;
                                    splitIntervalOne.Groups.Add(innerInterval);
                                    break;
                                }
                                innerInterval.end = itemOne.end;
                                splitIntervalOne.Groups.Add(innerInterval);
                            }
                            //split second interval
                            foreach (var itemOne in selectedResult.Groups)
                            {
                                var innerInterval = new Interval();
                                if (itemOne.start < item.end)
                                {
                                    innerInterval.start = item.end;
                                    innerInterval.end = itemOne.end;
                                }
                                else
                                {
                                    innerInterval.start = itemOne.start;
                                    innerInterval.end = itemOne.end;
                                }
                                splitIntervaltwo.Groups.Add(innerInterval);
                            }
                            result.Remove(selectedResult);
                            result.Add(splitIntervalOne);
                            result.Add(splitIntervaltwo);
                        }
                        break;

                    default:
                        break;
                }
            }

            var finalResult = result.Select(s => new Interval { start = s.Groups.Min(min => min.start), end = s.Groups.Max(min => min.end) });
            return finalResult.ToList();
        }
    }

    public class Interval
    {
        public int start { get; set; }
        public int end { get; set; }
        public Ops Actions { get; set; }
    }

    public enum Ops
    {
        Added = 1,
        Removed = 2,
        Deleted = 3
    }

    public class IntervalGroup
    {
        public List<Interval> Groups { get; set; } = new List<Interval>();
    }
}