import { writable } from 'svelte/store'

// potentials advances
// save to local storage and sync from there
// have flag, persist history, tracks all immutable changes, if function has name, track that
// add async option (does not wait for confirmation from server)
// ability to add jsonschema


// tutorial to write:
// create and start new svelte app `npm init k8cher my-app`, cd my-app, npm install, npm run dev
// startup backend `tilt up` create one optimized for working on front end
// create a normal svelte store, can switch to persistable to save to backend

export function localPersistable(storeName, initialState) {
    let value = initialState

    const devTools =
        window.__REDUX_DEVTOOLS_EXTENSION__ &&
        window.__REDUX_DEVTOOLS_EXTENSION__.connect();

    const { subscribe, set } = writable(initialState, (setFunc) => {
        var item = localStorage.getItem(`store-${storeName}`)
        if (item) {
            value = JSON.parse(item)
            setFunc(value)
        } else {
            setFunc(initialState)
        }

        return () => console.log(`no more subscribers for store ${storeName}`)
    })

    async function persist(json) {
        localStorage.setItem(`store-${storeName}`, JSON.stringify(json))
        value = json
        set(json)
    }

    return {
        subscribe,
        update: async (updateFunc, actionName) => {
            if (devTools && actionName) {
                devTools.send(actionName, value)
            }

            persist(updateFunc(value))
        },
    }
}

