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
      if (tokens.length != 2) {return};
      let uuid = tokens[1];
      Orders.push(uuid);
      if (NO_NEW_ORDER) {
        load_next();
      }
	    break;
  }
})

function success() {
    socket.send(UID + ":" + "success");
    load_next();
  // socket.send(UID + ":" + "success" + ":" + id);
}

function fail() {
    socket.send(UID + ":" + "fail");
    load_next();
}
