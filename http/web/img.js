const c1 = document.getElementById("c1");
const ctx1 = c1.getContext("2d");

const c2 = document.getElementById("c2");
const ctx2 = c2.getContext("2d");

function load_image_1(src) {



  console.log("loading image 1");
  var img = new Image();
  img.onload = function() {
    c1.width = img.width;
    c1.height = img.height;
    ctx1.drawImage(img, 0, 0);
  }

  img.src = "http://" + HOST_ADDRESS + src;
}


function load_image_2(src) {
  var img = new Image();
  img.onload = function() {
    c2.width = img.width;
    c2.height = img.height;
    ctx2.drawImage(img, 0, 0);
  }
  img.src = "http://" + HOST_ADDRESS + src;
}