# Jobsity

Chat Bot

This project was very challenging because I had never worked with RabbitMQ.

Which was very helpful because I learned a new technology and increased my skills.

Unfortunately I have to apologize for the delivery times. Due to external factors of my country for the company in which I currently work, I had a very special sales weekend in terms of buying services and products online. Which caused an overload of work that caused the delay.

# Instructions

## Docker
I setup a Docker folder with all configured to create an instance of RabbitMQ Server.

Command:

docker-compose up

##S ervice

I create a Service REST API Project where all request should be processed. It was developed with asp.net core 3.0

http://localhost:7000/v1/ + [Controller]

For database creation you can set an application.properties like this:

"Development": {
    "DbInstall": true
  },

## Web

The web application is an asp.net core web that use bootstrap and jquery frameworks

http://localhost:7001/

## Unit Testing

I created the Service.Test project with the unit test.

## Plus Postman

I addded a new folder for postman collection that You could be use for testing the Web API.



