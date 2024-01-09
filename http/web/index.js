window.addEventListener("beforeunload", () => {
    event.preventDefault();
    event.returnValue = '';
});

function custom_alert(message) {
    var modal = document.getElementById("myModal");
    // Get the <span> element that closes the modal
    var span = modal.getElementsByClassName("close")[0];
    var msg = modal.getElementsByClassName("modal-message")[0];
    msg.innerText = message;
    // When the user clicks on <span> (x), close the modal
    function close() {
      modal.style.display = "none";
    }

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function(event) {
      if (event.target == modal) {
        close();
      }
    }
    modal.style.display = "block";
}
