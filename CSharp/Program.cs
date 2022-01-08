using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

class Program
{
    public static List<City> _cities;
    public static List<Result> _result;
    public static ImmutableSortedSet<Routes> _routes;

    static void Main(string[] args)
    {
        // load data
        var cities = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "../../../../db/cities.json"));
        _cities = JsonConvert.DeserializeObject<List<City>>(cities);

        var routes = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "../../../../db/routes.json"));
        _routes = JsonConvert.DeserializeObject<ImmutableSortedSet<Routes>>(routes);

        _result = new List<Result>();

        // start
        var watch = System.Diagnostics.Stopwatch.StartNew();

        Calculate();

        watch.Stop();

        Console.WriteLine($"Duration(Milliseconds): {watch.Elapsed.TotalMilliseconds}");
        Console.WriteLine($"Result Count: {_result.Count()}");


        Console.ReadKey();
    }

    public static void Calculate()
    {
        int cityCount = _cities.Count;

        for (int i = 0; i < cityCount; i++)
        {
            for (int j = 0; j < cityCount; j++)
            {
                var x = _cities[i].Id;
                var y = _cities[j].Id;
                if (x == y)
                    continue;

                bool exist = _routes.Contains(new Routes { SourceCityId = x, DestinationCityId = y });

                if (!exist)
                {
                    _result.Add(new Result()
                    {
                        SourceCityId = _cities[i].Id,
                        DestinationCityId = _cities[j].Id
                    });
                }
            }
        }
    }


    public struct City
    {
        public int Id { get; set; }
        public String Title { get; set; }
    }

    public struct Routes : IComparable<Routes>
    {
        public int SourceCityId { get; set; }
        public int DestinationCityId { get; set; }

        public int CompareTo(Routes other)
        {
            int result = SourceCityId.CompareTo(other.SourceCityId);
            if (result == 0)
                result = DestinationCityId.CompareTo(other.DestinationCityId);
            return result;
        }
    }

    public struct Result
    {
        public int SourceCityId { get; set; }
        public int DestinationCityId { get; set; }
    }
}