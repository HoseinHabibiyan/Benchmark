using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;

class Program
{
    private static string connectionString = "Data Source =.; Initial Catalog = RouteBenchmark; Trusted_Connection=True;";
    public static List<City> _cities;
    public static List<Result> _result;
    public static ImmutableSortedSet<Routes> _routes;

    static void Main(string[] args)
    {
        // load data from database
        _cities = GetCities();
        _routes = GetRoutes();
        _result = new List<Result>();

        // start
        var watch = System.Diagnostics.Stopwatch.StartNew();

        Calculate();
        //end

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

    public static List<City> GetCities()
    {
        using IDbConnection db = new SqlConnection(connectionString);
        return db.Query<City>("select Id from [dbo].Cities", commandType: CommandType.Text).ToList();
    }

    public static ImmutableSortedSet<Routes> GetRoutes()
    {
        using IDbConnection db = new SqlConnection(connectionString);
        return db.Query<Routes>("select * from [dbo].Routes", commandType: CommandType.Text).ToImmutableSortedSet();
    }

    public struct City
    {
        public int Id { get; set; }
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