from flask import Flask, request

app = Flask(__name__)

@app.before_request
def log_everything():
    print(f"{request.method} {request.path}")
    print("Headers:", dict(request.headers))
    if request.data:
        print("Body:", request.data.decode())

@app.route("/", methods=["GET", "POST", "PUT", "DELETE", "OPTIONS"])
def log_request():
    print(f"{request.method} {request.path}")
    print("Headers:", dict(request.headers))
    if request.data:
        print("Body:", request.data.decode())
    return "OK", 200

app.run(port=8000)
