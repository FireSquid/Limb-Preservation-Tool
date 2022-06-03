import sqlite3
import argon2
import secrets
import time

ph = argon2.PasswordHasher()

DATABASE = "user_data.db"

class UnknownUser(Exception):
    pass

class UserAlreadyExists(Exception):
    pass

def authenticate_login(username, password):
    try:
        hash_str = get_hash_for_user(username)

        ph.verify(hash_str, password)

        if ph.check_needs_rehash(hash_str):
            set_hash_for_user(username, ph.hash(password))
    except argon2.exceptions.VerifyMismatchError:
        print("Incorrect Username or Password")
    except argon2.exceptions.VerificationError:
        print("Verification Failed")
    except argon2.exceptions.InvalidHash:
        print("Bad Password Hash")
    except UnknownUser:
        print("User Does Not Exist")
    else:
        return True
    return False

def create_user(username, password, name, email):
    if user_exists(username):
        raise UserAlreadyExists("User already exists")

    com = "INSERT INTO users(username, password, name, email) VALUES(?, ?, ?, ?)"
    args = (username, ph.hash(password), name, email, )

    con = db_connect()
    con.cursor().execute(com, args)
    con.commit()
    con.close()

def delete_user(username, password):
    if not user_exists(username):
        raise UnknownUser("Cannot find user to delete")

    if not authenticate_login(username, password):
        return False

    com = "DELETE FROM users WHERE username = ?"
    args = (username, )

    con = db_connect()
    con.cursor().execute(com, args)
    con.commit()
    con.close()

    return True

def get_hash_for_user(username):
    if not user_exists(username):
        raise UnknownUser("User not found in database")

    com = "SELECT password FROM users WHERE username = ?"
    args = (username, )

    con = db_connect()
    user_hash = con.cursor().execute(com, args).fetchone()[0]
    con.close()
    return user_hash

def set_hash_for_user(username, password_hash):
    if not user_exists(username):
        raise UnknownUser("User not found in database")

    com = "UPDATE users SET password = ? WHERE username = ?"
    args = (password_hash, username, )

    con = db_connect()
    con.cursor().execute(com, args)
    con.commit()
    con.close()

def user_exists(username):
    com = "SELECT EXISTS(SELECT 1 FROM users WHERE username = ?)"
    args = (username, )

    con = db_connect()
    user_ex = con.cursor().execute(com, args).fetchone()[0]
    con.close()
    return user_ex

def check_token(username, token):
    com = "SELECT EXISTS(SELECT 1 FROM authtokens WHERE (username = ? AND token = ?))"
    args = (username, token, )

    con = db_connect()
    auth_token = con.cursor().execute(com, args).fetchone()[0]
    con.close()

    return bool(auth_token)

def make_token(username):
    if not user_exists(username):
        raise UnknownUser("User not found in database")

    new_token = secrets.token_urlsafe(48)

    com = "INSERT INTO authtokens(token, username, expire_time) VALUES(?, ?, ?)"
    args = (new_token, username, int(time.time()), )

    con = db_connect()
    con.cursor().execute(com, args)
    con.commit()
    con.close()

    return new_token


def db_connect():
    return sqlite3.connect(DATABASE)
