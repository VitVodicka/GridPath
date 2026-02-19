# GridPath

GridPath is a web application developed as part of a bachelor's thesis focused on optimizing cable routing between a renewable energy source and a distribution grid connection point.

The application helps identify suitable land parcels for routing infrastructure by analysing cadastral data and computing an optimal path between two selected locations.

## How it works

The user selects:

- Point A (starting location)
- Point B (destination)

The system retrieves land parcels between these points using the Czech Land Registry REST API.  
Each parcel is evaluated based on predefined criteria such as land type and restrictions.

The parcels are converted into a grid structure and a modified Dijkstra-based algorithm is used to find an optimal routing corridor.

The result is a list of parcels representing a suggested cable route together with their definition points.

## Technologies

C#  
ASP.NET Core MVC  
REST API (Czech Land Registry)  
MVC architecture

## Limitations

- Uses only cadastral data
- Limited by API requests
- Produces a parcel corridor rather than a final cable line
