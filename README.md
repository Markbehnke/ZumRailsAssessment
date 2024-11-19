# Pokemon Tournament Simulator  

The **Pokemon Tournament Simulator** is a .NET Web API that simulates a round-robin Pokemon tournament. It integrates with the PokeAPI to fetch data, processes tournament logic, and returns detailed statistics about the results.  

---

## Technical Features  

- **Framework:** ASP.NET 8.0
- **Caching:** Uses `IMemoryCache` to minimize external API calls and improve performance.  
- **External API Integration:** Fetches Pokemon data from [PokeAPI](https://pokeapi.co).  
- **Sorting:**  Sorting results via query parameters.  
- **Error Handling:** Implements exception handling for API timeouts, invalid inputs, and edge cases.  
- **Separation of Concerns:** Architecture with distinct layers for controllers, services, and DTO models, following SOLID principles.  

---

## Endpoints  

### Fetch Tournament Statistics  

**Endpoint:**  
http
GET /pokemon/tournament/statistics?sortBy={sortBy}


Parameters
- `sortBy` (required): Field to sort by (`wins`, `losses`, `ties`, `name`, `id`).
- `sortDirection` (optional): Direction to sort (`asc`, `desc`). Defaults to descending if left blank

<br>
Example:
http
  GET pokemon/tournament/statistics?sortBy=wins
Example:
 <pre>[
  {
    "id": 131,
    "name": "lapras",
    "type": "water",
    "wins": 6,
    "losses": 1,
    "ties": 0
  },
  {
    "id": 68,
    "name": "machamp",
    "type": "fighting",
    "wins": 5,
    "losses": 2,
    "ties": 0
  },
 ...
 ]</pre>
Example with sortDirection specified:
http
  GET pokemon/tournament/statistics?sortBy=wins&sortDirection=asc
<pre> 
 [
  {
    "id": 116,
    "name": "horsea",
    "type": "water",
    "wins": 0,
    "losses": 7,
    "ties": 0
  },
  {
    "id": 90,
    "name": "shellder",
    "type": "water",
    "wins": 1,
    "losses": 6,
    "ties": 0
  },
  ...
]</pre>

## How to Run

Follow these steps to run the project locally:

1. **Clone the Repository**  
   Clone the repository to your local machine:  
   `git clone https://github.com/Markbehnke/ZumRailsAssessment.git`  
   `cd ZumRailsAssessment`  

2. **Set Up the Backend (Web API)**  
   Navigate to the backend project folder:  
   `cd PokemonWebAPI`
   
   Restore the required dependencies:  
   `dotnet restore`
   
   Run the Web API:  
   `dotnet run`
   
   The API will start on http://localhost:7252.

4. **Set Up the Frontend (Angular App)**  
   Navigate to the frontend project folder:  
   `cd PokemonFrontend`
   
   Install the required dependencies:  
   `npm install`
   
   Run the Angular development server:  
   `ng serve`
   
   The frontend will start on http://localhost:4200.

5. **Access the Application**  
   Open your browser and navigate to http://localhost:4200.  
   The application will communicate with the Web API to fetch and display data.

6. **Testing the API**  
   You can test the API endpoints using a tool like Postman or cURL.

   **Example:**  
   Fetch tournament statistics using the following endpoint:  
   `GET /pokemon/tournament/statistics?sortBy=wins`

## Swagger Documentation

To access the Swagger UI for this API, follow these steps:

    Ensure the backend API is running on http://localhost:7252
    Open a browser and navigate to:
    http://localhost:7252/swagger
    

## Running Tests

To run tests for the backend, use:
`dotnet test`


## Future Considerations

The API is designed to scale if we decide to increase the number of Pokemon in the tournament (e.g., going beyond 8 participants). The logic accounts for the possibility of repeatedly selecting the same random number by handling retries within the loop where Pokemon IDs are generated (between 1 and 151).
Caching has been implemented to reduce redundant API calls, ensuring that previously fetched Pokemon data is reused efficiently.
Pagination was considered for the frontend to manage large Pokemon datasets. However, it was not implemented as the rubric did not require it. If we decide to scale the tournament and increase the Pokemon pool significantly, pagination should be implemented to improve performance and user experience.
