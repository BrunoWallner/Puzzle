use std::{collections::HashMap, str::FromStr};

#[derive(Copy, Clone, Debug)]
pub enum Method {
    Get,
    Post,
}

#[derive(Clone, Debug)]
pub struct Request {
    pub method: Method,
    pub path: String,
    fields: HashMap<String, String>,
}
impl Request {
    pub fn get(&self, field: &str) -> Option<&String> {
        self.fields.get(field)
    }
}
impl FromStr for Request {
    type Err = ();

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let mut lines: Vec<&str> = s.split("\r\n").collect();
        if lines.is_empty() {
            return Err(());
        }

        let header = lines.remove(0);
        let method = match header.split(' ').nth(0) {
            Some("GET") => Method::Get,
            Some("POST") => Method::Post,
            _ => return Err(()),
        };
        let Some(path) = header.split(' ').nth(1) else {
            return Err(());
        };
        let path = path.to_string();

        let mut fields: HashMap<String, String> = HashMap::default();
        for line in lines {
            let mut split: Vec<&str> = line.split(":").collect();
            if split.len() < 2 {
                continue;
            }

            let field = split.remove(0);
            // let Some(value) = split.drain(1..).collect() else {continue};
            let mut value: String = String::new();
            split.iter().for_each(|x| {
                value.push_str(x);
                value.push(':')
            });
            value.pop();
            fields.insert(field.trim().to_string(), value.trim().to_string());
        }

        Ok(Request {
            method,
            path,
            fields,
        })
    }
}
