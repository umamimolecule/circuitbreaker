# Performance test

## Requirements

- Install K6 (https://k6.i)

## Usage

1. Configure the API endpoint being called in `api.js`.
2. Configure the number of unique keys (keys are the unique identifier for circuit breakers, and represent tenants in downstream services).
3. Run the following command:

```
k6 run ./index.js -e HOSTNAME=myhostname.com [--duration 60s] [-e RATE=2000] [-e KEY_COUNT=1000] [-e HOSTNAME=myhostname.com] [-e USE_HTTP=false]
```

Where:

- `-e HOSTNAME`: Specifies the host name of the service being tested. Required.
- `--duration`: Specifies the test run duration. Optional, default value = `60s`.
- `-e RATE`: Specifies the request rate per minute. Optional, default value = `2000`.
- `-e KEY_COUNT`: Specifies the number of unique circuit breaker keys. Optional, default value = `1000`.
- `-e USE_HTTP`: Specifies to use HTTP instead of HTTPS. Optional, default value = `false`.
