# URL Shortener API

## Description

The URL Shortener API is a service that allows you to shorten long URLs into shorter, more manageable links. It utilizes Redis for caching the original URLs and creates a table with the endpoints for easy access. The API provides two endpoints, one for creating shortened URLs and another for redirecting users to the original URLs.

## Installation

1. Clone the repository from GitHub: `git clone https://github.com/fernanduandrade/url-shortener.git`
2. Navigate to the project directory: `cd 
url-shortener`
3. Install the required dependencies: `dotnet run`

## Usage

### Start the API server

To start the URL Shortener API server, run the following command:


By default, the server will run on `http://localhost:5218`.

### Create Shortened URL

**Endpoint**: `POST http://localhost:5218/create`

**Payload**:

The request payload should be in JSON format and contain the original URL that you want to shorten:

```json
{
  "url": "https://www.example.com/your/long/url"
}
```

**Response**:

Upon successful creation of the shortened URL, the API will respond with a JSON object containing the shortened URL and a unique hash identifier:

```json
{
  "url": "https://www.example.com/go/hashId"
}
```

### Redirect to Original URL
Endpoint: GET http://localhost:5218/go/{hashId}

Parameters:

Replace {hashId} with the hash identifier obtained from the response when creating the shortened URL.

**Response**:

When a user accesses the shortened URL, the API will redirect them to the original URL associated with the given hash identifier.

If the provided {hashId} does not exist or has expired, the API will respond with an appropriate error message.

### Caching with Redis
The API uses Redis to cache the original URLs, which helps improve response times and reduce the load on the backend. When a shortened URL is created, it is stored in Redis with an expiration time. By default, the shortened URLs expire after a certain period, but this can be configured as per your requirements.

### Contributing
Contributions to the URL Shortener API are welcome! If you find any bugs or have suggestions for improvements, please feel free to create an issue or submit a pull request on the GitHub repository.

