package main

import (
	"database/sql"
	"fmt"
	"log"
	"time"

	_ "github.com/denisenkom/go-mssqldb"
)

var connectionString = "server=.;database=RouteBenchmark;port=1433;"
var db *sql.DB
var cities = make(map[string]City)
var routes = make(map[string]Route)
var result = make(map[string]Result)

func main() {

	db, _ = sql.Open("sqlserver", connectionString)
	err := db.Ping()
	if err != nil {
		log.Fatal(err)
	}

	// load data from database
	getCities()
	getRoutes()

	//start
	startTime := time.Now()

	calculate()
	//end

	fmt.Println("Duration(Milliseconds): ", time.Since(startTime).Milliseconds())
	fmt.Println("Result Count: ", len(result))

	var input string
	fmt.Scanln(&input)
}

func calculate() {

	for _, c1 := range cities {
		for _, c2 := range cities {

			x := c1.id
			y := c2.id

			if x == y {
				continue
			}

			_, exist := routes[fmt.Sprintf("%v-%v", x, y)]

			if !exist {
				result[fmt.Sprintf("%v-%v", x, y)] = Result{c1.id, c2.id}
			}
		}
	}
}

func getCities() {
	tsql := "select Id from Cities"

	rows, err := db.Query(tsql)
	if err != nil {
		fmt.Print(err)
	}

	defer rows.Close()

	for rows.Next() {
		var id int

		err := rows.Scan(&id)
		if err != nil {
			fmt.Print(err)
		}

		cities[fmt.Sprintf("_%v", id)] = City{id}
	}
}

func getRoutes() {

	tsql := "select sourceCityId,destinationCityId from Routes"

	rows, err := db.Query(tsql)
	if err != nil {
		fmt.Print(err)
	}

	defer rows.Close()

	for rows.Next() {
		var sourceCityId int
		var destinationCityId int

		err := rows.Scan(&sourceCityId, &destinationCityId)
		if err != nil {
			fmt.Print(err)
		}

		routes[fmt.Sprintf("%v-%v", sourceCityId, destinationCityId)] = Route{sourceCityId, destinationCityId}
	}
}

type City struct {
	id int
}

type Route struct {
	sourceCityId      int
	destinationCityId int
}

type Result struct {
	sourceCityId      int
	destinationCityId int
}
