# Ws Card Database Builder
This is a card database build tool for [Ws Deck Editor](https://github.com/joshuaavalon/WsDeckEditor).

## Quick Start

1. Download the latest release from [here](https://github.com/joshuaavalon/WsCardDbBuilder/releases).
2. Unzip the file.
3. Start the executable by `WSCDB -c`. Or use `WSCDB -c -s` for quick rebuild if you sure your serials are up-to-date.

## Parameter
**Disable Caching**
```
-d --disablecache
```
By default, all the card data will be cached locally in the `Cache` directory in JSON format. This prevents the program from adding cache files but if there are cache data, it still read from it.

**Use Card Cache**
```
-c --cardcache
```
Use cached card data if possible.

**Use Serial Cache**
```
-s --serialcache
```
Use cached serial data if possible.

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

**Output Path**
```
-k --cachepath [Path]
```
Set the path of the cache directory.


**Help**
```
--help
```
Print out help.
