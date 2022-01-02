const { MongoClient } = require('mongodb');
const express = require('express')
const app = express()
const crypto = require('crypto')
const jwt = require('jsonwebtoken')
require('dotenv').config()

app.use(express.json({limit: '64mb'}));

app.post('/signup', (req, res) => {
    const { username, password } = req.body
    const salt = crypto.randomBytes(64).toString('hex')
    const hash = crypto.createHmac('sha512', salt).update(password).digest('hex')

    req.app.locals.db.collection('users').insertOne({ username, salt, hash }, (err, result) => {
        if (err) {
            res.status(500)
            res.json({message: err})
        }
        else {
            res.json(result)
        }
    })
})

app.post('/login', (req, res) => {
    const { username, password } = req.body

    req.app.locals.db.collection('users').findOne({ username: username }, (err, fullUser) => {
        if (err) {
            res.status(500)
            res.json({message: err})
        }
        const hash = crypto.createHmac('sha512', fullUser.salt).update(password).digest('hex')
        if (hash === fullUser.hash) {
            res.status(201)
            const signedUser = {username: fullUser.username}
            jwt.sign(signedUser, process.env.SECRET, (err, token) => {
                if (err) {
                    res.sendStatus(403)
                } else {
                    res.status(201)
                    res.json({token})
                }
            })
        }
        else {
            res.status(403)
            res.json({message: 'invalid credentials'})
        }
    })
})

function gracefulShutdown() {
    app.locals.db_connection.close(() => {
        console.log("mongodb connection is closed")
        process.exit()
    })
}

process.on('SIGINT', gracefulShutdown);
process.on('SIGTERM', gracefulShutdown);
process.on('SIGKILL', gracefulShutdown);

MongoClient.connect(process.env.MONGO_URI, (err, db) => {
    if (err) throw err;

    app.locals.db_connection = db;
    app.locals.db = db.db(process.env.DB_NAME);

    app.listen(process.env.PORT, () => {
        console.log(`serving on port ${process.env.PORT}`)
    })
})