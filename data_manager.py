import database_util as du

DB = "user_data.db"

class UnknownEntry(Exception):
    pass

class EntryAlreadyExists(Exception):
    pass

def get_patient_id(patient_username):
    if not patient_name_exists(patient_username):
        raise UnknownEntry("Could not find patient id")

    com = "SELECT 1 FROM patients WHERE username = ?"
    args = (patient_username, )

    con = du.connect(DB)
    patient_id = con.cursor().execute(com, args).fetchone()[0]
    con.close()

    return patient_id

def patient_name_exists(patient_username):
    com = "SELECT EXISTS(SELECT 1 FROM patients WHERE username = ?)"
    args = (patient_username, )

    con = du.connect(DB)
    patient_ex = con.cursor().execute(com, args).fetchone()[0]
    con.close()

    return patient_ex

def patient_exists(patient_id):
    com = "SELECT EXISTS(SELECT 1 FROM patients WHERE id = ?)"
    args = (patient_id, )

    con = du.connect(DB)
    patient_ex = con.cursor().execute(com, args).fetchone()[0]
    con.close()
    
    return patient_ex

def print_all_patients():
    com = "SELECT * FROM patients"

    con = du.connect(DB)
    patients_table = con.cursor().execute(com).fetchall()
    con.close()

    print(patients_table)

def delete_patient(patient_id):
    if not patient_exists(patient_id):
        raise UnknownEntry("Could not find patient to delete")

    com = "DELETE FROM patients WHERE id = ?"
    args = (patient_id, )

    con = du.connect(DB)
    con.cursor().execute(com, args)
    con.commit()
    con.close()

def create_patient(patient_username):
    if patient_name_exists(patient_username):
        raise EntryAlreadyExists("Patient username already taken")

    com = "INSERT INTO patients(username) VALUES(?)"
    args = (patient_username, )

    con = du.connect(DB)
    con.cursor().execute(com, args)
    con.commit()
    con.close()

    return True

def get_all_patient_wifi(patient_id):
    if not patient_exists(patient_id):
        raise UnknownEntry("Could not find patient data")

    com = "SELECT * FROM wifi WHERE patient_id = ?"
    args = (patient_id, )

    con = du.connect(DB)
    wifi_data = con.cursor().execute(com, args).fetchall()
    con.close()

    return wifi_data

def wifi_exists(wifi_id):
    com = "SELECT EXISTS(SELECT 1 FROM wifi WHERE id = ?)"
    args = (wifi_id, )

    con = du.connect(DB)
    wifi_ex = con.cursor().execute(com, args).fetchone()[0]
    con.close()

    return wifi_ex

def print_all_wifi():
    com = "SELECT * FROM wifi"

    con = du.connect(DB)
    wifi_table = con.cursor().execute(com).fetchall()
    con.close()

    print(wifi_table)

def create_new_wifi(patient_id, wound, ischemia, foot_infection, date, wound_id):
    w_id = wound_id

    if wound_id < 0:
        w_id = generate_wound_id(patient_id)

    com = "INSERT INTO wifi(patient_id, wound, ischemia, foot_infection, date, wound_id) VALUES(?, ?, ?, ?, ?, ?)"
    args = (patient_id, wound, ischemia, foot_infection, date, w_id, )

    con = du.connect(DB)
    con.cursor().execute(com, args)
    con.commit()
    con.close()

def delete_wifi(wifi_id):
    if not wifi_exists(wifi_id):
        raise UnknownEntry("Could not find wifi data to delete")

    com = "DELETE FROM wifi WHERE id = ?"
    args = (wifi_id, )

    con = du.connect(DB)
    con.cursor().execute(com, args)
    con.commit()
    con.close()

def get_all_patient_wounds(patient_id):
    if not patient_exists(patient_id):
        raise UnknownEntry("Could not find patient data")

    com = "SELECT * FROM wounds WHERE patient_id = ?"
    args = (patient_id, )

    con = du.connect(DB)
    wound_data = con.cursor().execute(com, args).fetchall()
    con.close()

    return wound_data

def wound_exists(wnd_id):
    com = "SELECT EXISTS(SELECT 1 FROM wounds WHERE id = ?)"
    args = (wnd_id, )

    con = du.connect(DB)
    wound_ex = con.cursor().execute(com, args).fetchone()[0]
    con.close()

    return wound_ex

def print_all_wounds():
    com = "SELECT * FROM wounds"

    con = du.connect(DB)
    wound_table = con.cursor().execute(com).fetchall()
    con.close()

    print(wound_table)

def create_new_wound(patient_id, wound_size, img_path, date, wound_id):
    w_id = wound_id

    if wound_id < 0:
        w_id = generate_wound_id(patient_id)

    com = "INSERT INTO wounds(patient_id, wound_size, img_path, date, wound_id) VALUES(?, ?, ?, ?, ?)"
    args = (patient_id, wound_size, img_path, date, w_id, )

    con = du.connect(DB)
    con.cursor().execute(com, args)
    con.commit()
    con.close()

def delete_wound(wnd_id):
    if not wound_exists(wnd_id):
        raise UnknownEntry("Could not find wound to delete")

    com = "DELETE FROM wounds WHERE id = ?"
    args = (wnd_id, )

    con = du.connect(DB)
    con.cursor().execute(com, args)
    con.commit()
    con.close()

def generate_wound_id(patient_id):
    wifi_data = get_all_patient_wifi(patient_id)
    wound_data = get_all_patient_wounds(patient_id)

    max_id = 0

    for wifi in wifi_data:
        max_id = max(max_id, wifi[6])

    for wound in wound_data:
        max_id = max(max_id, wound[5])

    return max_id + 1
