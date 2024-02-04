PixelCelebrateBackend Project

Steps for running the app:

- fill in the details in `appsettings.json`;
- run `dotnet run` in terminal;

The app contains:

- entities and migrations for creating the database tables;
- dtos for sending/receiving data from client app;
- controllers that present an API that uses HTTP requests which can be called by client apps for manipulating the db data (db is configured in `appsettings.json`);
- email service that can be called to send an email to the server configured in `appsettings.json`;
- a background scheduler that verifies every day if there are users with birthdays in `x` days (`x` being a configuration value stored in the database), and sends emails to notify all the other users.
