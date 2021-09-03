// or use basis of writable??
// export const writable = (initial_value = 0) => {

//     let value = initial_value         // content of the store
//     let subs = []                     // subscriber's handlers
  
//     const subscribe = (handler) => {
//       subs = [...subs, handler]                                 // add handler to the array of subscribers
//       handler(value)                                            // call handler with current value
//       return () => subs = subs.filter(sub => sub !== handler)   // return unsubscribe function
//     }
  
//     const set = (new_value) => {
//       if (value === new_value) return         // same value, exit
//       value = new_value                       // update value
//       subs.forEach(sub => sub(value))         // update subscribers
//     }
  
//     const update = (update_fn) => set(update_fn(value))   // update function
  
//     return { subscribe, set, update }       // store contract
//   }



import { writable } from 'svelte/store'
import { get, post } from '$lib/utils/apiHelper'
import { serverUrl } from '$lib/utils/env'

// const initialState = {
//     actorState: undefined,
//     actions: [],
//     isLoading: false,
//     error: ""
// }

export function persistable(storeName, initialState) {
    const { subscribe, set, update } = writable(initialState);

    return {
        subscribe,
        initializeFromApi: async () => {
            update(state => (state = { ...state, isLoading: true }))
            try {
                const res = await get(`${serverUrl}store/get?storeName=${storeName}`)
                update(state => (state = { ...state, actorState: res.json }))
                // set(JSON.parse(res.json))
            } catch (e) {
                alert(e.message); // todo - mbk: make part of apiHelper and remove alert
            } finally {
                update(state => (state = { ...state, isLoading: false }))
            }
        },
        // a property to update and the json value to be updated
        set: async (json) => {
            update(state => (state = { ...state, isLoading: true }))
            try {
                // todo - mbk: instead of awaiting these, can I add them to a queue of
                // items to be asynchronously processed? Is then using another store like redux better then?
                const res = await post(`${serverUrl}store/set`, {
                    name: storeName,
                    json
                })
                // update(state => (state = { ...state, actorState: json }))
                //set(json)
            } catch (e) {
                alert(e.message); // todo - mbk: make part of apiHelper and remove alert
            } finally {
                update(state => (state = { ...state, isLoading: false }))
            }
        },
        updateProperty: async (propertyName, json) => {
            update(state => (state = { ...state, isLoading: true }))
            try {
                // todo - mbk: instead of awaiting these, can I add them to a queue of
                // items to be asynchronously processed? Is then using another store like redux better then?
                const res = await post(`${serverUrl}store/update`, {
                    
                })
                update(state => (state = { ...state, actorState: json }))
            } catch (e) {
                alert(e.message); // todo - mbk: make part of apiHelper and remove alert
            } finally {
                update(state => (state = { ...state, isLoading: false }))
            }
        }
    };
}

