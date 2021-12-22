using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;

class Program
{
    private static string connectionString = "Data Source =.; Initial Catalog = RouteBenchmark; Trusted_Connection=True;";
    private static List<Routes> _routes;
    private static List<City> _cities;

    static void Main(string[] args)
    {
        // load data from database
        _cities = GetCities();
        _routes = GetRoutes();

        // start 
        var watch = System.Diagnostics.Stopwatch.StartNew();

        var result = Calculate(); 

        Report(result); 

        watch.Stop();
        // end

        // print duration
        var duration = watch.Elapsed.TotalMinutes;
        Console.WriteLine($"Duration : {duration}");

        Console.ReadKey();
    }

    static List<Result> Calculate()
    {
        var result = new List<Result>();

        int cityCount = _cities.Count;

        for (int i = 0; i < cityCount; i++)
        {
            for (int j = 0; j < cityCount; j++)
            {
                var x = _cities[i].Id;
                var y = _cities[j].Id;

                if (x == y)
                    continue;

                bool exist = Contains(ref x, ref y);

                if (!exist)
                {
                    result.Add(new Result()
                    {
                        SourceCityId = x,
                        DestinationCityId = y
                    });
                }
            }

            Console.WriteLine($"City : {i + 1}");
        }

        return result;
    }

    static bool Contains(ref int sourceCityId, ref int destinationCityId)
    {
        int count = _routes.Count;
        for (int i = 0; i < count; i++)
        {
            var x = _routes[i].SourceCityId;
            var y = _routes[i].DestinationCityId;

            if (x == sourceCityId && y == destinationCityId)
            {
                return true;
            }
        }
        return false;
    }

    static void Report(List<Result> results)
    {
        var stringbuilder = new StringBuilder();

        var count = results.Count;
        for (int i = 0; i < count; i++)
        {
            stringbuilder.Append(results[i].SourceCityId).Append(" -> ").Append(results[i].DestinationCityId).Append('\n');
        }
        Console.WriteLine(stringbuilder.ToString());
        Console.WriteLine($"Result Count : {results.Count}");
    }



    static List<City> GetCities()
    {
        using IDbConnection db = new SqlConnection(connectionString);
        return db.Query<City>("select * from [dbo].Cities", commandType: CommandType.Text).ToList();
    }

    static List<Routes> GetRoutes()
    {
        using IDbConnection db = new SqlConnection(connectionString);
        return db.Query<Routes>("select * from [dbo].Routes", commandType: CommandType.Text).ToList();
    }

    public struct City
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public struct Routes
    {
        public int SourceCityId { get; set; }
        public int DestinationCityId { get; set; }
    }

    public struct Result
    {
        public int SourceCityId { get; set; }
        public int DestinationCityId { get; set; }
    }

}