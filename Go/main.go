package main

import (
	"encoding/json"
	"fmt"
	"io/ioutil"
	"time"
)

var cities = make(map[string]City)
var routes = make(map[string]Route)
var result = make(map[string]Result)

func main() {

	// load data from database
	cities = getCities()
	routes = getRoutes()

	//start
	startTime := time.Now()

	calculate()

	fmt.Println("Duration(Milliseconds): ", time.Since(startTime).Milliseconds())
	fmt.Println("Result Count: ", len(result))

	var input string
	fmt.Scanln(&input)
}

func calculate() {

	for _, c1 := range cities {
		for _, c2 := range cities {

			x := c1.Id
			y := c2.Id

			if x == y {
				continue
			}

			_, exist := routes[fmt.Sprintf("%v-%v", x, y)]

			if !exist {
				result[fmt.Sprintf("%v-%v", x, y)] = Result{c1.Id, c2.Id}
			}
		}
	}
}

func getCities() map[string]City {
	jsonFile, err := ioutil.ReadFile("../db/cities.json")

	if err != nil {
		fmt.Println(err)
	}

	var arr []City

	err = json.Unmarshal(jsonFile, &arr)

	if err != nil {
		fmt.Println(err)
	}

	model := make(map[string]City)

	for _, item := range arr {
		model[fmt.Sprintf("_%v", item.Id)] = City{item.Id}
	}

	return model
}

func getRoutes() map[string]Route {
	jsonFile, err := ioutil.ReadFile("../db/routes.json")

	if err != nil {
		fmt.Println(err)
	}

	var arr []Route

	err = json.Unmarshal(jsonFile, &arr)

	if err != nil {
		fmt.Println(err)
	}

	model := make(map[string]Route)

	for _, item := range arr {
		model[fmt.Sprintf("%v-%v", item.SourceCityId, item.DestinationCityId)] = Route{item.SourceCityId, item.DestinationCityId}
	}

	return model
}

type City struct {
	Id int
}

type Route struct {
	SourceCityId      int
	DestinationCityId int
}

type Result struct {
	sourceCityId      int
	destinationCityId int
}
