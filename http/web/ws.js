const HOST_ADDRESS = "@HOST_ADDRESS@";
const HOST_IP = HOST_ADDRESS.split(':')[0];
const WS_PORT = 8001;
const WS_ADDRESS = "ws://" + HOST_IP + ":" + WS_PORT;

let Orders = [];

const UID = "@UID@";
// will compile to: const socket = new WebSocket(url)
const socket = new WebSocket(WS_ADDRESS);

socket.addEventListener("open", (event) => {
  socket.send(UID + ":" + "login");
  console.log("logged in");
})

socket.addEventListener("message", (event) => {
  let msg = event.data;
  console.log("got: " + msg);

  let tokens = msg.split(':');
  if (tokens.length < 1) {return};

  let request = tokens[0];
  switch(request) {
    case "assign":
      if (tokens.length != 3) {return};
      let uuid = tokens[1];
      let scan_id = tokens[2];
      Orders.push([uuid, scan_id]);
      if (NO_NEW_ORDER) {
        load_next();
      }
	    break;
    case "invalid_user":
      // document.body.style.display = 'none';
      document.getElementById("container").style.display = 'none';
      socket.close();
      
      // alert("Another user already logged in");
      custom_alert("Another user already logged in");
      break;
  }
})

function logoff() {
  socket.send(UID + ":" + "logoff");
}

function success() {
    socket.send(UID + ":" + "success");
    load_next();
  // socket.send(UID + ":" + "success" + ":" + id);
}

function fail() {
    socket.send(UID + ":" + "fail");
    load_next();
}
