import base64
import os.path
import os
import sqlite3
from flask import Flask, request, make_response, send_file
import thesizer
import resizer
import authenticator as auth
# the port and address of aws (this) server network accessible from public ip has been allowed at 0.0.0.0/0 on ipv4
# and ::0 for ipv6. In such case, --host=0.0.0.0 argument is needed for task run.
import sys
app = Flask(__name__)
print("flask pid: %d" % os.getpid())
#app.config['MAX_CONTENT_LENGTH'] = 1
print(app.config)

DATABASE = '/exl'


def get_db():
    db = getattr(g, '_database', None)
    if db is None:
        db = g._database = sqlite3.connect(DATABASE)
    return db


def close_connection(exception):
    db = getattr(g, '_database', None)
    if db is not None:
        db.close()


#con = sqlite3.connect("data/portal_mammals.sqlite")
#
# Load the data into a DataFrame
#surveys_df = pd.read_sql_query("SELECT * from surveys", con)
#
# Select only data for 2002
#surveys2002 = surveys_df[surveys_df.year == 2002]
#
# Write the new DataFrame to a new SQLite table
#surveys2002.to_sql("surveys2002", con, if_exists="replace")
#
# con.close()
app = Flask(__name__)
if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)


@app.route("/")
def hello_world():
    return "<p>Hello, World!</p>"


@app.route("/getscore", methods=['GET'])
def get_score():
    patientID = request.args.get('patientID')

    return "<p>Hello, World!</p>"

upload_place = 'Post'
out_place='out'
in_place = 'in'

@app.route("/analyze", methods=['POST', 'GET'])
def analyze():
    # handle put command
    patient_string = request.args['patientID'] + '_' + request.args['date']
    upload_des = os.path.join(upload_place, patient_string+'.tmp')
    png_dest = os.path.join(upload_place, patient_string+'.png')
    save_des = os.path.join(in_place, patient_string+'.png')
    get_des = os.path.join(out_place, patient_string+'.png')
    print("replying analyze POST")
    my_resp = make_response('Response')
    if request.method == 'POST':
        if not os.path.exists(upload_place):
            os.mkdir(upload_place)
        with open(upload_des, 'ab') as file:
            chunk = request.get_json()['chunk']
            file.write(base64.b64decode(chunk))
            #file.write(base64.b64decode(chunk))    #not sure if two of these same lines is causing issue???
            # print(chunk) print("Posting for: " + patient_string)
    if request.method == 'GET':

        wound_size = -1.0
        if os.path.exists(upload_des):

            os.rename(upload_des, png_dest)

            if not os.path.exists(in_place):
                os.mkdir(in_place)
            if not os.path.exists(out_place):
                os.mkdir(out_place)
            # CV producing a file to get_des
           # wound_size = 
            print(png_dest)
            resizer.size(png_dest)
            wound_size = thesizer.cv(png_dest, 0.75,save_des,get_des, True)  # saved in in/ and output in out/

            #os.remove(png_dest) # remove png in Post/
        else: print("path not found for %s or already processed\n" % png_dest)

        if os.path.exists(get_des):
            print(f"Setting wound_size_hdr header in response to {wound_size}")
            # need to change send_file to other method to prevent safety issue
            my_resp = make_response(send_file(get_des, mimetype='png'))
            my_resp.headers['wound_size_hdr'] = str(wound_size)
        else:
            print("CV result not found")
            return "CV result not found"
    # my_resp.headers[ ]
    return my_resp


@app.route("/auth", methods=['POST'])
def authenticate():
    print("Validating Authentication Request...")
    username = request.json.get('username')
    password = request.json.get('password')

    verified = auth.authenticate_login(username, password)

    auth_resp_str = "LOGIN_VALID" if verified else "INVALID_LOGIN"

    print(auth_resp_str)
    auth_resp = make_response(auth_resp_str)

    return auth_resp


@app.route("/new_user", methods=['POST'])
def create_new_user():
    print("Creating New User...")

    username = request.json.get('username')
    password = request.json.get('password')
    name = request.json.get('name')
    email = request.json.get('email')

    auth_resp_str = "USER_CREATED"

    try:
        auth.create_user(username, password, name, email)
    except:
        auth_resp_str = "USER_CREATION_FAILED"

    print(auth_resp_str)
    auth_resp = make_response(auth_resp_str)

    return auth_resp


@app.route("/delete_user", methods=['POST'])
def delete_user():
    print("Deleting User...")
    username = request.json.get('username')
    password = request.json.get('password')

    auth_resp_str = "USER_DELETED"

    try:
        if not auth.delete_user(username, password):
            auth_resp_str = "USER_DELETION_FAILED_AUTHENTICATION"
    except:
        auth_resp_str = "USER_DELETION_FAILED_MISSING"

    print(auth_resp_str)
    auth_resp = make_response(auth_resp_str)

    return auth_resp
