from flask import Flask, request
import ssl
import socket

app = Flask(__name__)

@app.route('/run', methods=['POST'])
def run_script():
    data = request.get_json()
    arg = data.get('arg', 'Default Value')
    return {"message": f"Received argument: {arg}"}, 200

if __name__ == '__main__':
    # Get the local machine's hostname
    hostname = socket.gethostname()
    
    # Define SSL context using built-in ssl module
    context = ssl.SSLContext(ssl.PROTOCOL_TLS_SERVER)
    context.load_cert_chain('certificate.pem', 'private_key.pem')
    
    # Run Flask app with SSL context
    app.run(host='0.0.0.0', port=5000, ssl_context=context)