# FileManagement
## End Points
POST        ​/api​/file-management​/CreateFile
POST        ​/api​/file-management​/UpdateFile
POST        ​/api​/file-management​/DeleteFile
GET         ​/api​/file-management​/ListFiles
GET         ​/api​/file-management​/ListAllVersionsOfaFile
GET         ​/api​/file-management​/ListAllFilesAndVersions

### CreateFile
This end-point inserts a file in the database. This post call accepts a form-data parameter called inputfile. The field accepts a file as input. If a file with the same name was found in the database, it does not insert a file.
#### Responses
If the file does NOT get inserted, we get 200 with this message "The file already exists in the database. Nothing got updated"
If the file gets inserted, we get 201 with this message "The file has been inserted into the database"

### UpdateFile
This POST call looks for a file in the database. If the file exists, increments the version of the file and inserts a new line in the database.
If the file does NOT exist, it does nothing.
#### Responses
If a file with the same name exists, we get 201 with this message "The new version of the file got inserted into the database"
If the file does NOT exist, we get 400 with this message "The file does NOT exist in the database. Please insert it first then try to update it"

### DeleteFile
This POST call deletes the provided file with the right version. If the version is not provided, it deletes the latest version of the file. If the version is provided but does not match any of the current versions, it does not delete anything. This is the payload
{
    "inputfilename" : "Passport.jpg",
    "version" : 1
}

#### Responses
If the file's name and its version match, we get 200 with this message "The file got deleted"
If the name or the version does not match we get 400 with this message "The file did NOT get deleted because the file with the provided version does NOT exist in the database"

### ListFiles
Returns the files regardless of their version. If multiple versions exist, it returns one record for that file. The response is 200

### ListAllVersionsOfaFile
It returns all versions of a provided file name

### ListAllFilesAndVersions
Returns all files and all versions of all files

## Database
The backend database of this API is SQLite
We use Entity Framework and migration to create the database. This is a good example to show how to generate the database: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
These are the .NET Core CLI commands for running the Migration
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate
dotnet ef database update

## Postman scripts
Postman scripts are provided under the folder "PostmanCollection"
The other way to hit the endpoints is by using Swagger

## Next Steps
These are the improvements that can be done in the future:
Enable multiple files to upload, update, and delete. Currently, we support single-file manipulation.
Create a file system for the actual files. Currently, we only use the files' metadata for the database inserts.
After creating a file system, the database can be used as a dictionary for tracking the files.
Save the file stream in the database. Create an endpoint for the Get file. If a user sends a Get request, we should write the stream into a file and send it to the user.
Add logging to track information and exceptions.

## API Demo
https://www.youtube.com/watch?v=U4aGpJ1GP4I
