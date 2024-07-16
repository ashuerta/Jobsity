# Jobsity

Chat Bot

##### Mandatory Features (Copy from PDF)
<ul>
	<li> -Allow registered users to log in and talk with other users in a chatroom.</li>
	<li> -Allow users to post messages as commands into the chatroom with the following format /stock=stock_code</li>
	<li> -Create a decoupled bot that will call an API using the stock_code as a parameter
(https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv, here aapl.us is the
stock_code).</li>
	<li> -The bot should parse the received CSV file and then it should send a message back
	into the chatroom using a message broker like RabbitMQ. The message will be a stock quote
using the following format: “APPL.US quote is $93.42 per share”. The post owner will be
the bot.</li>
	<li> -Have the chat messages ordered by their timestamps and show only the last 50
messages.</li>
	<li> -Unit test the functionality you prefer.</li>
</ul>

##### Bonus (optional)
<ul>
	<li> -Have more than one chatroom.</li>
	<li> -Use .NET identity for users authentication.</li>
	<li> -Handle messages that are not understood or any exceptions raised within the bot.</li>
</ul>

This project help me a lot to start working with RabbitMQ which was very helpful because I learned a new technology and increased my skills.

### Instructions

#### Docker
I setup a Docker folder with all configured to create an instance of RabbitMQ Server.

Command:

<b>docker-compose up -d</b>

It will be create all docker stuff that you need to have SQL database and RabbitMQ services and all  pluggins that you will need.

<img width="1352" alt="image" src="https://github.com/user-attachments/assets/868e45e0-0237-46f2-aaec-65a32c15c309">

#### Service

I create a Service REST API Project where all request should be processed. It was developed with asp.net core 7 using MAC OS system

[http://localhost:5250/swagger/index.html]

<img width="1522" alt="image" src="https://github.com/user-attachments/assets/61cfa8b5-ae02-4c7f-bf0f-62ed98ed4c75">

#### Database

For database creation you can set an application.properties like this:

You will need to run migations

![image](https://github.com/user-attachments/assets/977e339c-a397-4b82-9a8b-e5017f996357)

- [dotnet ef database update]

![image](https://github.com/user-attachments/assets/4cbbf193-b098-4f5a-b43f-4dd474888a38)

- [Update-Database]

From Web API Project Folder
![image](https://github.com/user-attachments/assets/7ad8ef3d-7dea-4d9e-96d7-14d7772473e1)

#### RabbitMQ 

Probably you need to create a new user [ jobsityrmq ] with same password [ jobsityrmq ] In order to connect to RabbitMQ it will have defaults like follows:

Default RabbitMQ Services Portal
<img width="471" alt="image" src="https://github.com/user-attachments/assets/6701a4a0-6c9f-4f6d-bf67-efd34453d30b">
Create jobsityrmq
<img width="737" alt="image" src="https://github.com/user-attachments/assets/04e10b2d-a247-444f-be57-8a099f3e4ee5"> 

Nota: or you can use default Admin [ guest / guest ] but I don't recommend it.

#### Web

The web application is an asp.net core web that use bootstrap and jquery frameworks

[http://localhost:7001/](http://localhost:5006/)

[Login]
<img width="1201" alt="image" src="https://github.com/user-attachments/assets/29ad35c4-d84b-4fe8-881b-62ad47f44fd8">

[Register]
<img width="1052" alt="image" src="https://github.com/user-attachments/assets/3b5058ed-0937-4c3f-a75b-d2f04fc6d432">

[Dashboard]
<img width="1323" alt="image" src="https://github.com/user-attachments/assets/20d17441-0cfd-415d-826b-b093aff762bf">

#### Plus Postman

I addded a new folder for postman collection that You could be use for testing the Web API.
