const HOST_ADDRESS = "@HOST_ADDRESS@";
const HOST_IP = HOST_ADDRESS.split(':')[0];
const WS_PORT = 8001;
const WS_ADDRESS = "ws://" + HOST_IP + ":" + WS_PORT; 

const UID = "@UID@";
let x = -1;
let y = -1;
let id = -1;
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
    case "load_image":
      if (tokens.length < 3) {return};
      let image = tokens[1];
      console.log("image: " + image);
      let src = tokens[2];
      switch (image) {
        case "1":
          load_image_1(src);
          break;
        case "2":
          load_image_2(src);
          break;
      }
	break;
     case "send_meta":
       if (tokens.length < 5) {return};
       x = tokens[1];
       y = tokens[2];
       id = tokens[3];
       let scan_id = tokens[4];
       document.getElementById("position").innerText = "position: (" + x + "|" + y + ") " + "scan_id: " + scan_id;
       //alert("position: " + x + " " + y);
       break;
  }
})

function success() {
    socket.send(UID + ":" + "success" + ":"+ x + ":" + y + ":" + id);
}

function fail() {
    socket.send(UID + ":" + "fail" + ":"+ x + ":" + y + ":" + id);
}

function request_image() {
  socket.send(UID + ":" + "request_image");
}
