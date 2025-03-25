# SMS Rate Limiter Microservice

This .NET Core (C#) microservice checks whether an SMS message can be sent from a given business phone number without exceeding the provider’s limits. It is designed to avoid unnecessary API calls (and associated costs) by enforcing:

- **Per-Number Limit:** The maximum number of messages that can be sent from a single business phone number per second.
- **Global Limit:** The maximum number of messages that can be sent across the entire account per second.

The microservice exposes a RESTful endpoint for this check and includes unit tests to demonstrate how the service behaves under various conditions.

---

## Features

- **Resource Management:**  
  Automatically cleans up outdated entries to ensure efficient memory usage.

- **RESTful API Endpoint:**  
  Provides a `POST /sms/can-send` endpoint which accepts a JSON payload and returns whether an SMS can be sent.

- **Unit Testing:**  
  Includes tests (using MSTest) to verify service behavior when limits are approached, exceeded, and after the window resets.

---

## Running the Microservice
#### Start the microservice by running:

`dotnet run`

The service will listen on a configured port (e.g., http://localhost:5291). Use Postman, curl, or any HTTP client to test the API.

---

## API Endpoint

### POST /sms/can-send

This endpoint checks if an SMS message can be sent without exceeding the provider’s rate limits. It validates the input and returns whether the SMS can be sent based on both per-number and global limits.

---

#### Request

- **URL:** `/sms/can-send`
- **Method:** `POST`
- **Headers:**
  - `Content-Type: application/json`
- **Body:**

```json
{
  "PhoneNumber": "+14168585949"
}
```

---

## Running Unit Tests
#### Run the unit tests for the service by running:

`dotnet run`

This endpoint checks if an SMS message can be sent without exceeding the provider’s rate limits. It validates the input and returns whether the SMS can be sent based on both per-number and global limits.

---
