
-- JAHNVI PATEL (jpate201)
-- U. of Illinois, Chicago
-- CS 480, Summer 2018
-- Project 3
--


--Customer information. Also keeps track of whether the customer is out with a 
--bike or not (boolean)
--Have Customer ID start at 20001, increment by 1
CREATE TABLE Customer (
    CID 		    INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    First_name		VARCHAR(250) NOT NULL,
    Last_name		VARCHAR(250) NOT NULL,
	email_address	VARCHAR(500) NOT NULL
);


--Keeps track of different types of Bikes at the shop
--Bike Type ID's start at 1, increment by 1
CREATE TABLE Bike_Type (
    Bike_Type_ID    INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    Description     TEXT,
    Price_hour      MONEY NOT NULL
);

--Each bike has a unique BID, stores it's type and year added to BikeHike
--Each bike's ID starts at 1001, increments by 1
CREATE TABLE Bike (
    BID             INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    Bike_Type_ID    INT NOT NULL FOREIGN KEY REFERENCES Bike_Type(Bike_Type_ID),
    Year_In_Service SMALLINT NOT NULL
);

--Keeps track of each time a bike is rented out to a customer 
--Each rent ID starts at 1, increments by 1
CREATE TABLE Rent (
    RID             INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    BID             INT NOT NULL FOREIGN KEY REFERENCES Bike(BID),
    CID             INT NOT NULL FOREIGN KEY REFERENCES Customer(CID),
    Borrowed_Time   DATETIME NOT NULL,
    Returned_Time   DATETIME NOT NULL, 
	Bike_status		BIT			--0 = Bike is returned, 1 = Out with a bike
);


--indexes created 
create index rent_cid
on rent(cid);

create index rent_bid
on rent(bid);

