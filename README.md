# Mongo JWT Login

A simple project using MongoDB and jsonwebtoken for authentication.

## Setup

This project requires a `.env` file with the following properties:

```
MONGO_URI
DB_NAME
PORT
SECRET
EXPIRES_IN // e.g. '15m' or '4d'
```

You can generate a new `SECRET` by starting node in the console, then running:

```
require('crypto').randomBytes(64).toString('hex')
```

## Usage

```
POST /signup with {username, password}
```

- checks for existing user, generates salt, hashes password and inserts into `users` collection
- responds with the db result object

```
POST /login with {username, password}
```

- hashes password and compares with hash of user with same username 
- responds with `{token}`

```
GET /data
```

- protected route
- uses middleware which gets and verifies the token from the headers, then passes on the decrypted token
- responds with `{message: 'hello <username>'}`
