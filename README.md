# MusicStoreApiWeb Api MusicStoreApi back-end application 

Full description in the directory: DescriptionOfApp/AppDescriptionInEnglish.pdf

Used: C#, ASP.NET Core (.NET 6.0), REST, Entity Framework, MS_SQL, Swagger(homepage) documentation, Microsoft Azure Cloud, NLogger, JWT Token, Postman, CORS policy, Middleware. 

POST -> api/account/register 
A program that presents a music shop portal, where you can register as a new user with a role: 
- USER -> you can view everything (GET), you cannot create (POST), delete (DELETE), update (UPDATE)
- PREMIUM_USER -> you can view everything (GET), create (POST) only unique artist names with albums and songs, and delete (DELETE) and update (UPDATE) only your created artists, albums, songs.
- ADMIN -> you can do everything, 

We send the information via JSON: firstName, lastName, email, password, confirmPassword, nationality, dateOfBirth, roleId. By the roleId variable you know what permissions a user has: 1 -> USER,
2 -> PREMIUM_USER, 3 -> ADMIN. Security: 
- if wrong date is entered, error message 
- if email is empty, error message -> "email must not be empty", if it is already in the database, error -> "that email is taken", if email is invalid, error -> "is not a valid email address" 
- if lastName and firsName are empty, the message -> "must not be empty" 
- if password is less than 6 characters, message -> "The lenght of Password must be at least 6 characters. You entered 5 characters" 
- if you enter confirmPassword not equal to password, then error -> "must be equal to value of password" 

When registering correctly, the new password is hashed and PasswordHash is saved to the database. 

POST -> api/account/login 

After that, you can log in to the music portal, using the sending of email information as login and password. Security: 
- if you enter an empty or invalid email, a corresponding error message 
- If you enter a wrong password or email, the error message -> "invalid username or password". 
- If you enter a password with less than 6 characters, an error message will appear. 

When logging into the music shop, the login (email) is checked to see if it is the same in the database, the password hash of the user (PasswordHash) is checked against the password hash (PasswordHash) 
which is in the database.  If everything is ok, the server generates a JWT token containing information about the user(CLAIMY): UserId, FirstName, LastName, RoleName, DateOfBirth. To be able to use all 
the actions in the api as a logged in user, you need to send in the header Key -> Authorization, and Value -> Bearer {the generated JWT Token that was generated at login}. 

Artist 
GET -> api/artist 
 You can search for all artists with its albums and songs, you need to specify two values in the parameters: PageSize -> 5, 10, 15 and PageNumber -> 1, 2, 3 ... etc. In addition, 3 more values can be entered: 
 SearchWord -> searches for a given word after given Name or Description records, SortDirection -> 0(asc), 1(desc), sorts ascending or descending, SortBy -> sorts after given Name, Description, KindOfMusic records. 
 In addition, result pagination(pagination) information with values is shown: TotalPages -> all possible pages, TotalItemsCount -> all records, ItemFrom -> shows the beginning of the page where the item starts,
 ItemTo -> shows the end of the page where the item ends. 

Security features: 
-if the number of all records to be displayed is less than or equal to PageSize * (PageNumber-1), then an error -> 400 Bad Request with the message "search result items of Artists: 15 is too small or equal,
because the number of skip 15, change the values in PageSize = 5, PageNumber = 1, to see the result " 
-if you specify the PageSize incorrectly, i.e. other than 5, 10, 15 , then error 400 Bad Request with the information -> must in [5, 10, 15]. 
If the PageNumber is specified incorrectly, i.e. other than 1, 2, 3...., then error 400 with the message -> "must be greater than or equal to 1" 

GET -> api/artist/{id} 
You can search by the id number of a given artist. 
Safeguards:
-if you specify an artistId that does not exist in the database, a 404 Not Found error with the information -> "Artist {artistId} is not found" 

POST -> api/artist 
You can create a new artist by specifying the values Name, Description, KindOfMusic, ContactEmail, ContactNumber, Country, City. Can only be used by a logged in user with the role: PREMIUM_USER and ADMIN. 
Security features: 
-if you enter City, Name, Country, Description as empty value, then error with message -> "field is required". 
-if you enter ContactEmail and ContactNumber wrong, then error with message -> "is not a valid {variable}"  
-if you enter an artist name that was already created by this user, error with info => "Name: invalid value because there is already on artist created by this user(duplicate)" 

DELETE -> api/artist/{id} 
You can delete a given album by specifying the album id. Only a user with the role: USER , who created this artist, or a user with the role: ADMIN can delete this album. 
Security features: 
-if you specify an artistId that does not exist in the database, an error with the message -> "Artist {artistId} is not found". 

PUT -> api/artist/{id} 
Artist can be updated, with values name, description, kindOfMusic, contactEmail, contactNumber, Country, City. Changes can only be done by a user with the role: PREMIUM_USER who created the artist in question
or a user with the role: ADMIN. 

