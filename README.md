# Ws Card Database Builder
This is a card database build tool for [Ws Deck Editor](https://gitlab.com/GroupAvalon/WsDeckEditor).

## Quick Start

1. Download the latest release from [here](https://gitlab.com/GroupAvalon/WsCardDatabaseBuilder/tags).
2. Unzip the file.
3. Start the executable.

## Parameter
**Force Download**
```
-f --force
```
This forces the program to ignore any cache data. All the data will be downloaded from internet.

**Disable Caching**
```
-c --cache
```
By default, all the card data will be cached locally in the `Cache` directory in JSON format. This prevents the program from adding cache files but if there are cache data, it still read from it.

**Output Path**
```
-p --path [Path]
```
The output path of the database file. By default, it is `wsdb.db` in the same directory.

**Version**
```
-v --version [Version]
```
Set the version number of the database. By default, it is 1.

**Help**
```
--help
```
Print out help.
