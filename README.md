# Mongo JWT Login

A simple project using MongoDB and jsonwebtoken for authentication.

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
