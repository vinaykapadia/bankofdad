# The Bank of Dad

** Filling more of this out later (who am I kidding, its never getting done). **

## Settings
There are required settings in [appsettings.json](BankOfDad/BankOfDad.Web/appsettings.json).
* Secret: This can be any string. Should be long. It is used to generate and read tokens. If it is ever change, it will just invalidate all older tokens (they would be regenerated next time someone logs in).
* ConnectionString: The connection string to your database.

## Projects
### BankOfDad.Client
Wrapper for the HTTP client that connects to the web service. Used in the GUI, the Website accesses data directly.
### BankOfDad.Dadabase
Context and models for database access.
### BankOfDad.Gui
The MAUI App.
### BankOfDad.Models
The data contracts used to communicate with the Web API.
### BankOfDad.Web
Contains both the Web API and the Website.
