# Data

Scripts to build the `worldolio.sqlite` database

- `V2` various data artifacts rescued from Worldolio v2, these have been used to produced the v3 DB and are no longer used and are included for reference only.
- `V3` scripts to build the `worldolio.sqlite` v3 database

## V2

The data was originally managed in an Access `mdb`. Either use Access of you can use LibraOffice to look at the file

## V3

This is where we can build a new DB

### Original data

Edit the city and country data as required. `city.csv` and `country.csv` are plain text data files that are loaded into the sqlite db

### Building the db

#### Edit the schema

The file `worldolio_schema_sqlite.sql` contains the script needed to build the DB

Locate the line

```
PRAGMA user_version = 1;
```

And increment the version, in this case from 1 to 2. This will appear as the schema version in the helper code and also the schema as reported in Android.

#### Build the DB

Note that running the script will delete `worldolio.sqlite` and then recreated it, unless it fails

Run `buildDB.bat`. 

Any errors in the foreign keys will throw an error in the script.

The IANA TZ names cannot be checked in SQL, use the `WorldolioDataChecker` tool in the `POC` folder to check the TZ names.

#### Copy the DB

This is a convenience script, you can copy the `worldolio.sqlite` manually if you prefer.

Run `copyDB.bat`. 

This will copy the newly built DB to various locations in the build tree. For instance

- WorldolioDataChecker App - the folder where the Windows debug build is created
- WorldolioCLI App - the folder where the Windows debug build is created
- WorldolioPOC App - the folder where the Windows debug build is created
- WorldolioMauiPOC App - the db is copied into the resources folder and will be build into the WorldolioMauiPOC app on next *rebuild*. Often a build is not enough for VS.