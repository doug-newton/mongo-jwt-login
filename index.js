const { MongoClient } = require('mongodb');
const express = require('express')
const crypto = require('crypto')
const jwt = require('jsonwebtoken')
require('dotenv').config()

const app = express()
app.use(express.json({limit: '64mb'}));

/*
    creates user with salt and hash
*/

app.post('/signup', (req, res) => {
    const { username, password } = req.body
    const salt = crypto.randomBytes(64).toString('hex')
    const hash = crypto.createHmac('sha512', salt).update(password).digest('hex')

    req.app.locals.db.collection('users').findOne({ username: username }, (err, fullUser) => {
        if (err) {
            res.status(500)
            res.json({ message: err })
        }
        else if (fullUser) {
            res.status(400)
            res.json({ message: 'a user already exists with that name' })
        }
        else {
            req.app.locals.db.collection('users').insertOne({ username, salt, hash }, (err, result) => {
                if (err) {
                    res.status(500)
                    res.json({ message: err })
                }
                else {
                    res.json(result)
                }
            })
        }
    })
})

/*
    hashes password using user's salt
    compares hash against db
    sends back token if successful
*/

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
            jwt.sign(signedUser, process.env.SECRET, {expiresIn: process.env.EXPIRES_IN}, (err, token) => {
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

/*
    jwt authentication middleware
    expects header `Authorization: Bearer <token>`
*/

function auth(req, res, next) {
    const token = req.headers['authorization']?.split(' ')[1]
    if (token) {
        req.token = token
        jwt.verify(req.token, process.env.SECRET, (err, signedUser) => {
            if (err) {
                res.sendStatus(403)
            } else {
                res.locals.user = signedUser
                next()
            }
        })
    } else {
        res.sendStatus(403)
    }
}

/*
    protected route using data from verified token
*/

app.get('/data', auth, (req, res) => {
    const user = res.locals.user
    console.log(user)
    res.send({message: 'hello ' + user.username})
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