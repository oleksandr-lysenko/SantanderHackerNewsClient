# HackerNewsClient

HackerNewsClient is a .NET 6.0 web application that fetches and displays the best stories from Hacker News.
To avoid hitting the Hacker News API rate limit, the application caches the stories in memory for 10 minutes.

As for a future improvement, I would add CI/CD pipeline to automate the deployment process (could be GitHub Actions).

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker](https://www.docker.com/get-started)

## Getting Started

### Running Locally

1. Clone the repository:
    ```sh
    git clone https://github.com/oleksandr-lysenko/SantanderHackerNewsClient.git
    cd HackerNewsClient
    ```

2. Restore the dependencies:
    ```sh
    dotnet restore
    ```

3. Build the project:
    ```sh
    dotnet build
    ```

4. Run the application:
    ```sh
    dotnet run
    ```

5. Open your browser and navigate to `http://localhost:5501/swagger` to see the Swagger UI.

### Running with Docker

1. Build the Docker image:
    ```sh
    docker build -t hackernewsclient .
    ```

2. Run the Docker container:
    ```sh
    docker run --rm -d -p 5501:80 --name hackernewsclientcontainer -e PORT=80 hackernewsclient
    ```

3. Open your browser and navigate to `http://localhost:5501/swagger` to see the Swagger UI.

4. Stop the Docker container when it is not needed:
    ```sh
    docker stop hackernewsclient
    ```

## Project Structure

- [Controllers](https://github.com/oleksandr-lysenko/SantanderHackerNewsClient/tree/main/Controllers): Contains the API controllers.
- [Models](https://github.com/oleksandr-lysenko/SantanderHackerNewsClient/tree/main/Models): Contains the data models.
- [Services](https://github.com/oleksandr-lysenko/SantanderHackerNewsClient/tree/main/Services): Contains the service classes.
- [Properties](https://github.com/oleksandr-lysenko/SantanderHackerNewsClient/tree/main/Properties): Contains the launch settings.
- [Program.cs](https://github.com/oleksandr-lysenko/SantanderHackerNewsClient/blob/main/Program.cs): The entry point of the application.
- [Dockerfile](https://github.com/oleksandr-lysenko/SantanderHackerNewsClient/blob/main/Dockerfile): The Dockerfile for building the Docker image.
- [appsettings.json](https://github.com/oleksandr-lysenko/SantanderHackerNewsClient/blob/main/appsettings.json): The configuration file for the application.
- [appsettings.Development.json](https://github.com/oleksandr-lysenko/SantanderHackerNewsClient/blob/main/appsettings.Development.json): The configuration file for the development environment.

## API Endpoints

- `GET /api/hackernews/beststories?n={number}`: Fetches the top [n](http://_vscodecontentref_/8) best stories from Hacker News.

## Using cURL

You can use the following cURL command to fetch the top 333 best stories from Hacker News:

```sh
curl -X 'GET' \
  'http://localhost:5501/api/HackerNews/beststories?n=18' \
  -H 'accept: */*'
```

## License

This project is licensed under the MIT License.