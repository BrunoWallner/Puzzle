use crate::http;
use std::io::Read;
use std::sync::{Arc, Mutex};
use std::{fs, io};
use tokio::io::{AsyncReadExt, AsyncWriteExt};
use tokio::net::{TcpListener, TcpStream, ToSocketAddrs};

lazy_static::lazy_static! {
    static ref NOT_FOUND_HTML: &'static str = "<html><h1p>NOT FOUND</h1></html>";
    static ref NOT_FOUND: String = format!("HTTP/1.1 404 Not Found\r\nContent-Length: {}\r\n\r\n{}", NOT_FOUND_HTML.len(), *NOT_FOUND_HTML);
    static ref UID: Arc<Mutex<u64>> = Arc::new(Mutex::new(rand::random::<u64>()));
}

const INDEX: &str = "/index.html";

pub async fn run<A: ToSocketAddrs>(addr: A) -> Result<(), io::Error> {
    let listener = TcpListener::bind(addr).await?;

    // for stream in listener.incoming() {
    loop {
        let stream = listener.accept().await;
        match stream {
            Ok((stream, _socket_addr)) => {
                // generate random uid
                let mut mutex_uid = UID.lock().unwrap();
                *mutex_uid += 1;
                let uid = rand::random::<u64>();
                *mutex_uid = uid;
                drop(mutex_uid);

                tokio::spawn(async move {
                    handle_client(stream, uid).await.unwrap();
                });
            }
            Err(error) => {
                eprintln!("Failed to establish connection: {}", error);
            }
        }
    }

    // Ok(())
}

async fn handle_client(mut stream: TcpStream, uid: u64) -> std::io::Result<()> {
    let mut buffer = Vec::new();

    loop {
        let mut chunk = [0_u8; 1024];
        let bytes_read = stream.read(&mut chunk).await?;

        if bytes_read == 0 {
            // End of stream
            break;
        }

        // Extend the buffer with the received chunk
        buffer.extend(&chunk[..bytes_read]);

        // Check if the message is complete
        if buffer.ends_with(b"\r\n\r\n") {
            // Process the received message
            match process_message(&buffer, &mut stream, uid).await {
                Ok(_) => (),
                // Err(e) => eprintln!("error occured: {:?}", e),
                Err(_) => (),
            };

            // Clear the buffer for the next message
            buffer.clear();
        }
    }

    Ok(())
}

async fn process_message(
    message: &[u8],
    stream: &mut TcpStream,
    uid: u64,
) -> Result<(), io::Error> {
    let msg = String::from_utf8_lossy(message);
    let request: http::Request = msg.parse().unwrap();
    let content_type = get_content_type(&request.path);
    let path = convert_path(&request.path);
    match request.method {
        http::Method::Get => {
            // let Some(file) = root.get(&path) else {
            //     stream.write_all(NOT_FOUND.as_bytes()).await?;
            //     return Err(io::Error::new(io::ErrorKind::NotFound, path));
            // };
            let Ok(mut file) = fs::File::open(&path) else {
                stream.write_all(NOT_FOUND.as_bytes()).await?;
                return Err(io::Error::new(io::ErrorKind::NotFound, path));
            };
            let mut file_content: Vec<u8> = Vec::new();
            let Ok(_) = file.read_to_end(&mut file_content) else {
                stream.write_all(NOT_FOUND.as_bytes()).await?;
                return Err(io::Error::new(io::ErrorKind::NotFound, path));
            };
            let Some(host_address) = request.get("Host") else {
                return Err(io::Error::from(io::ErrorKind::InvalidInput));
            };

            // modify string to include @var@ variables if fully valid utf8
            // otherwise do retarded and unnecessary heap allocation
            let mut file: Vec<u8> = if let Ok(string) = String::from_utf8(file_content.to_vec()) {
                include_variables(&string, host_address, &uid.to_string())
                    .as_bytes()
                    .to_vec()
            } else {
                file_content.to_vec()
            };

            let status_line = "HTTP/1.1 200 OK";
            let response = format!(
                "{}{}{}\r\n\r\n",
                status_line,
                if let Some(content_type) = content_type {
                    format!("\r\nContent-Type: {}", content_type)
                } else {
                    format!("")
                },
                format!("\r\nContent-Length: {}", file.len()),
            );
            let mut response = response.as_bytes().to_vec();
            response.append(&mut file);
            // println!("response: {}", response);

            stream.write_all(&response).await?;
        }
        http::Method::Post => (),
    }

    Ok(())
}

fn convert_path(path: &str) -> String {
    let path = match path {
        "/" => String::from(INDEX),
        p => String::from(p),
    };

    String::from(super::ROOT) + &path
}

fn include_variables(file: &str, host_address: &str, uid: &str) -> String {
    let file = file.replace("@HOST_ADDRESS@", host_address);
    let file = file.replace("@UID@", uid);
    file
}

fn get_content_type(file_name: &str) -> Option<String> {
    let Some(extension) = file_name.split(".").last() else {
        return None;
    };
    let x = match extension {
        "html" => "text/html",
        "css" => "text/css",
        "js" => "text/javascript",
        "jpg" => "image/jpeg",
        "png" => "image/png",
        _ => return None,
    };
    Some(String::from(x))
}

// fn prng(mut x: u64) -> u64 {
//     x ^= x << 13;
//     x ^= x >> 7;
//     x ^= x << 17;
//     x
// }
