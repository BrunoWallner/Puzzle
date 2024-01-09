mod http;
mod listener;

use fast_qr::qr::QRBuilder;
use std::{cmp::Ordering, net::IpAddr};

const IP: &str = "0.0.0.0:8000";
const PORT: u16 = 8000;
const ROOT: &str = "./web/";

#[tokio::main]
async fn main() {
    // let root = get_root().unwrap();
    // for iface in if_addrs::get_if_addrs().unwrap() {
    //     println!("{:#?}", iface);
    // }
    let local_ip = get_local_ip();
    println!("listening on: {:#?}", local_ip);
    if let Ok(qrcode) = QRBuilder::new(String::from("http://") + &local_ip).build() {
        println!("{}", qrcode.to_str());
    }

    listener::run(IP).await.unwrap();
}

fn get_local_ip() -> String {
    let interfaces = if_addrs::get_if_addrs().unwrap();
    let mut interfaces = interfaces
        .iter()
        .filter(|x| !x.is_loopback())
        .collect::<Vec<_>>();

    // prev ipv4 over ipv6
    interfaces.sort_by(|a, b| match (a.ip(), b.ip()) {
        (IpAddr::V4(_), IpAddr::V4(_)) => Ordering::Equal,
        (IpAddr::V6(_), IpAddr::V6(_)) => Ordering::Equal,
        (IpAddr::V4(_), IpAddr::V6(_)) => Ordering::Less,
        (IpAddr::V6(_), IpAddr::V4(_)) => Ordering::Greater,
    });

    let interface = interfaces[0];
    let local_ip = interface.ip().to_string() + ":" + &PORT.to_string();
    local_ip
}

// fn get_root() -> io::Result<HashMap<String, Vec<u8>>> {
//     let mut map = HashMap::default();

//     for entry in fs::read_dir(ROOT)? {
//         let entry = entry?;
//         if entry.file_type()?.is_dir() {
//             map.extend(read_folder(entry, "")?);
//         } else {
//             let path = entry.path();
//             // let content = fs::read_to_string(&path)?;
//             let content = fs::read(&path)?;
//             let Ok(name) = entry.file_name().into_string() else {
//                 continue;
//             };
//             let index = format!("/{}", name);
//             map.insert(index, content);
//         }
//     }

//     Ok(map)
// }

// fn read_folder(entry: DirEntry, base_path: &str) -> io::Result<HashMap<String, Vec<u8>>> {
//     let mut map = HashMap::default();

//     if entry.file_type()?.is_dir() {
//         let Ok(name) = entry.file_name().into_string() else {
//             return Err(io::Error::from(io::ErrorKind::InvalidData));
//         };
//         let base_path = format!("{}/{}", base_path, name);

//         for entry in fs::read_dir(entry.path())? {
//             let entry = entry?;
//             map.extend(read_folder(entry, &base_path)?);
//         }
//     } else {
//         let path = entry.path();
//         let content = fs::read(&path)?;
//         let Ok(name) = entry.file_name().into_string() else {
//             return Err(io::Error::from(io::ErrorKind::InvalidData));
//         };
//         let index = format!("{}/{}", base_path, name);

//         map.insert(index, content);
//     }

//     Ok(map)
// }
