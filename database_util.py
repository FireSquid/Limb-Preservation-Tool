import sqlite3 as sql3


def connect(db_name):
    return sql3.connect(db_name)
