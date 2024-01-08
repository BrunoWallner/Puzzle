const c0 = document.getElementById("c0");
const ctx0 = c0.getContext("2d");

const c1 = document.getElementById("c1");
const ctx1 = c1.getContext("2d");

let NO_NEW_ORDER = true;

function switch_image() {
  if (c0.style.display === 'none') {
    c1.style.display = "none";
    c0.style.display = "block";
  } else {
    c1.style.display = "block";
    c0.style.display = "none";
  }
}

function load_next() {
  if (Orders.length == 0) {
    NO_NEW_ORDER = true;
    alert("Keine neuen Aufträge");
    return;
  }
  if (Orders.length >= 1) {
    NO_NEW_ORDER = false;
  }

  let order = Orders.shift();
  let uuid = order[0];
  let scan_id = order[1];
  let image0 = "/" + uuid + "_0" + ".png";
  let image1 = "/" + uuid + "_1" + ".png";

  document.getElementById("scan").innerText = "SCAN: " + scan_id;

  // load and draw image 0
  var img0 = new Image();
  img0.onload = function() {
    c0.width = img0.width;
    c0.height = img0.height;
    ctx0.drawImage(img0, 0, 0);
  }
  img0.src = "http://" + HOST_ADDRESS + image0;

  // load and draw image 1
  var img1 = new Image();
  img1.onload = function() {
    c1.width = img1.width;
    c1.height = img1.height;
    ctx1.drawImage(img1, 0, 0);
  }
  img1.src = "http://" + HOST_ADDRESS + image1;
}
