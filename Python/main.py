import pyodbc 
from datetime import datetime

conn = pyodbc.connect('Driver={SQL Server};'
                      'Server=localhost;'
                      'Database=RouteBenchmark;'
                      'Trusted_Connection=yes;')

cursor = conn.cursor()


cities = []
routes = []
results = []


def getCities():
  cursor.execute('SELECT * FROM Cities')

  for i in cursor:
    cities.append({ "id" :i[0] , "title" : i[1]})


  
def getRoutes():
  cursor.execute('SELECT * FROM Routes')

  for i in cursor:
    routes.append({ "sourceCityId" :i[0] , "destinationCityId" : i[1]})


def calculate():
  index = 0
  for source in cities:
   for destination in cities:
     if source['id'] == destination['id']:
        continue
     
     exist = contains(source['id'], destination['id'])
     if not exist:
       results.append({ "sourceCityTitle" : source['title'] , "destinationCityTitle" : destination['title']})

   print("City : " + (index + 1).__str__())
   index += 1

def contains(sourceCityId, destinationCityId):
  for i in routes: 
    if i["sourceCityId"] == sourceCityId and i["destinationCityId"] == destinationCityId:
      return True
  return False



def report():
  str = ''
  for item in results:
    str += item['sourceCityTitle'] + ' -> ' + item['destinationCityTitle'] + '\n'
  
  print(str)
  print('Result Count : ' + results.__len__().__str__())



# load data from database
getCities()
getRoutes()

# start 
startTime = datetime.now()

calculate()
report()

endTime = datetime.now()
# end

duration_time = endTime - startTime
print("Duration : {}".format(duration_time))


