package main

import (
	"database/sql"
	"fmt"
	"log"
	"time"

	_ "github.com/denisenkom/go-mssqldb"
)

var connectionString = "server=.;database=RouteBenchmark;port=1433; Trusted_Connection=True;"
var db *sql.DB
var cities []City
var routes []Route

func main() {

	db, _ = sql.Open("sqlserver", connectionString)
	err := db.Ping()
	if err != nil {
		log.Fatal(err)
	}

	// load data from database
	cities = getCities()
	routes = getRoutes()

	//start
	startTime := time.Now()

	result := calculate()
	report(result)

	//end

	fmt.Println("Duration : ", time.Since(startTime).Minutes())
	var input string
	fmt.Scanln(&input)
}

func calculate() []Result {
	result := []Result{}

	for i := 0; i < len(cities); i++ {
		for j := 0; j < len(cities); j++ {
			if cities[i] == cities[j] {
				continue
			}

			exist := contains(cities[i].id, cities[j].id)

			if !exist {
				result = append(result, Result{cities[i].title, cities[j].title})
			}
		}
		fmt.Println("City : ", i+1)
	}
	return result
}

func contains(sourceCityId int, destinationCityId int) bool {
	for i := 0; i < len(routes); i++ {
		if routes[i].sourceCityId == sourceCityId && routes[i].destinationCityId == destinationCityId {
			return true
		}
	}
	return false
}

func report(result []Result) {
	str := ""
	for i := 0; i < len(result); i++ {
		str += fmt.Sprintf("%v -> %v \n", result[i].sourceCityTitle, result[i].destinationCityTitle)
	}
	fmt.Println(str)
	fmt.Println("Result Count : ", len(result))
}

func getCities() []City {

	err := db.Ping()
	if err != nil {
		fmt.Print(err)
	}

	tsql := "select Id,Title from Cities"

	rows, err := db.Query(tsql)
	if err != nil {
		fmt.Print(err)
	}

	defer rows.Close()

	model := []City{}

	for rows.Next() {
		var title string
		var id int

		err := rows.Scan(&id, &title)
		if err != nil {
			fmt.Print(err)
		}

		model = append(model, City{id, title})
	}

	return model
}

func getRoutes() []Route {

	err := db.Ping()
	if err != nil {
		fmt.Print(err)
	}

	tsql := "select sourceCityId,destinationCityId from Routes"

	rows, err := db.Query(tsql)
	if err != nil {
		fmt.Print(err)
	}

	defer rows.Close()

	model := []Route{}

	for rows.Next() {
		var sourceCityId int
		var destinationCityId int

		err := rows.Scan(&sourceCityId, &destinationCityId)
		if err != nil {
			fmt.Print(err)
		}

		model = append(model, Route{sourceCityId, destinationCityId})
	}

	return model
}

type City struct {
	id    int
	title string
}

type Route struct {
	sourceCityId      int
	destinationCityId int
}

type Result struct {
	sourceCityTitle      string
	destinationCityTitle string
}
