## Routes

### Get all statuses

```
GET /api/status
```

#### Response

```
200 OK

[
  {
    "key": "1",
    "retryAfterInSeconds": 300,
    "circuitState": "Open"
  },
  {
    "key": "2",
    "circuitState": "Closed"
  },
  {
    "key": "3",
    "circuitState": "HalfOpen"
  }
]
```

Notes
- `circuitState` is one of: `Open`, `Closed`, `HalfOpen`

### Get status for key

```
GET /api/status/:key
```

#### Response

```
200 OK

{
  "key": "1",
  "retryAfterInSeconds": 300,
  "circuitState": "Open"
}
```

Notes
- `circuitState` is one of: `Open`, `Closed`, `HalfOpen`

### Record a call failure for key

```
POST /api/calls/:key/failure

{
  "retryAfterInSeconds": 300
}
```

#### Request

- Request body is optional.
- `retryAfterInSeconds` is optional, but if supplied it must be a valid number.

#### Response

```
200 OK
```

### Record a call success for key

```
POST /api/calls/:key/success
```

#### Response

```
200 OK
```