Security features: 
-if you specify an artist name already in the database, created by a given user, error 409 Conflict with the message -> "Name : invalid value because there is already an artist created by this user (duplicate)". 
In albums and songs, additional possibility still to delete all albums or songs. Safeguards: 
-if there are no albums to delete, or no albums to display, the information -> "list of albums is empty"
-if there are no songs to delete, or no songs to display, the message -> "list of songs is empty" 

Albums and songs just like artists, can be created new, deleted, updated, displayed all or individually. In the search for all albums there is an additional option to add 3 values SearchWord -> search name after
the variable Title, SortBy -> Title sorts by this record, SortDirection -> 0 is ascending or 1 descending sorting. No additional filters in the song search. 

File 
GET -> file 
Possibility to read the content of a given PrivateFiles/private-file.txt text file that is located on the server. You must use the path url + file/?fileName=private-file.txt. Only users who are logged in can use it. 

If non-logged-in users try to use any action except the GET action, a message -> 401 Unauthorised will occur. 
If a logged-in user is not authorised for a PUT, DELETE or POST action, the message -> 403 Forbidden will appear.

Album  
POST -> api/artist/{artistId}/album 
Possibility to create a new album, you have to enter the values title, length -> length of the whole album, price. You cannot enter the same title that is already in Artist. 

PUT -> api/artist/{artistId}/album/{albumId} 
Ability to update the album data, providing values as in the POST action. 

GET -> api/artist/{artistId}/album -> possible to display all albums 

GET -> api/artist/{artistId}/album/{albumId} -> displays one album 

DELETE -> api/artist/{artistId}/album -> deletes all albums 

DELETE -> api/artist/{artistId}/album/{albumId} -> deletes one album 

Song 
POST -> api/artist/{artistId}/album/{albumId}/song -> creates a new unique song with name value 

GET -> api/artist/{artistId}/album/{albumId}/song -> displays all songs 

GET -> api/artist/{artistId}/{album/{albumId}/song/{songId} -> displays one song 

DELETE -> api/artist/{artistId}/album/{albumId}/song -> deletes all songs 

DELETE -> api/artist/{artistId}/album/{albumId}/song/{songId} -> deletes one song only 

PUT -> api/artist/{artistId}/album/{albumId}/song/{songId} -> updates a song, name has to be unique , not duplicate. 

In an album, an additional variable numberOfSongs shows the current number of songs in that album. When a song is added or deleted, this variable changes its value. 

The NLogger library is still used, for creating logs in the MusicStoreAPILogs directory, which is configured in the nlog.config file . It is best to change the path where you want to have the logs yourself 
-> all, exceptions, request-time -> occurs when the request to the server takes more than 4 seconds. 

Use of middleware as ErrorHandlingMiddleware class, which is responsible for analysing queries that come into the API and when an exception occurs, adds information about it to the exceptions logger. 

A CORS policy has been added so that in future queries can be sent to a particular api, from a different frontend domain. 

Deployed application to Azure cloud : https://musicstore-api-app9.azurewebsites.net/swagger

On the link https://musicstore-api-app9.azurewebsites.net/api/artist, you can test the operation of the application (please use Postman). About 148 artists have been added, created by the user with the email address: userpremium@gmail.com.
There are also 4 additional Users created:
1. USER about the role USER (RoleId = 1):
login/email: user2@gmail.com
password: password1

in Parameter in Postman -> Authorization = Bearer {Token JWT}

//////////////////////////////////////////////////////////////////

2. PREMIUM_USER about the role PREMIUM_USER (RoleId = 2): -> Creator 150 new artist in database
login/email: userpremium@gmail.com
password: password1

in Parameter in Postman -> Authentication = Bearer {Token JWT} 

//////////////////////////////////////////////////////////////////

3. PREMIUM_USER_2 about the role PREMIUM_USER (RoleId = 2):
login/ email : premiumuser2@gmail.com
password : password1

in Parameter in Postman -> Authentication = Bearer {Token JWT} 

Authentication = Bearer {Token JWT} 

////////////////////////////////////////////////////////////////////

4. ADMIN about the role ADMIN (RoleId = 3):
login / email: admin2@gmail.com
password: password1

in Parameter in Postman -> Authentication = Bearer {Token JWT} 


If you test applications locally, e.g. link: https://localhost:5110/api/artist, four users and 150 artists created by the user with the email address: userpremium@gmail.com will also be available, you just need to follow the instructions in visual studio 2022 community or VS code:
1. update-database
2. Then run the application, when you first start the application: 150 artists will be created, created by a user with email: userpremium@gmail.com and password: password1. For testing purposes, 4 users will be created with the roles 1->USER, 2->PREMIUMUSER, 2->PREMIUMUSER, 3->ADMIN. Their login and password are provided above. You need to log in to the application, generate a new JWT Token and you can test all actions in the Web API.