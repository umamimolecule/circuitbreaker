# Performance test

## Requirements

Install K6 (https://k6.i)

## Usage

1. Configure the API endpoint being called in `api.js`.
2. Configure the number of unique keys (keys are the unique identifier for circuit breakers, and represent tenants in downstream services).
3. Run the following command:

```
k6 run ./index.js
```
