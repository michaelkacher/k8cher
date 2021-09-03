import { browser } from '$app/env'

// todo - mbk: probably better to implement as typescript to 
// get typed return type of get, post 

function browserGet(key) {
    if (browser) {
        let item = sessionStorage.getItem(key)
        if (item == null) {
            item = localStorage.getItem(key)
        }

        if (item) {
            return item;
        }
    }

    return null
}

async function send(method, url, data) {
    try {
        let payload = {
            method,
            headers: {}
        }

        if (data) {
            payload.body = JSON.stringify(data)
        }

        payload.headers['Content-Type'] = 'application/json' // todo - mbk: do I want this only if not form data? if(!(body instanceof FormData))
        payload.headers['Accept'] = 'application/json'  // todo - mbk: can I remove this?
        payload.headers['Access-Control-Allow-Origin'] = '*' // todo - mbk: try to remove
        payload.headers['Access-Control-Allow-Headers'] = 'access-control-allow-origin,content-type' // todo - mbk: try to remove

        const token = browserGet('jwt')
        if (token) {
            payload.headers['Authorization'] = 'Bearer ' + token
        }

        const res = await fetch(url, payload).catch((err) => {
            return {
                success: false,
                message: 'Network Error: ' + err
            }
        })

        if (!res.ok) {
            return {
                success: false,
                message: 'Unsucessful response: ' + res.statusText
            }
        }
        
        const contentType = res.headers.get("content-type");
        if (contentType && contentType.indexOf("application/json") !== -1) {
            const json = await res.json()
            return {
                success: true,
                json
            }
        }

        return {
            success: true
        }
    } catch (err) {
        return {
                success: false,
                message: 'Unkown Error' + err
            }
    }
}

export async function post(url, body) {
    return send('POST', url, body)
}

export async function get(url) {
    return send('GET', url)
}