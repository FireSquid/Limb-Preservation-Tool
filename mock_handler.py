import os
import io
import os.path
import base64
import shutil
import PIL.Image as Image
from flask import Flask, request, send_file, make_response
app = Flask(__name__)
if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)


@app.route("/")
def hello_world():
    return "<p>Hello, World!</p>"


upload_place = 'Upload'
get_place = 'Get'


@app.route("/analyze", methods=['POST', 'GET'])
def analyze():
    patient_string = request.args['patientID'] + '_' + request.args['date']
    upload_des = os.path.join(upload_place, patient_string+'.tmp')
    get_des = os.path.join(get_place, patient_string+'.png')
    # handle put command
    my_resp = make_response('Response')
    if request.method == 'POST':
        if not os.path.exists(upload_place):
            os.mkdir(upload_place)
        with open(upload_des, 'ab') as file:
            chunk = request.get_json()['chunk']
            file.write(base64.b64decode(chunk))
            # print(chunk)
            print("Posting for: " + patient_string)

    if request.method == 'GET':
        # rename .tmp to png for CV algorithm
        png_dest = os.path.join(upload_place, patient_string+'.png')
        if os.path.exists(upload_des):
            os.rename(upload_des, png_dest)
            if not os.path.exists(get_place):
                os.mkdir(get_place)
            # CV producing a file to get_des
            shutil.copyfile(png_dest, get_des)
            os.remove(png_dest)
        else:

            print("path not found for %s or already processed\n" % png_dest)
        # with open(get_des, 'rb')as result:
        # encoding + jsonserialized version  of image

        if os.path.exists(get_des):
            # need to change send_file to other method to prevent safety issue
            my_resp = make_response(send_file(get_des, mimetype='png'))
    # my_resp.headers[ ]
    return my_resp
